

Shader "Hidden/PostProcessing/ColorAdjustment/ContrastV3"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half4 _Contrast;

    half3 ColorAdjustment_Contrast_V3(float3 In, half3 ContrastFactor, float Contrast)
    {
        half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
        return Out;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        finalColor.rgb = ColorAdjustment_Contrast_V3(finalColor.rgb, half3(_Contrast.x, _Contrast.y, _Contrast.z), 1 - (_Contrast.w));
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