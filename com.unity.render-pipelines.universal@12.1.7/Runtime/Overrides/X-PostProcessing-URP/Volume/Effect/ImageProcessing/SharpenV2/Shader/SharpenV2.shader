

Shader "Hidden/PostProcessing/ImageProcessing/SharpenV2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Sharpness;

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
        pixelSize *= 1.5f;

        half4 blur = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(pixelSize.x, -pixelSize.y));
        blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-pixelSize.x, -pixelSize.y));
        blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(pixelSize.x, pixelSize.y));
        blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-pixelSize.x, pixelSize.y));
        blur *= 0.25;


        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        return sceneColor + (sceneColor - blur) * _Sharpness;
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