// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Underwater/Terrain-AddPass"
{
	Properties
	{
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "black" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		// Custom Properties
		_Caustics("Caustics Texture (RGBA)", 2D) = "white" {}
		_CausticsCoord("Tiling(XY) Offset(ZW) - Overrides texture coords", Vector) = (0.5,0.5,0,0)
		_CausticsSpeed("Speed", Float) = 1
		_CausticsBoost("Boost", Range(0,1)) = 0
		_CausticsIntensity0("Intensity A", Range(0, 1)) = 1
		_CausticsIntensity1("Intensity B", Range(0, 1)) = 0
		_CausticsPosition0("Position A (World Y)", Float) = 2
		_CausticsPosition1("Position B (World Y)", Float) = 4
		_FogColor0("Color A", Color) = (0.004, 0.271, 0.302, 1)
		_FogColor1("Color B", Color) = (0.004, 0.271, 0.302, 1)
		_FogIntensity0("Intensity A", Range(0, 1)) = 1
		_FogIntensity1("Intensity B", Range(0, 1)) = 1
		_FogPosition0("Position A (World Y)", Float) = 0
		_FogPosition1("Position B (World Y)", Float) = 6
		_FogStart("Start", Float) = 0
		_FogEnd("End", Float) = 15
		[HideInInspector]
		_Animation("Animation", Float) = 0
	}

	CGINCLUDE
		#pragma surface surf CustomLambert noambient nofog decal:add vertex:SplatmapVert finalcolor:myfinal exclude_path:prepass exclude_path:deferred
		#pragma multi_compile SHADER_CONTROL SCRIPT_CONTROL
		#pragma target 3.0
		#define TERRAIN_SPLAT_ADDPASS

		struct Input
		{
			float2 uv_Splat0 : TEXCOORD0;
			float2 uv_Splat1 : TEXCOORD1;
			float2 uv_Splat2 : TEXCOORD2;
			float2 uv_Splat3 : TEXCOORD3;
			float2 tc_Control : TEXCOORD4;	// Not prefixing '_Contorl' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
			float3 worldPos;
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

#ifdef _TERRAIN_NORMAL_MAP
		sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
#endif

		sampler2D _Caustics;
		float4 _CausticsCoord;
		fixed _CausticsSpeed;
		fixed _CausticsBoost;
		fixed _CausticsIntensity0;
		fixed _CausticsIntensity1;
		fixed _CausticsPosition0;
		fixed _CausticsPosition1;
		fixed3 _FogColor0;
		fixed3 _FogColor1;
		uniform fixed _FogIntensity0;
		uniform fixed _FogIntensity1;
		uniform fixed _FogPosition0;
		uniform fixed _FogPosition1;
		uniform half _FogStart;
		uniform half _FogEnd;

		void SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	// Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
			float4 pos = UnityObjectToClipPos(v.vertex);

#ifdef _TERRAIN_NORMAL_MAP
			v.tangent.xyz = cross(v.normal, float3(0, 0, 1));
			v.tangent.w = -1;
#endif
		}

		void SplatmapMix(Input IN, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
		{
			splat_control = tex2D(_Control, IN.tc_Control);
			weight = dot(splat_control, half4(1, 1, 1, 1));

#ifndef UNITY_PASS_DEFERRED
			// Normalize weights before lighting and restore weights in applyWeights function so that the overal
			// lighting result can be correctly weighted.
			// In G-Buffer pass we don't need to do it if Additive blending is enabled.
			// TODO: Normal blending in G-buffer pass...
			splat_control /= (weight + 1e-3f); // avoid NaNs in splat_control
#endif

#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
			clip(weight - 0.0039 /*1/255*/);
#endif

			mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2);
			mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3);

#ifdef _TERRAIN_NORMAL_MAP
			fixed4 nrm = 0.0f;
			nrm += splat_control.r * tex2D(_Normal0, IN.uv_Splat0);
			nrm += splat_control.g * tex2D(_Normal1, IN.uv_Splat1);
			nrm += splat_control.b * tex2D(_Normal2, IN.uv_Splat2);
			nrm += splat_control.a * tex2D(_Normal3, IN.uv_Splat3);
			mixedNormal = UnpackNormal(nrm);
#endif
		}

		void SplatmapApplyWeight(inout fixed4 color, fixed weight)
		{
			color.rgb *= weight;
			color.a = 1.0f;
		}

		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			fixed3 worldPos;
		};

#ifdef SCRIPT_CONTROL
		fixed _Animation;
#endif

		half4 LightingCustomLambert(SurfaceOutputCustom s, half3 lightDir, half atten)
		{
			half diffuseReflection = dot(s.Normal, lightDir);

			float dist = distance(_WorldSpaceCameraPos, s.worldPos);

			fixed mixed = clamp((s.worldPos.y - _FogPosition0) / (_FogPosition1 - _FogPosition0), 0, 1);
			fixed3 fogColor = lerp(_FogColor0 * _FogIntensity0, _FogColor1 * _FogIntensity1, mixed);

			mixed = lerp(_FogIntensity0, _FogIntensity1, mixed);
			mixed = saturate((_FogStart - dist) / (_FogStart - _FogEnd)) * mixed;

#ifdef DIRECTIONAL
			fogColor = fogColor * mixed;
#else
			fogColor = fogColor * mixed * atten;
#endif

			fixed lightColor = diffuseReflection * atten * (1 - mixed);
#ifdef SCRIPT_CONTROL
			fixed4 caustics0 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_Animation, 0));
			fixed4 caustics1 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(-_Animation, 0));
			fixed4 caustics2 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(0, _Animation));
			fixed4 caustics3 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(0, -_Animation));
#else
			fixed4 caustics0 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			fixed4 caustics1 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			fixed4 caustics2 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
			fixed4 caustics3 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));
#endif
			fixed3 caustics = ((saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * (_CausticsBoost + 1)) + _CausticsBoost);

			fixed causticsIntensity = clamp((s.worldPos.y - _CausticsPosition0) / (_CausticsPosition1 - _CausticsPosition0), 0, 1);
			causticsIntensity = lerp(_CausticsIntensity0, _CausticsIntensity1, causticsIntensity);

			fixed3 light = lightColor * _LightColor0;

#ifdef DIRECTIONAL
			fixed3 ambient = (UNITY_LIGHTMODEL_AMBIENT.xyz * (1 - mixed));
			light = light * ((caustics * causticsIntensity) + (1 - causticsIntensity));
#else
			fixed3 ambient = (UNITY_LIGHTMODEL_AMBIENT.xyz * (1 - mixed)) * atten;
#endif

			half4 c;

			c.rgb = s.Albedo * ( light + ambient ) + fogColor;
			c.a = 1;

			return c;
		}

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);
			o.worldPos = IN.worldPos;
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
		}

		void myfinal(Input IN, SurfaceOutputCustom o, inout fixed4 color)
		{
			SplatmapApplyWeight(color, o.Alpha);
		}

	ENDCG

	Category
	{
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-99"
			"IgnoreProjector"="True"
			"RenderType" = "Opaque"
		}
		// TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
		// Use two sub-shaders to simulate different features for different targets and still fallback correctly.
		SubShader
		{ // for sm3.0+ targets
			CGPROGRAM
				#pragma target 3.0
				#pragma multi_compile __ _TERRAIN_NORMAL_MAP
			ENDCG
		}
		SubShader
		{ // for sm2.0 targets
			CGPROGRAM
			ENDCG
		}
	}

	Fallback off
}
