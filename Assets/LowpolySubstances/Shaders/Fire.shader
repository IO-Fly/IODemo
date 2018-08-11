// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "LowpolySubstances/Fire" {

Properties {
	_PrimaryColor("Primary color", Color) = (1,1,1,1)
	_SecondaryColor("Secondary color", Color) = (1,1,1,1)
    
    [Space]
    _SecondaryColorAngle("Secondary color angle", Range(0, 2)) = 1.0
    _SecondaryColorImpact("Secondary color impact", Range(0, 2)) = 1.0
    
    [Space]
    _Alpha("Opacity", Range(0, 1)) = 0.8
    
    [Space]
    _Height1("Flame vertex height", Float) = 3
    _IntensityV1("Intensity vertical", Range(0, 15)) = 1
    _SpeedV1("Speed vertical", Range(0, 10)) = 1
    _IntensityV1Variance2("Coherence", Range(0, 5)) = 1
    _IntensityV1Variance1("Distance dependency", Range(0, 5)) = 1
    _IntensityHShift("Horizontal shift", Range(0, 10)) = 1
    _IntensityHScale("Horizontal scale", Range(0, 10)) = 1
    _SpeedH1("Speed horizontal", Range(0, 8)) = 1
    [MaterialToggle] _MoveInwards("Move inwards", Float) = 0
    
    [Space]
    [MaterialToggle] _EnableParticles("Enable particles", Float) = 1
    _Height2("Particles vertex height", Float) = 10
    _ParticlesAmplitudeV("Vertical amplitude", Range(0, 10)) = 5
    _ParticlesRadius("Horizontal amplitude", Range(0, 15)) = 8
    _ParticlesSpeed("Particles speed", Range(0, 10)) = 5
    _ParticlesDelay("Particles delay", Range(1, 10)) = 5
    
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }

	AlphaTest Off
	Blend One OneMinusSrcAlpha
	Cull Back
	Fog { Mode Off }
	Lighting Off
	ZTest LEqual
	ZWrite On
	
	SubShader {
		Pass {
			CGPROGRAM
			
            #pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fog
            
			#include "UnityCG.cginc"

			struct appdata {
			    half4 position : POSITION;
			    half2 texcoord : TEXCOORD0;
			    half3 normal : NORMAL;
			};

			struct v2f {
			    half4 position : SV_POSITION;
			    half4 color : TEXCOORD0;
                UNITY_FOG_COORDS(1)
			};

			half4 _PrimaryColor;
			half4 _SecondaryColor;
            
            half _SecondaryColorImpact;
            half _SecondaryColorAngle;
            
            half _Alpha;

            half _Height1;
            half _IntensityV1;
            half _IntensityV1Variance1;
            half _IntensityV1Variance2;
            half _IntensityHShift;
            half _IntensityHScale;
            half _SpeedV1;
            half _SpeedH1;
            
            half _EnableParticles;
            half _Height2;
            half _ParticlesAmplitudeV;
            half _ParticlesSpeed;
            int _ParticlesDelay;
            half _ParticlesRadius;
            half _MoveInwards;
            
			v2f vert(appdata vertex) {
			    v2f output;
                
                if (_EnableParticles && vertex.position.y > _Height2) {  // Particles
                    if (vertex.position.x >= 0 && vertex.position.z >= 0) {  // XZ quadrant 1
                        // Pause
                        vertex.position.xz *= saturate(-(_ParticlesDelay - 1) + fmod(_Time.y * _ParticlesSpeed * 0.4, _ParticlesDelay));
                        // Move up
                        vertex.position.y += -_Height2 + _Height1 + fmod(_Time.y * _ParticlesSpeed * 0.4, 1) * _ParticlesAmplitudeV * 0.125;
                        // Move sideways and scale to 0
                        vertex.position.xz *= (1 - fmod(_Time.y * _ParticlesSpeed * 0.4, 1)) * _ParticlesRadius;
                    } else if (vertex.position.x < 0 && vertex.position.z >= 0) {  // XZ quadrant 2
                        // Pause
                        vertex.position.xz *= saturate(-(_ParticlesDelay - 1) + fmod((_Time.y + 100) * _ParticlesSpeed * 0.4 + 3, _ParticlesDelay));
                        // Move up
                        vertex.position.y += -_Height2 + _Height1 + fmod((_Time.y + 100) * _ParticlesSpeed * 0.4, 1) * _ParticlesAmplitudeV * 0.125;
                        // Move sideways and scale to 0
                        vertex.position.xz *= (1 - fmod((_Time.y + 100) * _ParticlesSpeed * 0.4 + 2, 1)) * _ParticlesRadius;
                    } else if (vertex.position.x >= 0 && vertex.position.z < 0) {  // XZ quadrant 3
                        // Pause
                        vertex.position.xz *= saturate(-(_ParticlesDelay - 1) + fmod(_Time.y * _ParticlesSpeed * 0.4 + 2, _ParticlesDelay));
                        // Move up
                        vertex.position.y += -_Height2 + _Height1 + fmod(_Time.y * _ParticlesSpeed * 0.4, 1) * _ParticlesAmplitudeV * 0.125;
                        // Move sideways and scale to 0
                        vertex.position.xz *= (1 - fmod(_Time.y * _ParticlesSpeed * 0.4, 1)) * _ParticlesRadius;
                        
                    } else {  // XZ quadrant 4
                        // Pause
                        vertex.position.xz *= saturate(-(_ParticlesDelay - 1) + fmod(_Time.y * _ParticlesSpeed * 0.4 + 1, _ParticlesDelay));
                        // Move up
                        vertex.position.y += -_Height2 + _Height1 + fmod(_Time.y * _ParticlesSpeed * 0.4, 1) * _ParticlesAmplitudeV * 0.125;
                        // Move sideways and scale to 0
                        vertex.position.xz *= (1 - fmod(_Time.y * _ParticlesSpeed * 0.4, 1)) * _ParticlesRadius;
                        
                    }
                } else if (vertex.position.y > _Height1) { // Flames
                    // Vertices closer to center should move further
                    half hDistance = sqrt(length(vertex.position.xz) * 20 * _IntensityV1Variance1 + 0.3);
                    // Move up
                    half mod = fmod(_SpeedV1 * _Time.y + (_MoveInwards? length(vertex.position.xz) : vertex.position.x) * 20 * _IntensityV1Variance2, 3.1415 * 0.5);
			    	vertex.position.y += sin(mod) / hDistance * _IntensityV1 * 0.05;
                    // Horizontal shift
                    vertex.position.xz += sin(_SpeedH1 * _Time.y) * _IntensityHShift * 0.01;
                    // Horizontal scale
                    vertex.position.xz *= 1 + sin(_SpeedH1 * 2 * _Time.y + fmod(vertex.position.z, 1) * 15) * _IntensityHScale * 0.05;
                }
                
			    half3 worldPosition = mul(unity_ObjectToWorld, vertex.position).xyz;
			    half3 cameraVector = normalize(worldPosition.xyz - _WorldSpaceCameraPos);
			    half3 worldNormal = normalize(mul(unity_ObjectToWorld, half4(vertex.normal, 0)).xyz);
			    half blend = dot(worldNormal, cameraVector) * _SecondaryColorAngle + _SecondaryColorImpact;

			    output.color = (1 - blend) * _PrimaryColor + blend * _SecondaryColor;
                output.color.a = _Alpha;
			    output.color.rgb *= output.color.a;
                
                output.position = UnityObjectToClipPos(vertex.position);
                
                UNITY_TRANSFER_FOG(output, output.position);

			    return output;
			}
			
			half4 frag(v2f fragment) : COLOR {
                UNITY_APPLY_FOG(fragment.fogCoord, fragment.color);
				return fragment.color;
			}
            
			ENDCG
		}
	}
	
}
}
