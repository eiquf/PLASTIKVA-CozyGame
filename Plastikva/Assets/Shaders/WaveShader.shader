Shader "Custom/WaveShader"
{
    Properties {
        _MainTex("Base Texture", 2D) = "white" {}
        _WaveTex("Wave Texture", 2D) = "white" {}
        _WaveStrength("Wave Strength", Range(0,0.1)) = 0.05
        _WaveSpeed("Wave Speed", Range(0,10)) = 2.0
        _WaveFrequency("Wave Frequency", Range(0,20)) = 5.0
        _Alpha("Transparency", Range(0,1)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass {
            Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WaveTex;
            float4 _WaveTex_ST;

            float _WaveStrength;
            float _WaveSpeed;
            float _WaveFrequency;
            float _Alpha;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uvBase : TEXCOORD0;
                float2 uvWave : TEXCOORD1;
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(2,3)
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBase = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvWave = TRANSFORM_TEX(v.uv, _WaveTex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Base texture
                fixed4 baseCol = tex2D(_MainTex, i.uvBase);

                // Wave distortion
                float wave = sin(i.uvWave.y * _WaveFrequency + _Time.y * _WaveSpeed);
                float2 distortedUV = i.uvWave;
                distortedUV.x += wave * _WaveStrength;

                // Wave texture (distorted)
                fixed4 waveCol = tex2D(_WaveTex, distortedUV);

                // Blend base + wave
                float blend = 0.3;
                fixed4 col = lerp(baseCol, waveCol, blend);

                // Apply shadow attenuation
                half shadow = SHADOW_ATTENUATION(i);
                col.rgb *= shadow;

                // Transparency
                col.a *= _Alpha;
                return col;
            }
            ENDHLSL
        }

        // Shadow caster pass (so it casts shadows too)
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_full v) {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDHLSL
        }
    }
}
