// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

Shader "Magic Leap/Depth Camera"
{
    Properties
    {
        [KeywordEnum(Depth, Confidence)] _Buffer("Buffer", int) = 0
        _MinDepth("Min Depth", Float) = 0
        _MaxDepth("Max Depth", Float) = 0
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        _MapTex("Frame Data Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MapTex;
            float4 _MainTex_ST;
            float _MinDepth;
            float _MaxDepth;
            int _Buffer;

            float InverseLerp(float v, float min, float max)
            {
                return clamp((v - min) / (max - min), 0.0, 1.0);
            }

            float Normalize(float v, float min, float end)
            {
                return InverseLerp(v, min, end);
            }

            float NormalizeDepth(float depth_meters)
            {
                return InverseLerp(depth_meters, _MinDepth, _MaxDepth);
            }

            float NormalizeConfidence(float confidence)
            {
                float conf = clamp(abs(confidence), 0.0, 0.5);
                return Normalize(conf, 0.0, 0.5);
            }

            fixed3 GetColorVisualization(float x) {
                return tex2D(_MapTex, fixed2(x, 0.5)).rgb;
            }

            float3 GetConfidenceColor(float conf)
            {
                return float3(conf, 1.0 - conf, 0.0);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_Buffer == 0)
                {
                    float depth_meters = tex2D(_MainTex, i.uv).r;
                    float normalized_depth = NormalizeDepth(depth_meters);
                    fixed4 depth_color = fixed4(GetColorVisualization(normalized_depth), 1.0);

                    // Values outside of range mapped to black.
                    if (depth_meters < _MinDepth || depth_meters > _MaxDepth)
                    {
                        depth_color.rgb *= 0.0;
                    }

                    return depth_color;
                }
                else
                {
                    float depth_meters = tex2D(_MainTex, i.uv).r;
                    float confidence = tex2D(_MapTex, i.uv).r;
                    float normalized_confidence = NormalizeConfidence(confidence);
                    float normalized_depth = NormalizeDepth(depth_meters);
                    fixed4 depth_color = fixed4(GetConfidenceColor(normalized_confidence), 1.0);
                    depth_color *= (1.0 - normalized_depth / 2.0);

                    return depth_color;
                }
            }
            ENDCG
        }
    }
}
