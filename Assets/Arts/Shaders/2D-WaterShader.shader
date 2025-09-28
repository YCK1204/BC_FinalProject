Shader "Custom/2D Water Shader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0.4, 0.7, 1.0, 0.7)
        _WaveSpeed ("Wave Speed", Range(0.1, 5.0)) = 2.0
        _WaveStrength ("Wave Strength", Range(0.001, 0.1)) = 0.02
        _WaveFrequency ("Wave Frequency", Range(1.0, 20.0)) = 8.0
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.7
        _Refraction ("Refraction Strength", Range(0.0, 0.1)) = 0.03
        _FlowDirection ("Flow Direction", Vector) = (1, 0.5, 0, 0)
        _FlowSpeed ("Flow Speed", Range(0.0, 2.0)) = 1.0
        _Distortion ("Distortion Texture", 2D) = "bump" {}
        _DistortionStrength ("Distortion Strength", Range(0.0, 0.2)) = 0.05
        _Fresnel ("Fresnel Power", Range(0.1, 5.0)) = 1.5
        _ReflectionColor ("Reflection Color", Color) = (1, 1, 1, 0.3)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 screenPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float fogCoord : TEXCOORD3;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Distortion);
            SAMPLER(sampler_Distortion);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _WaveSpeed;
                float _WaveStrength;
                float _WaveFrequency;
                float _Transparency;
                float _Refraction;
                float4 _FlowDirection;
                float _FlowSpeed;
                float4 _Distortion_ST;
                float _DistortionStrength;
                float _Fresnel;
                float4 _ReflectionColor;
            CBUFFER_END
            
            // 노이즈 함수
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // FBM (Fractional Brownian Motion)
            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for(int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(p * frequency);
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }
                return value;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                
                // 버텍스 애니메이션 제거 - 타일맵에서 틈이 생기지 않도록
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.screenPos = ComputeScreenPos(o.vertex);
                o.fogCoord = ComputeFogFactor(o.vertex.z);
                
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;
                
                // 월드 좌표 기반으로 연속적인 효과 생성
                float2 worldUV = i.worldPos.xy;
                
                // 흐름 효과를 위한 UV 오프셋
                float2 flowOffset = _FlowDirection.xy * time * _FlowSpeed;
                
                // 디스토션 효과 (월드 좌표 기반)
                float2 distortionUV = worldUV * 0.5 + flowOffset * 0.1;
                float4 distortion = SAMPLE_TEXTURE2D(_Distortion, sampler_Distortion, distortionUV * _Distortion_ST.xy + _Distortion_ST.zw);
                float2 distortionOffset = (distortion.xy * 2.0 - 1.0) * _DistortionStrength;
                
                // 복합적인 물결 효과 (월드 좌표 기반으로 타일 간 연속성 보장)
                float2 waveUV = i.uv;
                float waveTime = time * _WaveSpeed;
                
                // 월드 좌표 기반 웨이브로 타일 간 연속성 확보
                float wave1 = sin(worldUV.x * _WaveFrequency + waveTime);
                float wave2 = cos(worldUV.y * _WaveFrequency * 0.7 + waveTime * 1.3);
                float wave3 = sin((worldUV.x + worldUV.y) * _WaveFrequency * 0.5 + waveTime * 0.8);
                
                waveUV.x += wave1 * _WaveStrength;
                waveUV.y += wave2 * _WaveStrength * 0.7;
                waveUV += wave3 * _WaveStrength * 0.3;
                
                // FBM을 이용한 자연스러운 노이즈 (월드 좌표 기반)
                float2 noiseUV = worldUV * 0.3 + flowOffset;
                float noiseValue = fbm(noiseUV + time * 0.2);
                waveUV += (noiseValue - 0.5) * 0.02;
                
                // 최종 UV에 디스토션 적용
                float2 finalUV = waveUV + distortionOffset;
                
                // 텍스처 샘플링
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, finalUV);
                
                // 기본 색상 적용
                col *= _Color * i.color;
                
                // 프레넬 효과 (가장자리가 더 투명)
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float2 centerDist = abs(screenUV - 0.5) * 2.0;
                float fresnelBase = saturate(1.0 - max(centerDist.x, centerDist.y));
                float fresnel = pow(fresnelBase, _Fresnel);
                
                // 반사 효과 (월드 좌표 기반)
                float reflection = noise(worldUV * 2.0 + time * 2.0) * 0.3 + 0.7;
                col.rgb = lerp(col.rgb, _ReflectionColor.rgb, _ReflectionColor.a * reflection * fresnel);
                
                // 깊이감을 위한 그라데이션
                float depth = 1.0 - (i.uv.y * 0.3);
                col.rgb *= depth;
                
                // 투명도 적용
                col.a *= _Transparency;
                
                // 포그 적용
                col.rgb = MixFog(col.rgb, i.fogCoord);
                
                return col;
            }
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}