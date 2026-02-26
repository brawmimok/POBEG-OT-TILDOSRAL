Shader "Custom/AbsoluteDarkness"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(worldPos);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _Color;
            }

            ENDHLSL
        }
    }
}
