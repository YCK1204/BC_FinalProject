Shader "Custom/GlitchShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.5
        _ScanLineJitter ("Scan Line Jitter", Range(0, 1)) = 0.5
        _VerticalJump ("Vertical Jump", Range(0, 1)) = 0.3
        _HorizontalShake ("Horizontal Shake", Range(0, 1)) = 0.2
        _ColorDrift ("Color Drift", Range(0, 1)) = 0.4
        _DigitalNoise ("Digital Noise", Range(0, 1)) = 0.6
        _Speed ("Animation Speed", Range(0, 10)) = 1.0
        
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
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _GlitchIntensity;
            float _ScanLineJitter;
            float _VerticalJump;
            float _HorizontalShake;
            float _ColorDrift;
            float _DigitalNoise;
            float _Speed;

            float random(float2 st) 
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            float noise(float2 st) 
            {
                float2 i = floor(st);
                float2 f = frac(st);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;
                float2 uv = i.uv;
                
                if(_GlitchIntensity <= 0.01)
                {
                    return tex2D(_MainTex, uv) * i.color;
                }
                
                float2 originalUV = uv;
                
                float jumpNoise = noise(float2(0, time * 3.0));
                if(jumpNoise > 0.8)
                {
                    uv.y += sin(time * 10.0) * _VerticalJump * _GlitchIntensity * 0.1;
                }
                
                float shakeNoise = noise(float2(time * 5.0, uv.y * 10.0));
                uv.x += (shakeNoise - 0.5) * _HorizontalShake * _GlitchIntensity * 0.1;
                
                float jitter = noise(float2(0, uv.y * 50.0 + time * 2.0));
                uv.x += (jitter - 0.5) * _ScanLineJitter * _GlitchIntensity * 0.05;
                
                if(uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                {
                    uv = originalUV;
                }
                
                fixed4 col = tex2D(_MainTex, uv) * i.color;
                
                if(_ColorDrift > 0.01)
                {
                    float driftOffset = _ColorDrift * _GlitchIntensity * 0.02;
                    
                    float2 rUV = uv + float2(driftOffset, 0);
                    float2 bUV = uv - float2(driftOffset, 0);
                    
                    if(rUV.x < 0.0 || rUV.x > 1.0) rUV = uv;
                    if(bUV.x < 0.0 || bUV.x > 1.0) bUV = uv;
                    
                    float r = tex2D(_MainTex, rUV).r;
                    float g = col.g;
                    float b = tex2D(_MainTex, bUV).b;
                    col.rgb = float3(r, g, b);
                }
                
                float scanline = sin(uv.y * 800.0 + time * 10.0);
                scanline = pow(abs(scanline), 10.0) * 0.1 * _GlitchIntensity;
                col.rgb += scanline;
                
                if(_DigitalNoise > 0.01)
                {
                    float2 noiseUV = floor(uv * 50.0 + time * 2.0) / 50.0;
                    float dNoise = random(noiseUV);
                    col.rgb = lerp(col.rgb, float3(dNoise, dNoise, dNoise), 
                                   _DigitalNoise * _GlitchIntensity * 0.3);
                }
                
                float2 blockUV = floor(uv * 20.0 + time * 0.5) / 20.0;
                float blockNoise = random(blockUV);
                if(blockNoise > 0.95 && _GlitchIntensity > 0.3)
                {
                    col.rgb = lerp(col.rgb, float3(1, 0, 1), 0.5);
                }
                
                float flash = noise(float2(time * 7.0, 0));
                if(flash > 0.9)
                {
                    col.rgb += 0.3 * _GlitchIntensity;
                }
                else if(flash < 0.1)
                {
                    col.rgb *= (1.0 - _GlitchIntensity * 0.5);
                }
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Sprites/Default"
}