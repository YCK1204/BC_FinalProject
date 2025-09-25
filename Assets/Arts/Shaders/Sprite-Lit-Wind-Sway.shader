Shader "Universal Render Pipeline/2D/Sprite-Lit-Wind"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        [MaterialToggle] _ZWrite("ZWrite", Float) = 0
        
        // 바람 효과 속성
        [Header(Wind Effects)]
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.3
        _WindSpeed ("Wind Speed", Range(0, 10)) = 2.0
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0, 0)
        
        // 디테일한 움직임
        _SwayFrequency ("Sway Frequency", Range(0, 10)) = 1.5
        _SwayAmount ("Sway Amount", Range(0, 0.1)) = 0.02
        
        // 세밀한 흔들림 (나뭇잎 등)
        _DetailFreq ("Detail Frequency", Range(0, 20)) = 8.0
        _DetailAmount ("Detail Amount", Range(0, 0.05)) = 0.01
        
        // 높이에 따른 영향 (위쪽이 더 많이 흔들림)
        _HeightInfluence ("Height Influence", Range(0, 2)) = 1.0

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                half2   lightingUV  : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                // 바람 효과 변수들
                float _WindStrength;
                float _WindSpeed;
                float4 _WindDirection;
                float _SwayFrequency;
                float _SwayAmount;
                float _DetailFreq;
                float _DetailAmount;
                float _HeightInfluence;
            CBUFFER_END

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            // 바람 함수들
            float windNoise(float2 pos, float time, float frequency)
            {
                return sin(pos.x * frequency + time) * cos(pos.y * frequency * 0.7 + time * 0.8);
            }
            
            float2 calculateWindOffset(float3 worldPos, float2 uv)
            {
                float time = _Time.y * _WindSpeed;
                
                // 높이에 따른 영향 계산 (UV.y가 높을수록 더 많이 흔들림)
                float heightFactor = pow(uv.y, _HeightInfluence);
                
                // 메인 바람 효과
                float2 windOffset = _WindDirection.xy * _WindStrength * heightFactor;
                windOffset *= sin(time + worldPos.x * 0.5) * cos(time * 0.7 + worldPos.z * 0.3);
                
                // 주기적인 흔들림
                float swayX = sin(time * _SwayFrequency + worldPos.x) * _SwayAmount * heightFactor;
                float swayY = cos(time * _SwayFrequency * 0.8 + worldPos.z) * _SwayAmount * 0.5 * heightFactor;
                
                // 세밀한 디테일 움직임
                float detailX = windNoise(worldPos.xz * 2.0, time, _DetailFreq) * _DetailAmount * heightFactor;
                float detailY = windNoise(worldPos.xz * 1.5 + float2(10, 20), time * 1.3, _DetailFreq) * _DetailAmount * 0.3 * heightFactor;
                
                return windOffset + float2(swayX + detailX, swayY + detailY);
            }

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(v);

                SetUpSpriteInstanceProperties();
                
                // 월드 좌표 계산 (바람 효과를 위해)
                float3 worldPos = TransformObjectToWorld(v.positionOS);
                
                // 바람 오프셋 계산 및 적용
                float2 windOffset = calculateWindOffset(worldPos, v.uv);
                v.positionOS.xy += windOffset;
                
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                
                o.uv = v.uv;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * unity_SpriteColor;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                const half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, i.positionWS, i.positionCS, _MainTex);

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float3 normal       : NORMAL;
                float4 tangent      : TANGENT;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                half4   color           : COLOR;
                float2  uv              : TEXCOORD0;
                half3   normalWS        : TEXCOORD1;
                half3   tangentWS       : TEXCOORD2;
                half3   bitangentWS     : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START( UnityPerMaterial )
                half4 _Color;
                // 바람 효과 변수들
                float _WindStrength;
                float _WindSpeed;
                float4 _WindDirection;
                float _SwayFrequency;
                float _SwayAmount;
                float _DetailFreq;
                float _DetailAmount;
                float _HeightInfluence;
            CBUFFER_END

            // 바람 함수들
            float windNoise(float2 pos, float time, float frequency)
            {
                return sin(pos.x * frequency + time) * cos(pos.y * frequency * 0.7 + time * 0.8);
            }
            
            float2 calculateWindOffset(float3 worldPos, float2 uv)
            {
                float time = _Time.y * _WindSpeed;
                
                // 높이에 따른 영향 계산 (UV.y가 높을수록 더 많이 흔들림)
                float heightFactor = pow(uv.y, _HeightInfluence);
                
                // 메인 바람 효과
                float2 windOffset = _WindDirection.xy * _WindStrength * heightFactor;
                windOffset *= sin(time + worldPos.x * 0.5) * cos(time * 0.7 + worldPos.z * 0.3);
                
                // 주기적인 흔들림
                float swayX = sin(time * _SwayFrequency + worldPos.x) * _SwayAmount * heightFactor;
                float swayY = cos(time * _SwayFrequency * 0.8 + worldPos.z) * _SwayAmount * 0.5 * heightFactor;
                
                // 세밀한 디테일 움직임
                float detailX = windNoise(worldPos.xz * 2.0, time, _DetailFreq) * _DetailAmount * heightFactor;
                float detailY = windNoise(worldPos.xz * 1.5 + float2(10, 20), time * 1.3, _DetailFreq) * _DetailAmount * 0.3 * heightFactor;
                
                return windOffset + float2(swayX + detailX, swayY + detailY);
            }

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(attributes);

                SetUpSpriteInstanceProperties();
                
                // 월드 좌표 계산 (바람 효과를 위해)
                float3 worldPos = TransformObjectToWorld(attributes.positionOS);
                
                // 바람 오프셋 계산 및 적용
                float2 windOffset = calculateWindOffset(worldPos, attributes.uv);
                attributes.positionOS.xy += windOffset;
                
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = attributes.uv;
                o.color = attributes.color * _Color * unity_SpriteColor;
                o.normalWS = TransformObjectToWorldDir(attributes.normal);
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START( UnityPerMaterial )
                half4 _Color;
                // 바람 효과 변수들
                float _WindStrength;
                float _WindSpeed;
                float4 _WindDirection;
                float _SwayFrequency;
                float _SwayAmount;
                float _DetailFreq;
                float _DetailAmount;
                float _HeightInfluence;
            CBUFFER_END

            // 바람 함수들
            float windNoise(float2 pos, float time, float frequency)
            {
                return sin(pos.x * frequency + time) * cos(pos.y * frequency * 0.7 + time * 0.8);
            }
            
            float2 calculateWindOffset(float3 worldPos, float2 uv)
            {
                float time = _Time.y * _WindSpeed;
                
                // 높이에 따른 영향 계산 (UV.y가 높을수록 더 많이 흔들림)
                float heightFactor = pow(uv.y, _HeightInfluence);
                
                // 메인 바람 효과
                float2 windOffset = _WindDirection.xy * _WindStrength * heightFactor;
                windOffset *= sin(time + worldPos.x * 0.5) * cos(time * 0.7 + worldPos.z * 0.3);
                
                // 주기적인 흔들림
                float swayX = sin(time * _SwayFrequency + worldPos.x) * _SwayAmount * heightFactor;
                float swayY = cos(time * _SwayFrequency * 0.8 + worldPos.z) * _SwayAmount * 0.5 * heightFactor;
                
                // 세밀한 디테일 움직임
                float detailX = windNoise(worldPos.xz * 2.0, time, _DetailFreq) * _DetailAmount * heightFactor;
                float detailY = windNoise(worldPos.xz * 1.5 + float2(10, 20), time * 1.3, _DetailFreq) * _DetailAmount * 0.3 * heightFactor;
                
                return windOffset + float2(swayX + detailX, swayY + detailY);
            }

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(attributes);

                SetUpSpriteInstanceProperties();
                
                // 월드 좌표 계산 (바람 효과를 위해)
                float3 worldPos = TransformObjectToWorld(attributes.positionOS);
                
                // 바람 오프셋 계산 및 적용
                float2 windOffset = calculateWindOffset(worldPos, attributes.uv);
                attributes.positionOS.xy += windOffset;
                
                attributes.positionOS = UnityFlipSprite( attributes.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
                #endif
                
                o.uv = attributes.uv;
                o.color = attributes.color * _Color * unity_SpriteColor;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, i.positionWS, i.positionCS, _MainTex);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }
    }
}