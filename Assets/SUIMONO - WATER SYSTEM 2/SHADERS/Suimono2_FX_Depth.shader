﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suimono2/SuimonoDepth" {
Properties {
   _MainTex ("", 2D) = "white" {}
}

SubShader {
Tags { "RenderType"="Opaque" }

Pass{
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "AutoLight.cginc"

sampler2D _MainTex;
sampler2D _CameraDepthTexture;

float _sceneDepth = 0;
float _shoreDepth = 0;
float _clipDepth = 0;
float _edgeDepth = 0;

struct v2f {
    float4 pos : SV_POSITION;
    half2 uv : TEXCOORD0;
    float4 screenPos: TEXCOORD1;
};


//Vertex
v2f vert (appdata_tan v){
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos=ComputeScreenPos(o.pos);
    o.uv = v.texcoord;
    TRANSFER_VERTEX_TO_FRAGMENT(o);

    return o;
}



half4 frag (v2f i) : COLOR{

	half4 retValue = fixed4(0,0,0,0);

	//get textures
	half4 origColor = tex2D(_MainTex, i.uv);

	//CALCULATE DEPTH
	#if UNITY_VERSION >= 550
		half rawDepth = 1-SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.screenPos.xyz);
	#else
		half rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.screenPos.xyz);
	#endif


	//DEPTH GENERATION
	_edgeDepth = 100;
	_clipDepth = 17;

	//set clip value
	retValue.b = 1-(saturate(lerp(-0.25,_clipDepth,rawDepth)*2));
	retValue.a = 0;//(1.0-saturate(lerp(0.05,_edgeDepth*1.01,rawDepth)))*50.0;

	//set depth values
	retValue.r = (1.0-lerp(0.0,_sceneDepth*0.5,rawDepth));
	retValue.g = saturate((1.0-lerp(0.0,_shoreDepth*0.5,rawDepth))*3.0);

	//remove clip values
	//retValue.r = retValue.r * (1-retValue.b);
	//retValue.g = retValue.g - (1-retValue.b);


	//retValue.r = 0;
	//retValue.g = 0;
	//retValue.b = 0;


	return retValue;
}


ENDCG
}
}
FallBack "Diffuse"
}