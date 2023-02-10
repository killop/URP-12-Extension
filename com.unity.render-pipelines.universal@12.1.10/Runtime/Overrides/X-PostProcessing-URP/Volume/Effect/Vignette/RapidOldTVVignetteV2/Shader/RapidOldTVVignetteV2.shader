

Shader "Hidden/PostProcessing/Vignette/RapidOldTVVignetteV2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _VignetteSize;
    uniform half _SizeOffset;
    uniform half4 _VignetteColor;
    
    
    float4 Frag(VaryingsDefault i): SV_Target
    {
        half2 uv = -i.uv * i.uv + i.uv;	     //MAD
        half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
        return VignetteIndensity * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
    }
    
    float4 Frag_ColorAdjust(VaryingsDefault i): SV_Target
    {
        half2 uv = -i.uv * i.uv + i.uv;    //MAD
        half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
        
        return lerp(_VignetteColor, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv), VignetteIndensity);
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
        
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_ColorAdjust
            
            ENDHLSL

        }
    }
}