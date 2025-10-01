Shader "Custom/SpriteShadowShader3D"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

        // Wind controls
        _WindStrength("Wind Strength", Range(0,0.2)) = 0.05
        _WindSpeed("Wind Speed", Range(0,10)) = 2.0
        _WindFrequency("Wind Frequency", Range(0,10)) = 3.0

        // Highlight wave controls
        _HighlightColor("Highlight Color", Color) = (1,1,0,1)
        _WaveStrength("Wave Strength", Range(0,1)) = 0.4
        _WaveSpeed("Wave Speed", Range(0,10)) = 2.0
        _WaveFrequency("Wave Frequency", Range(1,50)) = 6.0
        _BlendStrength("Blend Strength", Range(0,2)) = 1.0
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

        Cull Off
        ZWrite On
        ZTest LEqual
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alphatest:_Cutoff addshadow nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile_local _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };

        // Wind
        float _WindStrength;
        float _WindSpeed;
        float _WindFrequency;

        // Highlight wave
        fixed4 _HighlightColor;
        float _WaveStrength;
        float _WaveSpeed;
        float _WaveFrequency;
        float _BlendStrength;

        void vert(inout appdata_full v, out Input o)
        {
            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            // Wind sway only modifies X, keeps Y and Z intact
            float wave = sin(_Time.y * _WindSpeed + v.vertex.y * _WindFrequency);
            v.vertex.x += wave * _WindStrength;

            // Keep Z as provided by mesh/transform for proper 3D placement
            // (no override needed)

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap(v.vertex);
            #endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color * _RendererColor;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;

            // Base color
            fixed3 finalCol = c.rgb;

            // Wave highlight overlay
            float wave = sin((IN.uv_MainTex.y * _WaveFrequency) + (_Time.y * _WaveSpeed));
            wave = (wave * 0.5 + 0.5); // normalize 0–1
            wave *= _WaveStrength;

            finalCol = lerp(finalCol, _HighlightColor.rgb, wave * _BlendStrength);

            o.Albedo = finalCol * c.a;
            o.Alpha  = c.a;
        }
        ENDCG
    }

    Fallback "Transparent/VertexLit"
}
