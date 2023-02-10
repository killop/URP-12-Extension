

Shader "Hidden/PostProcessing/Blur/KawaseBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Offset;
    
    
    half4 KawaseBlur(float2 uv, float2 texelSize, half pixelOffset)
    {
        half4 o = 0;
        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(pixelOffset +0.5, pixelOffset +0.5) * texelSize);
        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-pixelOffset -0.5, pixelOffset +0.5) * texelSize);
        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-pixelOffset -0.5, -pixelOffset -0.5) * texelSize);
        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(pixelOffset +0.5, -pixelOffset -0.5) * texelSize);
        return o * 0.25;
    }
    
    
    half4 Frag(VaryingsDefault i): SV_Target
    {
        return KawaseBlur(i.uv.xy, _MainTex_TexelSize.xy, _Offset);
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