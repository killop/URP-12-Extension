

Shader "Hidden/PostProcessing/Glitch/ScreenShake"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }

    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"
    
    uniform half _ScreenShake;
    
    
    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(127.1, 311.7))) * 43758.5453);
    }
    
    
    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShake;
        
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(i.uv.x + shake, i.uv.y)));
        
        return sceneColor;
    }
    
    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {
        
        float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShake;
        
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(i.uv.x, i.uv.y + shake)));
        
        return sceneColor;
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
    }
}
