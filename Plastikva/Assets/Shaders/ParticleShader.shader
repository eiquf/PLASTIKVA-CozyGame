Shader "Custom/ParticleShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BubbleTex ("Bubble Mask Texture", 2D) = "white" {} // bubble pattern (circle/tiling)
        _Color ("Tint", Color) = (1,1,1,1)
        _Speed ("Rise Speed", Range(0,5)) = 1
        _Distortion ("Wobble", Range(0,1)) = 0.1
        _Intensity ("Bubble Intensity", Range(0,2)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BubbleTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Speed;
            float _Distortion;
            float _Intensity;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base sprite
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                // Animate bubbles rising (scroll UV upward)
                float2 bubUV = i.uv;
                bubUV.y += _Time.y * _Speed;

                // Add horizontal wobble
                bubUV.x += sin((bubUV.y + _Time.y) * 5) * _Distortion;

                // Sample bubble texture (use a tiling circle mask texture)
                fixed4 bub = tex2D(_BubbleTex, bubUV);

                // Combine bubble layer with sprite (multiply alpha so bubbles fade naturally)
                fixed4 col = baseCol + bub * _Intensity;
                col.a *= baseCol.a;

                return col;
            }
            ENDCG
        }
    }
}
