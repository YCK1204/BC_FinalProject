Shader "UI/SmartphoneScreenGlass"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Gradient
        _GradientTop ("Gradient Top Color", Color) = (0.1, 0.1, 0.2, 1)
        _GradientBottom ("Gradient Bottom Color", Color) = (0.05, 0.05, 0.15, 1)
        _GradientIntensity ("Gradient Intensity", Range(0, 1)) = 0.3
        
        // Glass Effect
        _GlassBlur ("Glass Blur", Range(0, 10)) = 3
        _GlassAlpha ("Glass Alpha", Range(0, 1)) = 0.85
        _Frosting ("Frosting", Range(0, 1)) = 0.2
        
        // Glow
        _GlowColor ("Glow Color", Color) = (0.3, 0.6, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 2)) = 0.5
        _GlowSize ("Glow Size", Range(0, 0.1)) = 0.02
        
        // Reflection
        _ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.15
        
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
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _GradientTop;
            fixed4 _GradientBottom;
            float _GradientIntensity;
            float _GlassBlur;
            float _GlassAlpha;
            float _Frosting;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowSize;
            float _ReflectionIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            // Noise function for frosted glass effect
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

            fixed4 frag(v2f i) : SV_Target
            {
                // Base texture
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                
                // Gradient overlay
                float gradientMask = i.texcoord.y;
                fixed4 gradient = lerp(_GradientBottom, _GradientTop, gradientMask);
                
                // Frosted glass noise
                float2 noiseUV = i.texcoord * 50.0;
                float noiseVal = noise(noiseUV) * _Frosting;
                
                // Edge glow effect
                float2 center = i.texcoord - 0.5;
                float dist = length(center);
                float edgeDist = min(i.texcoord.x, min(i.texcoord.y, min(1.0 - i.texcoord.x, 1.0 - i.texcoord.y)));
                float glow = smoothstep(_GlowSize, 0.0, edgeDist) * _GlowIntensity;
                
                // Reflection effect (subtle diagonal highlight)
                float2 reflectUV = i.texcoord * 2.0 - 1.0;
                float reflection = max(0, 1.0 - length(reflectUV - float2(-0.3, 0.3))) * _ReflectionIntensity;
                reflection = pow(reflection, 3.0);
                
                // Combine effects
                fixed4 finalColor = texColor * i.color;
                finalColor.rgb = lerp(finalColor.rgb, gradient.rgb, _GradientIntensity);
                finalColor.rgb += noiseVal;
                finalColor.rgb += _GlowColor.rgb * glow;
                finalColor.rgb += reflection;
                
                // Apply glass alpha
                finalColor.a *= _GlassAlpha;
                
                return finalColor;
            }
            ENDCG
        }
    }
}