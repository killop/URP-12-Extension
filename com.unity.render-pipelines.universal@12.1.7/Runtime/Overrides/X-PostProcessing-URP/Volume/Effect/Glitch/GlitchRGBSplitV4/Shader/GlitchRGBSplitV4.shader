

Shader "Hidden/PostProcessing/Glitch/RGBSplitV4"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }


    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half2 _Params;

    #define _Indensity _Params.x
    #define _TimeX _Params.y

    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float splitAmount = _Indensity * randomNoise(_TimeX, 2);

        half4 ColorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x + splitAmount, i.uv.y));
        half4 ColorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half4 ColorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x - splitAmount, i.uv.y));

        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
    }

    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {

        float splitAmount = _Indensity * randomNoise(_TimeX, 2);

        half4 ColorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half4 ColorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x, i.uv.y + splitAmount));
        half4 ColorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x, i.uv.y - splitAmount));

        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
    }

    half4 Frag_Horizontal_Vertical(VaryingsDefault i): SV_Target
    {

        float splitAmount = _Indensity * randomNoise(_TimeX, 2);

        half4 ColorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half4 ColorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x + splitAmount, i.uv.y + splitAmount));
        half4 ColorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x - splitAmount, i.uv.y - splitAmount));

        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
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
            #pragma fragment Frag_Horizontal
            
            ENDHLSL

        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_Vertical
            
            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_Horizontal_Vertical

            ENDHLSL

        }
    }
}
