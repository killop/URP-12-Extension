

Shader "Hidden/PostProcessing/Pixelate/PixelizeQuad"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 _Params;
    #define _PixelSize _Params.x
    #define _PixelRatio _Params.y
    #define _PixelScaleX _Params.z
    #define _PixelScaleY _Params.w

    float2 RectPixelizeUV(half2 uv)
    {
        float pixelScale = 1.0 / _PixelSize;
        // Divide by the scaling factor, round up, and multiply by the scaling factor to get the segmented UV
        float2 coord = half2(pixelScale * _PixelScaleX * floor(uv.x / (pixelScale * _PixelScaleX)), (pixelScale * _PixelRatio * _PixelScaleY) * floor(uv.y / (pixelScale * _PixelRatio * _PixelScaleY)));

        return coord;
    }

    float4 Frag(VaryingsDefault i): SV_Target
    {
        return GetScreenColor(RectPixelizeUV(i.uv));
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