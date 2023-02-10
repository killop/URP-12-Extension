

Shader "Hidden/PostProcessing/Blur/DirectionalBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half3 _Params;

    #define _Iteration _Params.x
    #define _Direction _Params.yz
    
    half4 DirectionalBlur(VaryingsDefault i)
    {
        half4 color = half4(0.0, 0.0, 0.0, 0.0);

        for (int k = -_Iteration; k < _Iteration; k++)
        {
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - _Direction * k);
        }
        half4 finalColor = color / (_Iteration * 2.0);

        return finalColor;
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {
        return DirectionalBlur(i);
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