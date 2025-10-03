Shader "Custom/BubblesSurfaceWithTex"
{
    Properties
    {
        _Color ("Bubble Color", Color) = (0.5,0.8,1,1)
        _BubbleTex ("Bubble Texture", 2D) = "white" {}   // Bubble texture
        _BubbleCount ("Bubble Count", Range(1, 50)) = 20
        _Speed ("Speed", Range(0.1, 2.0)) = 0.5
        _Radius ("Bubble Radius", Range(0.01, 0.2)) = 0.08
        _LifeTime ("Bubble Life Time", Range(0.001, 10.0)) = 3.0
        _Direction ("Bubble Direction", Vector) = (0,1,0,0) // << New property
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
            float4 _BubbleTex_ST;
            int _BubbleCount;
            float _Speed;
            float _Radius;
            float _LifeTime;
            float4 _Direction;

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
                float3 col = 0;

                float2 dir = normalize(_Direction.xy);

                for (int idx = 0; idx < _BubbleCount; idx++)
                {
                    float id = (float)idx;

                    float startOffset = hash(id * 99.9);

                    float life = fmod((t + startOffset) / _LifeTime, 1.0);

                    float baseX = hash(id * 12.3) * 2.0 - 1.0;
                    float baseY = hash(id * 45.6) * 2.0 - 1.0;

                    float2 basePos = float2(baseX, baseY);

                    float2 center = basePos + dir * (life * 2.0 - 1.0);

                    float growth = 0.0;
                    if (life < 0.2)
                        growth = smoothstep(0.0, 0.2, life);
                    else if (life > 0.8)
                        growth = 1.0 - smoothstep(0.8, 1.0, life);
                    else
                        growth = 1.0;

                    float r = _Radius * growth + hash(id * 7.7) * 0.02;

                    float2 bubUV = (uv - center) / (r * 2.0) + 0.5;

                    if (all(bubUV >= 0) && all(bubUV <= 1))
                    {
                        fixed4 tex = tex2D(_BubbleTex, bubUV * _BubbleTex_ST.xy + _BubbleTex_ST.zw);
                        col += tex.rgb * _Color.rgb * tex.a * growth;
                    }
                }

                float alpha = saturate(length(col));
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
