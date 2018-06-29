Shader "Underwater/Standard"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		[Header(Caustics)]
		_Caustics("Caustics Texture (RGBA)", 2D) = "white" {}
		_CausticsCoord("Tiling(XY) Offset(ZW) - Overrides texture coords", Vector) = (0.5,0.5,0,0)
		_CausticsSpeed("Speed", Float) = 1
		_CausticsBoost("Boost", Range(0,1)) = 0
		_CausticsIntensity0("Intensity A", Range(0, 1)) = 1
		_CausticsIntensity1("Intensity B", Range(0, 1)) = 0
		_CausticsPosition0("Position A (World Y)", Float) = 2
		_CausticsPosition1("Position B (World Y)", Float) = 4
		[Header(Fog)]
		_FogColor0("Color A", Color) = (0.004, 0.271, 0.302, 1)
		_FogColor1("Color B", Color) = (0.004, 0.271, 0.302, 1)
		_FogIntensity0("Intensity A", Range(0, 1)) = 1
		_FogIntensity1("Intensity B", Range(0, 1)) = 1
		_FogPosition0("Position A (World Y)", Float) = 0
		_FogPosition1("Position B (World Y)", Float) = 6
		_FogStart("Start", Float) = 0
		_FogEnd("End", Float) = 15
		[Header(Standard Shader)]
		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0
		[HideInInspector]
		_Animation( "Animation", Float ) = 0
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf CustomStandard fullforwardshadows nofog finalcolor:applyfog

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _ParallaxMap;
		sampler2D _Caustics;
		float4 _CausticsCoord;
		float _CausticsSpeed;
		fixed _CausticsBoost;
		fixed _CausticsIntensity0;
		fixed _CausticsIntensity1;
		float _CausticsPosition0;
		float _CausticsPosition1;
		float3 _FogColor0;
		float3 _FogColor1;
		fixed _FogIntensity0;
		fixed _FogIntensity1;
		float _FogPosition0;
		float _FogPosition1;
		half _FogStart;
		half _FogEnd;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_ParallaxMap;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputStandardCustom
		{
			fixed3 worldPos;
			fixed3 Albedo;		// base (diffuse or specular) color
			fixed3 Normal;		// tangent space normal, if written
			half3 Emission;
			half Metallic;		// 0=non-metal, 1=metal
			half Smoothness;	// 0=rough, 1=smooth
			half Occlusion;		// occlusion (default 1)
			fixed Alpha;		// alpha for transparencies
			fixed3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;

		float3 getCaustics(float3 worldPos)
		{
			float4 caustics0 = tex2D(_Caustics, worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics1 = tex2D(_Caustics, worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics2 = tex2D(_Caustics, worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
			float4 caustics3 = tex2D(_Caustics, worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));
			float3 caustics = saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a);

			return caustics;
		}

		float getCausticsIntensity(float3 worldPos, float3 worldNormal)
		{
			float causticsIntensity = clamp((worldPos.y - _CausticsPosition0) / (_CausticsPosition1 - _CausticsPosition0), 0.0f, 1.0f);
			causticsIntensity = lerp(_CausticsIntensity0, _CausticsIntensity1, causticsIntensity);
			causticsIntensity = causticsIntensity * min(1.0f, max(0.0f, dot(worldNormal, float3(0, 1, 0)) + 0.5f));

			return causticsIntensity;
		}

		inline half4 LightingCustomStandard(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi)
		{
			s.Normal = normalize(s.Normal);

			half oneMinusReflectivity;
			half3 specColor;
			s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

			// Caustics
			float4 caustics0 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics1 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics2 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
			float4 caustics3 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));
			float3 caustics = ((saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * (_CausticsBoost + 1)) + _CausticsBoost);

			float causticsIntensity = getCausticsIntensity(s.worldPos, s.worldNormal);

			s.Albedo = lerp(s.Albedo, s.Albedo * caustics, causticsIntensity);

			// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
			// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
			half outputAlpha;
			s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

			half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
			c.rgb += UNITY_BRDF_GI(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
			c.a = outputAlpha;
			return c;
		}

		inline half4 LightingCustomStandard_Deferred(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
		{
			half oneMinusReflectivity;
			half3 specColor;
			s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

			// Caustics
			float4 caustics0 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics1 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			float4 caustics2 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
			float4 caustics3 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));
			float3 caustics = ((saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * (_CausticsBoost + 1)) + _CausticsBoost);

			float causticsIntensity = getCausticsIntensity(s.worldPos, s.worldNormal);

			s.Albedo = lerp(s.Albedo, s.Albedo * caustics, causticsIntensity);

			half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
			c.rgb += UNITY_BRDF_GI(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);

			outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);
			outSpecSmoothness = half4(specColor, s.Smoothness);
			outNormal = half4(s.Normal * 0.5 + 0.5, 1);
			half4 emission = half4(s.Emission + c.rgb, 1);
			return emission;
		}

		inline void LightingCustomStandard_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			gi = UnityGlobalIllumination(data, s.Occlusion, s.Smoothness, s.Normal);
		}

		void applyfog(Input IN, SurfaceOutputStandardCustom o, inout fixed4 color)
		{
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			float mixed = clamp((IN.worldPos.y - _FogPosition0) / (_FogPosition1 - _FogPosition0), 0, 1);
			float3 fogColor = lerp(_FogColor0 * _FogIntensity0, _FogColor1 * _FogIntensity1, mixed);
			mixed = lerp(_FogIntensity0, _FogIntensity1, mixed);
			mixed = saturate((_FogStart - dist) / (_FogStart - _FogEnd)) * mixed;

#ifdef UNITY_PASS_FORWARDADD
			fogColor = 0;
#endif

			color.rgb = ( color.rgb * ( 1 - mixed) ) + ( fogColor * mixed);
		}

		void surf (Input IN, inout SurfaceOutputStandardCustom o)
		{
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.worldNormal = WorldNormalVector(IN, o.Normal);
			o.worldPos = IN.worldPos;

			float3 caustics = getCaustics(o.worldPos);
			float causticsIntensity = getCausticsIntensity(o.worldPos, o.worldNormal);

			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			o.Metallic = lerp(_Metallic, _Metallic * caustics, causticsIntensity);
			o.Smoothness = lerp(_Glossiness, _Glossiness * caustics, causticsIntensity);
			o.Alpha = c.a;
		}
		ENDCG
	} 

	FallBack "Diffuse"
}
