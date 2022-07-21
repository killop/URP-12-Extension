

Shader "Hidden/PostProcessing/Blur/IrisBlurV2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half3 _Gradient;
    half4 _GoldenRot;
    half4 _Params;
    
    #define _Offset _Gradient.xy
    #define _AreaSize _Gradient.z
    #define _Iteration _Params.x
    #define _Radius _Params.y
    #define _PixelSize _Params.zw
    
    
    float IrisMask(float2 uv)
    {
        float2 center = uv * 2.0 - 1.0 + _Offset; // [0,1] -> [-1,1]
        return dot(center, center) * _AreaSize;
    }
    
    half4 FragPreview(VaryingsDefault i): SV_Target
    {
        return IrisMask(i.uv);
    }
    
    half4 IrisBlur(VaryingsDefault i)
    {
        half2x2 rot = half2x2(_GoldenRot);
        half4 accumulator = 0.0;
        half4 divisor = 0.0;
        
        half r = 1.0;
        half2 angle = half2(0.0, _Radius * saturate(IrisMask(i.uv)));
        
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
        return IrisBlur(i);
    }
    

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        // Pass 0 - IrisBlur
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag
            
            ENDHLSL

        }
        
        // Pass 1 - Preview
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragPreview
            
            ENDHLSL

        }
    }
}