

Shader "Hidden/PostProcessing/ColorAdjustment/Contrast"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Contrast;


    half3 ColorAdjustment_Contrast(half3 In, half Contrast)
    {
        half midpoint = 0.21763h;//pow(0.5, 2.2);
        half3 Out = (In - midpoint) * Contrast + midpoint;
        return Out;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        finalColor.rgb = ColorAdjustment_Contrast(finalColor.rgb, _Contrast);

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