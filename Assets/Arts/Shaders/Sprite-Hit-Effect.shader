Shader "Universal Render Pipeline/2D/Sprite-Hit-Effect"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        [MaterialToggle] _ZWrite("ZWrite", Float) = 0
        
        // Hit Effect Properties
        [Header(Hit Effect)]
        _HitIntensity("Hit Intensity", Range(0, 1)) = 0
        _HitColor("Hit Color", Color) = (1, 0.2, 0.2, 1)
        _FlashIntensity("Flash Intensity", Range(0, 3)) = 2
        _ShakeAmount("Shake Amount", Range(0, 0.1)) = 0.02
        _ShakeSpeed("Shake Speed", Range(0, 50)) = 20
        _PulseSpeed("Pulse Speed", Range(0, 10)) = 5
        
        [Header(Damage Visualization)]
        _DamageFlash("Damage Flash", Range(0, 1)) = 0
        _DamageColor("Damage Color", Color) = (1, 1, 1, 1)
        _CrackIntensity("Crack Effect", Range(0, 1)) = 0
        _CrackTex("Crack Texture", 2D) = "black" {}
        
        [Header(Dissolve Effect)]
        _DissolveAmount("Dissolve Amount", Range(0, 1)) = 0
        _DissolveColor("Dissolve Color", Color) = (1, 0.5, 0, 1)
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0.01, 0.2)) = 0.05
        _DissolveTex("Dissolve Texture", 2D) = "white" {}
        
        [Header(Stun Effect)]
        _StunIntensity("Stun Intensity", Range(0, 1)) = 0
        _StunColor("Stun Color", Color) = (1, 1, 0, 1)
        _StunFrequency("Stun Frequency", Range(0, 20)) = 10
        
        // Legacy properties
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
                float2  worldPos    : TEXCOORD2;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD3;
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

            TEXTURE2D(_CrackTex);
            SAMPLER(sampler_CrackTex);

            TEXTURE2D(_DissolveTex);
            SAMPLER(sampler_DissolveTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _HitIntensity;
                half4 _HitColor;
                half _FlashIntensity;
                half _ShakeAmount;
                half _ShakeSpeed;
                half _PulseSpeed;
                half _DamageFlash;
                half4 _DamageColor;
                half _CrackIntensity;
                half _DissolveAmount;
                half4 _DissolveColor;
                half _DissolveEdgeWidth;
                half _StunIntensity;
                half4 _StunColor;
                half _StunFrequency;
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

            // Utility functions
            float random(float2 st) {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float noise(float2 st) {
                float2 i = floor(st);
                float2 f = frac(st);
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            // Hit shake effect
            float2 getHitShake(float2 uv) {
                float time = _Time.y * _ShakeSpeed;
                float2 shake = float2(
                    sin(time * 2.1 + uv.y * 10) * cos(time * 1.7),
                    cos(time * 1.9 + uv.x * 8) * sin(time * 2.3)
                );
                return shake * _ShakeAmount * _HitIntensity;
            }

            // Pulse effect
            float getPulse() {
                return (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) * _HitIntensity;
            }

            // Stun effect
            float getStunEffect(float2 uv) {
                float time = _Time.y * _StunFrequency;
                float stun = step(0.7, sin(time + uv.x * 20) * sin(time * 1.3 + uv.y * 15));
                return stun * _StunIntensity;
            }

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(v);

                SetUpSpriteInstanceProperties();
                
                // Apply hit shake to vertex position
                float2 shakeOffset = getHitShake(v.uv);
                v.positionOS.xy += shakeOffset;
                
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = v.uv;
                o.worldPos = TransformObjectToWorld(v.positionOS).xy;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * unity_SpriteColor;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Apply shake to UV
                uv += getHitShake(uv) * 0.5;
                
                // Sample main texture
                half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                // Dissolve effect
                if (_DissolveAmount > 0)
                {
                    float dissolveNoise = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, uv * 2).r;
                    float dissolve = dissolveNoise - _DissolveAmount;
                    
                    // Create dissolve edge
                    float edge = step(0, dissolve) - step(_DissolveEdgeWidth, dissolve);
                    main.rgb = lerp(main.rgb, _DissolveColor.rgb, edge * _DissolveColor.a);
                    
                    // Apply dissolve clipping
                    clip(dissolve);
                }
                
                // Hit flash effect
                float pulse = getPulse();
                float flashEffect = _HitIntensity * _FlashIntensity * pulse;
                main.rgb = lerp(main.rgb, _HitColor.rgb, flashEffect);
                
                // Damage flash (white flash)
                main.rgb = lerp(main.rgb, _DamageColor.rgb, _DamageFlash * _DamageColor.a);
                
                // Crack effect
                if (_CrackIntensity > 0)
                {
                    float4 crack = SAMPLE_TEXTURE2D(_CrackTex, sampler_CrackTex, uv * 3);
                    float crackMask = 1 - crack.r;
                    crackMask = pow(crackMask, 5) * _CrackIntensity;
                    main.rgb = lerp(main.rgb, float3(0.2, 0.2, 0.2), crackMask);
                }
                
                // Stun effect (electric-like overlay)
                float stunEffect = getStunEffect(uv);
                main.rgb = lerp(main.rgb, _StunColor.rgb, stunEffect * _StunColor.a);
                
                // Add extra glow during hit
                main.rgb += _HitColor.rgb * _HitIntensity * 0.3;
                
                // Apply color intensity boost during effects
                float totalEffectIntensity = _HitIntensity + _DamageFlash + _StunIntensity;
                main.rgb *= (1 + totalEffectIntensity * 0.5);
                
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

            CBUFFER_START( UnityPerMaterial )
                half4 _Color;
            CBUFFER_END

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(attributes);

                SetUpSpriteInstanceProperties();
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

            CBUFFER_START( UnityPerMaterial )
                half4 _Color;
            CBUFFER_END

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(attributes);

                SetUpSpriteInstanceProperties();
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