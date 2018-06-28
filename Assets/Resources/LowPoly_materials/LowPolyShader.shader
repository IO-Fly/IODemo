// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LowpolyShader"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
		_SpecularColor("Specular Color",Color) = (0.1,0.1,1,1)
		_SpecualrStrength("Specular Strength",float) = 1.0
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"

			sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _SpecularColor;
		float _SpecualrStrength;

		struct Input
		{
			float4 position : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct Out
		{
			float4 pos : SV_POSITION;
			float2 uv : Texcoord0;
			float3 normal : NORMAL;
		};

		Out vert(Input i)
		{
			Out o;
			// 转化屏幕坐标系位置
			o.pos = UnityObjectToClipPos(i.position);
			// 将本地坐标系法向量转化为世界坐标系方向量
			o.normal = mul(float4(i.normal,1),unity_WorldToObject).xyz;
			o.uv = i.uv;
			return o;
		}

		fixed4 frag(Out o) : COLOR
		{
			// 法向量标准化
			float3 normal = normalize(o.normal);
			// 获取平行光源方向并标准化
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			// 获取贴图纹理 这步可有可无，取决于你是否贴贴图
			float3 texColor = tex2D(_MainTex,o.uv);
			// 计算漫反射
			fixed3 diffuseColor = texColor * _Color * max(0,dot(normal,lightDir)) * _LightColor0.rgb;
			// 计算边缘光
			float spe = 1 - max(0,dot(normal,lightDir));
			fixed3 speColor = _SpecularColor.rgb * pow(spe,_SpecualrStrength);
			// 混合输出
			return fixed4(diffuseColor + speColor,1.0);
		}

			ENDCG
	}
	}
		FallBack "Diffuse"
}