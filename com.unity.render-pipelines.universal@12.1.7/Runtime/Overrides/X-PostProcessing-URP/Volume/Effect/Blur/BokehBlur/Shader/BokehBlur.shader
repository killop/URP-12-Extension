

Shader "Hidden/PostProcessing/Blur/BokehBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 _GoldenRot;
    half4 _Params;
    
    #define _Iteration _Params.x
    #define _Radius _Params.y
    #define _PixelSize _Params.zw
    
    half4 BokehBlur(VaryingsDefault i)
    {
        half2x2 rot = half2x2(_GoldenRot);
        half4 accumulator = 0.0;
        half4 divisor = 0.0;

        half r = 1.0;
        half2 angle = half2(0.0, _Radius);

        for (int j = 0; j < _Iteration; j++)
        {
            r += 1.0 / r;
            angle = mul(rot, angle);
            half4 bokeh = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv + _PixelSize * (r - 1.0) * angle));
            accumulator += bokeh * bokeh;
            divisor += bokeh;
        }
        return accumulator / divisor;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {
        return BokehBlur(i);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

            ENDHLSL

        }
    }
}