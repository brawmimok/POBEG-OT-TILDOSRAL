Shader "Custom/TextureDarkenByZ"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeStart ("Fade Start (Z)", Float) = 0
        _FadeEnd ("Fade End (Z)", Float) = 5
        _Darkness ("Darkness Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionLS : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _FadeStart;
            float _FadeEnd;
            float4 _Darkness;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionLS = v.positionOS.xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float z = i.positionLS.z;
                float fade = saturate((z - _FadeStart) / (_FadeEnd - _FadeStart));

                float4 baseColor = tex2D(_MainTex, i.uv);
                float4 darkened = lerp(baseColor, _Darkness, fade);

                return darkened;
            }
            ENDHLSL
        }
    }
}
