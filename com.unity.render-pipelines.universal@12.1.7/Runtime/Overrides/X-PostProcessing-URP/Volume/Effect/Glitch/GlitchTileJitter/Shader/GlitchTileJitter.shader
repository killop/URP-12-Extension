

Shader "Hidden/PostProcessing/Glitch/TileJitter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }

    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    #pragma shader_feature JITTER_DIRECTION_HORIZONTAL
    #pragma shader_feature USING_FREQUENCY_INFINITE


    uniform half4 _Params;

    #define _SplittingNumber _Params.x
    #define _JitterAmount _Params.y
    #define _JitterSpeed _Params.z
    #define _Frequency _Params.w

    
    float randomNoise(float2 c)
    {
        return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
    }

    float4 Frag_Vertical(VaryingsDefault i): SV_Target
    {
        float2 uv = i.uv.xy;
        half strength = 1.0;
        half pixelSizeX = 1.0 / _ScreenParams.x;
        
        // --------------------------------Prepare Jitter UV--------------------------------
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif

        if (fmod(uv.x * _SplittingNumber, 2) < 1.0)
        {
            #if JITTER_DIRECTION_HORIZONTAL
                uv.x += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
            #else
                uv.y += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
            #endif
        }

        // -------------------------------Final Sample------------------------------
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        return sceneColor;
    }
    
    float4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float2 uv = i.uv.xy;
        half strength = 1.0;
        half pixelSizeX = 1.0 / _ScreenParams.x;

        // --------------------------------Prepare Jitter UV--------------------------------
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        if (fmod(uv.y * _SplittingNumber, 2) < 1.0)
        {
            #if JITTER_DIRECTION_HORIZONTAL
                uv.x += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
            #else
                uv.y += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
            #endif
        }

        // -------------------------------Final Sample------------------------------
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
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
