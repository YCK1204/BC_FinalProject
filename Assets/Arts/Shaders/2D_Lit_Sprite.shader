Shader "Custom/2D_Lit_Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Lighting)]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _EmissionMap ("Emission Map", 2D) = "black" {}
        
        [Header(Advanced)]
        _AmbientColor ("Ambient Color", Color) = (0.2, 0.2, 0.3, 1)
        _LightIntensity ("Light Intensity Multiplier", Range(0, 3)) = 1
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.5
        
        // UI Properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "LightMode"="Universal2D"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Sprite2DLit"
            Tags { "LightMode" = "Universal2D" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            // 2D Light keywords
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
            #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _NormalMap_ST;
                float4 _EmissionMap_ST;
                float _NormalStrength;
                float _Metallic;
                float _Smoothness;
                float4 _EmissionColor;
                float4 _AmbientColor;
                float _LightIntensity;
                float _ShadowStrength;
            CBUFFER_END

            half3 Calculate2DLighting(float3 positionWS, half3 normalWS, half3 albedo, half metallic, half smoothness)
            {
                half3 color = albedo * _AmbientColor.rgb;
                
                #if USE_SHAPE_LIGHT_TYPE_0 || USE_SHAPE_LIGHT_TYPE_1 || USE_SHAPE_LIGHT_TYPE_2 || USE_SHAPE_LIGHT_TYPE_3
                    float2 lightingUV = positionWS.xy;
                    half4 lightColor = half4(0, 0, 0, 1);
                    
                    #ifdef USE_SHAPE_LIGHT_TYPE_0
                        lightColor += SampleShapeLight0(lightingUV);
                    #endif
                    #ifdef USE_SHAPE_LIGHT_TYPE_1
                        lightColor += SampleShapeLight1(lightingUV);
                    #endif
                    #ifdef USE_SHAPE_LIGHT_TYPE_2
                        lightColor += SampleShapeLight2(lightingUV);
                    #endif
                    #ifdef USE_SHAPE_LIGHT_TYPE_3
                        lightColor += SampleShapeLight3(lightingUV);
                    #endif
                    
                    half3 lightDirection = normalize(half3(0, 0, 1)); 
                    half NdotL = max(0, dot(normalWS, lightDirection));
                    
                    half3 diffuse = albedo * lightColor.rgb * NdotL * _LightIntensity;
                    
                    half3 viewDir = half3(0, 0, 1);
                    half3 halfDir = normalize(lightDirection + viewDir);
                    half NdotH = max(0, dot(normalWS, halfDir));
                    half specularPower = exp2(10 * smoothness + 1);
                    half3 specular = lightColor.rgb * pow(NdotH, specularPower) * metallic * _LightIntensity;
                    
                    color += diffuse + specular;
                #endif
                
                return color;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 color = texColor * input.color;
                
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, 
                    TRANSFORM_TEX(input.uv, _NormalMap)), _NormalStrength);
                
                half3 normalWS = half3(normalTS.xy, normalTS.z);
                normalWS = normalize(normalWS);
                
                half3 litColor = Calculate2DLighting(input.positionWS, normalWS, 
                    color.rgb, _Metallic, _Smoothness);
                
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, 
                    TRANSFORM_TEX(input.uv, _EmissionMap)).rgb * _EmissionColor.rgb;
                
                color.rgb = litColor + emission;
                
                return color;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Sprite2DShadow"
            Tags { "LightMode" = "UniversalRenderer2DShadow" }
            
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                clip(color.a - 0.01);
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}