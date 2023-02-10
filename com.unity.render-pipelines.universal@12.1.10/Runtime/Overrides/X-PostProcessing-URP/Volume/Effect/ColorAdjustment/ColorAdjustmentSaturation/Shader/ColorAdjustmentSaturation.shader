

Shader "Hidden/PostProcessing/ColorAdjustment/Saturation"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Saturation;

    half3 Saturation(half3 In, half Saturation)
    {
        half luma = dot(In, half3(0.2126729, 0.7151522, 0.0721750));
        half3 Out = luma.xxx + Saturation.xxx * (In - luma.xxx);
        return Out;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half3 col = 0.5 + 0.5 * cos(_Time.y + i.uv.xyx + half3(0, 2, 4));

        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        return half4(Saturation(sceneColor.rgb, 1 - _Saturation), 1.0);
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