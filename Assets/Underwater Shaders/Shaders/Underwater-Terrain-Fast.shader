// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Underwater/Terrain Fast"
{
	Properties
	{
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
		// Custom Properties
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
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _Control;
			float4 _Control_ST;
			sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
			float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
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

			uniform float4 _MainTex_ST;
			uniform fixed4 _LightColor0;

#ifdef SCRIPT_CONTROL
			uniform fixed _Animation;
#endif

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
				float2 tc_Control : TEXCOORD2;
				float4 uvSplat01 : TEXCOORD3;
				float4 uvSplat23 : TEXCOORD4;				
				fixed4 ambientColor : COLOR0;
				fixed4 fogColor : COLOR1;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);
				
				float2 uvSplat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
				float2 uvSplat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
				float2 uvSplat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
				float2 uvSplat3 = TRANSFORM_TEX(v.texcoord, _Splat3);

				o.uvSplat01 = float4(uvSplat0, uvSplat1);
				o.uvSplat23 = float4(uvSplat2, uvSplat3);

				float3 normalDirection = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				fixed diffuseReflection = max(0, dot(normalDirection, lightDirection));

				float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));

				o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);

				fixed mixed = clamp((o.worldSpacePos.y - _FogPosition0) / (_FogPosition1 - _FogPosition0), 0, 1);
				fixed3 fogColor = lerp(_FogColor0 * _FogIntensity0, _FogColor1 * _FogIntensity1, mixed);

				mixed = lerp(_FogIntensity0, _FogIntensity1, mixed);
				mixed = saturate((_FogStart - dist) / (_FogStart - _FogEnd)) * mixed;

				o.position = UnityObjectToClipPos(v.vertex);
				o.mainTexCoord = v.texcoord;
				o.ambientColor = fixed4(UNITY_LIGHTMODEL_AMBIENT.xyz, (1 - mixed));
				o.fogColor = fixed4((fogColor * mixed), diffuseReflection * (1 - mixed));
				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
				fixed4 splat_control = tex2D(_Control, i.tc_Control);

				fixed4 color = 0.0f;
				color += splat_control.r * tex2D(_Splat0, i.uvSplat01.xy);
				color += splat_control.g * tex2D(_Splat1, i.uvSplat01.zw);
				color += splat_control.b * tex2D(_Splat2, i.uvSplat23.xy);
				color += splat_control.a * tex2D(_Splat3, i.uvSplat23.zw);

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

				fixed4 light = ((caustics * causticsIntensity) + (1 - causticsIntensity)) * i.fogColor.a * _LightColor0;
				color = color * (light + (i.ambientColor * i.ambientColor.a)) + fixed4(i.fogColor.rgb,1);
				return color;
			}

			ENDCG
		}
	}
	
	Dependency "AddPassShader" = "Hidden/Underwater/Terrain Fast-AddPass"
	Dependency "BaseMapShader" = "Underwater/Mobile Fast"
	Dependency "Details0" = "Hidden/TerrainEngine/Details/Vertexlit"
	Dependency "Details1" = "Hidden/TerrainEngine/Details/WavingDoublePass"
	Dependency "Details2" = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
	Dependency "Tree0" = "Hidden/TerrainEngine/BillboardTree"
	
	FallBack "Underwater/Mobile Fast"
}
