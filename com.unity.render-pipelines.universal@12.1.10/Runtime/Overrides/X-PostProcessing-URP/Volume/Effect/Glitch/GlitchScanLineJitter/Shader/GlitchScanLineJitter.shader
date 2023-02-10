

Shader "Hidden/PostProcessing/Glitch/ScanLineJitter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"
    
    #pragma shader_feature USING_FREQUENCY_INFINITE
    
    uniform half3 _Params;
    #define _Amount _Params.x
    #define _Threshold _Params.y
    #define _Frequency _Params.z
    
    
    
    
    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }
    
    
    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        half strength = 0;
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        
        
        float jitter = randomNoise(i.uv.y, _Time.x) * 2 - 1;
        jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
        
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(i.uv + float2(jitter, 0)));
        
        return sceneColor;
    }
    
    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {
        half strength = 0;
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        
        float jitter = randomNoise(i.uv.x, _Time.x) * 2 - 1;
        jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
        
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(i.uv + float2(0, jitter)));
        
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
