Shader "Custom/2D_Nature_Wind_Sway"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        // 바람 효과 속성
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
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
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
            float4 _Color;
            
            float _WindStrength;
            float _WindSpeed;
            float4 _WindDirection;
            float _SwayFrequency;
            float _SwayAmount;
            float _DetailFreq;
            float _DetailAmount;
            float _HeightInfluence;
            
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
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // 월드 좌표 계산
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                // 바람 오프셋 계산
                float2 windOffset = calculateWindOffset(worldPos, v.uv);
                
                // 정점 위치에 바람 효과 적용
                v.vertex.xy += windOffset;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;
                return col;
            }
            ENDCG
        }
    }
}