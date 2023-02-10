

Shader "Hidden/PostProcessing/Glitch/RGBSplitV2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }


    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half3 _Params;

    #define _TimeX _Params.x
    #define _Amount _Params.y
    #define _Amplitude _Params.z

    
    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);
        
        half3 finalColor;
        finalColor.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x + splitAmout, i.uv.y)).r;
        finalColor.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).g;
        finalColor.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x - splitAmout, i.uv.y)).b;
        
        finalColor *= (1.0 - splitAmout * 0.5);
        
        return half4(finalColor, 1.0);
    }

    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);
        
        half3 finalColor;
        finalColor.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x, i.uv.y + splitAmout)).r;
        finalColor.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).g;
        finalColor.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x, i.uv.y - splitAmout)).b;
        
        finalColor *= (1.0 - splitAmout * 0.5);
        
        return half4(finalColor, 1.0);
    }

    half4 Frag_Vertical_Horizontal(VaryingsDefault i): SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);

        half3 finalColor;
        finalColor.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x + splitAmout, i.uv.y + splitAmout)).r;
        finalColor.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).g;
        finalColor.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(i.uv.x - splitAmout, i.uv.y + splitAmout)).b;

        finalColor *= (1.0 - splitAmout * 0.5);

        return half4(finalColor, 1.0);
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
            #pragma fragment Frag_Vertical_Horizontal

            ENDHLSL

        }
    }
}
