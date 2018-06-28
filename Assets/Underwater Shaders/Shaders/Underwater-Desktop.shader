Shader "Underwater/Desktop"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		[Header(Caustics)]
		_Caustics("Caustics Texture (RGBA)", 2D) = "white" {}
		_CausticsCoord("Tiling(XY) Offset(ZW) - Overrides texture coords", Vector) = (0.5,0.5,0,0)
		_CausticsSpeed("Speed", Float) = 1
		_CausticsBoost( "Boost", Range(0,1)) = 0
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
		[HideInInspector]
		_Animation("Animation", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
			#pragma surface surf CustomLambert fullforwardshadows nofog noambient
			#pragma multi_compile SHADER_CONTROL SCRIPT_CONTROL
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
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

#ifdef SCRIPT_CONTROL
			fixed _Animation;
#endif

			struct SurfaceOutputCustom
			{
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				half Specular;
				fixed Gloss;
				fixed Alpha;
				fixed3 worldPos;
				fixed3 worldNormal;
			};

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
				fixed3 caustics = ( ( saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * ( _CausticsBoost + 1 ) ) + _CausticsBoost );

				fixed causticsIntensity = clamp((s.worldPos.y - _CausticsPosition0) / (_CausticsPosition1 - _CausticsPosition0), 0, 1);
				causticsIntensity = lerp(_CausticsIntensity0, _CausticsIntensity1, causticsIntensity);

				fixed3 light = lightColor * _LightColor0;

#ifdef DIRECTIONAL
				fixed3 ambient = ShadeSH9(half4(s.worldNormal, 1)) * (1 - mixed);
				light = lerp( light, light * caustics, causticsIntensity );
#else
				fixed3 ambient = (ShadeSH9(half4(s.worldNormal, 1)) * (1 - mixed)) * atten;
#endif
				ambient = lerp( ambient, ambient * caustics, causticsIntensity );

				half4 c;

				c.rgb = s.Albedo * ( light + ambient ) + fogColor;
				c.a = s.Alpha;
				
				return c;
			}

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 worldPos;
				float3 worldNormal;
				INTERNAL_DATA
			};

			void surf (Input IN, inout SurfaceOutputCustom o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.worldNormal = WorldNormalVector(IN, o.Normal);
				o.worldPos = IN.worldPos;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		ENDCG  
	}

	FallBack "Underwater/Mobile"
}