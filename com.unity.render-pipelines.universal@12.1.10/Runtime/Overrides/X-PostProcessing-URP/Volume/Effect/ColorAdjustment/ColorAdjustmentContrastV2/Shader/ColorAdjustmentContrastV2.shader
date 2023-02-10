

Shader "Hidden/PostProcessing/ColorAdjustment/ContrastV2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Contrast;
    uniform half3 _ContrastFactorRGB;

    half3 ColorAdjustment_Contrast_V2(float3 In, half3 ContrastFactor, float Contrast)
    {
        half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
        return Out;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        finalColor.rgb = ColorAdjustment_Contrast_V2(finalColor.rgb, _ContrastFactorRGB, _Contrast);

        return finalColor;
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