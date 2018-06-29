// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Underwater/Mobile Fast"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
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
		[HideInInspector]
		_Animation("Animation", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma multi_compile SHADER_CONTROL SCRIPT_CONTROL
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _Caustics;
			uniform float4 _CausticsCoord;
			uniform fixed _CausticsSpeed;
			uniform fixed _CausticsBoost;
			uniform fixed _CausticsIntensity0;
			uniform fixed _CausticsIntensity1;
			uniform fixed _CausticsPosition0;
			uniform fixed _CausticsPosition1;
			uniform fixed3 _FogColor0;
			uniform fixed3 _FogColor1;
			uniform fixed _FogIntensity0;
			uniform fixed _FogIntensity1;
			uniform fixed _FogPosition0;
			uniform fixed _FogPosition1;
			uniform half _FogStart;
			uniform half _FogEnd;

#ifdef SCRIPT_CONTROL
			uniform fixed _Animation;
#endif

			uniform float4 _MainTex_ST;
			uniform fixed4 _LightColor0;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 position : SV_POSITION;
				float4 mainTexCoord : TEXCOORD0;
				float4 worldSpacePos : TEXCOORD1;
				fixed4 ambientColor : COLOR0;
				fixed4 fogColor : COLOR1;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				float3 normalDirection = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				fixed diffuseReflection = max(0, dot(normalDirection, lightDirection));

				float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));

				o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);

				fixed mixed = clamp((o.worldSpacePos.y - _FogPosition0) / (_FogPosition1 - _FogPosition0), 0, 1);
				fixed3 fogColor = lerp(_FogColor0 * _FogIntensity0, _FogColor1 * _FogIntensity1, mixed);

				mixed = lerp(_FogIntensity0, _FogIntensity1, mixed);
				mixed = saturate((_FogStart - dist) / (_FogStart - _FogEnd)) * mixed;

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);

				o.position = UnityObjectToClipPos(v.vertex);
				o.mainTexCoord = v.texcoord;
				o.ambientColor = fixed4(ShadeSH9(half4(worldNormal, 1)), (1 - mixed));
				o.fogColor = fixed4((fogColor * mixed), diffuseReflection * (1 - mixed));
				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
				fixed4 color = tex2D(_MainTex, _MainTex_ST.xy * i.mainTexCoord.xy + _MainTex_ST.zw);

#ifdef SCRIPT_CONTROL
				fixed4 caustics0 = tex2D(_Caustics, i.worldSpacePos.zx * _CausticsCoord.xy + float2(_Animation, 0));
				fixed4 caustics1 = tex2D(_Caustics, i.worldSpacePos.zx * _CausticsCoord.xy + float2(-_Animation, 0));
				fixed4 caustics2 = tex2D(_Caustics, i.worldSpacePos.zx * _CausticsCoord.xy + float2(0, _Animation));
				fixed4 caustics3 = tex2D(_Caustics, i.worldSpacePos.zx * _CausticsCoord.xy + float2(0, -_Animation));
#else
				fixed4 caustics0 = tex2D(_Caustics, i.worldSpacePos.xz * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
				fixed4 caustics1 = tex2D(_Caustics, i.worldSpacePos.xz * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
				fixed4 caustics2 = tex2D(_Caustics, i.worldSpacePos.xz * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
				fixed4 caustics3 = tex2D(_Caustics, i.worldSpacePos.xz * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));
#endif

				fixed4 caustics = ((saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * (_CausticsBoost + 1)) + _CausticsBoost);

				fixed causticsIntensity = clamp((i.worldSpacePos.y - _CausticsPosition0) / (_CausticsPosition1 - _CausticsPosition0), 0, 1);
				causticsIntensity = lerp(_CausticsIntensity0, _CausticsIntensity1, causticsIntensity);

				fixed4 ambient = lerp(i.ambientColor, i.ambientColor * caustics, causticsIntensity);

				fixed4 light = ((caustics * causticsIntensity) + (1 - causticsIntensity)) * i.fogColor.a * _LightColor0;
				return color * (light + (ambient * i.ambientColor.a)) + fixed4(i.fogColor.rgb,1);
			}

			ENDCG
		}
	}

	FallBack "Mobile/VertexLit"
}
