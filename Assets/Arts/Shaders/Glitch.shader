Shader "Universal Render Pipeline/2D/Glitch-Effect"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlitchAmount("Glitch Amount", Range(0, 1)) = 0.5
        _GlitchTex("Glitch Noise Texture", 2D) = "white" {}
        _GlitchColor1("Glitch Color 1 (Red)", Color) = (1, 0, 0, 1)
        _GlitchColor2("Glitch Color 2 (Green)", Color) = (0, 1, 0, 1)
        _GlitchColor3("Glitch Color 3 (Blue)", Color) = (0, 0, 1, 1)
        _GlitchCutAmountX("Glitch Cut Amount X", Range(0.1, 10)) = 1
        _GlitchCutAmountY("Glitch Cut Amount Y", Range(0.1, 10)) = 1
        _OffsetStrength("Offset Strength", Range(0, 0.5)) = 0.1
        
        // Standard sprite properties
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "GlitchEffect"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GlitchTex);
            SAMPLER(sampler_GlitchTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _GlitchAmount;
                half4 _GlitchColor1;
                half4 _GlitchColor2;
                half4 _GlitchColor3;
                half _GlitchCutAmountX;
                half _GlitchCutAmountY;
                half _OffsetStrength;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 기본 텍스처 샘플
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // 글리치 효과가 꺼져있으면 원본 반환
                if (_GlitchAmount <= 0.001)
                {
                    return mainColor * input.color;
                }
                
                // 시간 기반 애니메이션
                float time = _Time.y;
                
                // 글리치 노이즈 UV 계산
                float2 glitchUV = float2(
                    input.uv.x * _GlitchCutAmountX + time * 10.0,
                    input.uv.y * _GlitchCutAmountY + sin(time * 10.0) * 0.1
                );
                
                // 글리치 노이즈 샘플링
                half glitchNoise = SAMPLE_TEXTURE2D(_GlitchTex, sampler_GlitchTex, glitchUV).r;
                
                // UV 오프셋 계산
                float2 offset = (glitchNoise - 0.5) * _GlitchAmount * _OffsetStrength;
                
                // RGB 채널 분리 효과
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(offset.x, 0)).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-offset.x * 0.5, 0)).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, offset.y)).b;
                
                // 글리치 컬러 적용
                float glitchIntensity = _GlitchAmount;
                
                half3 glitchR = r * lerp(half3(1, 0, 0), _GlitchColor1.rgb, glitchIntensity);
                half3 glitchG = g * lerp(half3(0, 1, 0), _GlitchColor2.rgb, glitchIntensity);
                half3 glitchB = b * lerp(half3(0, 0, 1), _GlitchColor3.rgb, glitchIntensity);
                
                // 최종 컬러 합성
                half3 finalColor = glitchR + glitchG + glitchB;
                
                // 원본과 글리치 효과 블렌딩
                finalColor = lerp(mainColor.rgb, finalColor, _GlitchAmount);
                
                return half4(finalColor, mainColor.a) * input.color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}