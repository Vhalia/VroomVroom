Shader "Custom/TwoLaneRoad"
{
    Properties
    {
        _RoadColor ("Road Color", Color) = (0.25, 0.25, 0.25, 1)
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _LineWidth ("Line Width", Range(0, 0.3)) = 0.01
        _DashLength ("Dash Length", Range(0.01, 0.1)) = 0.05
        _DashGap ("Dash Gap", Range(0.01, 0.1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _RoadColor;
            fixed4 _LineColor;
            float _LineWidth;
            float _DashLength;
            float _DashGap;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = _RoadColor;
                float2 uv = i.uv - 0.5;

                // Center dashed line
                if (abs(uv.x) < _LineWidth) {
                    float dashPattern = fmod(abs(uv.y + 0.5), _DashLength + _DashGap);
                    if (dashPattern < _DashLength) {
                        col = _LineColor;
                    }
                }

                // Left solid line
                if (abs(uv.x + 0.45) < _LineWidth) {
                    col = _LineColor;
                }

                // Right solid line
                if (abs(uv.x - 0.45) < _LineWidth) {
                    col = _LineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}