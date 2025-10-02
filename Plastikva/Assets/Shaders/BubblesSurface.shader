Shader "Custom/BubblesSurfaceWithTex"
{
    Properties
    {
        _Color ("Bubble Color", Color) = (0.5,0.8,1,1)
        _BubbleTex ("Bubble Texture", 2D) = "white" {}   // New texture property
        _BubbleCount ("Bubble Count", Range(1, 50)) = 20
        _Speed ("Speed", Range(0.1, 2.0)) = 0.5
        _Radius ("Bubble Radius", Range(0.01, 0.2)) = 0.08
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            sampler2D _BubbleTex;
            float4 _BubbleTex_ST; // For scaling/tiling
            int _BubbleCount;
            float _Speed;
            float _Radius;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash(float n) { return frac(sin(n) * 43758.5453); }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;

                float t = _Time.y * _Speed;
                float3 col = float3(0,0,0);

                for (int idx = 0; idx < _BubbleCount; idx++)
                {
                    float id = (float)idx;
                    float x = hash(id * 12.3) * 2.0 - 1.0;
                    float y = fmod(t * (0.2 + hash(id) * 0.5) + hash(id * 45.6), 2.0) - 1.0;
                    float r = _Radius + hash(id * 7.7) * 0.05;

                    float2 center = float2(x + sin(t * 0.5 + id) * 0.1, y);

                    float2 bubUV = (uv - center) / (r * 2.0) + 0.5;

                    if (bubUV.x >= 0 && bubUV.x <= 1 && bubUV.y >= 0 && bubUV.y <= 1)
                    {
                        fixed4 tex = tex2D(_BubbleTex, bubUV * _BubbleTex_ST.xy + _BubbleTex_ST.zw);
                        col += tex.rgb * _Color.rgb * tex.a;
                    }
                }

                float alpha = saturate(length(col));
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
