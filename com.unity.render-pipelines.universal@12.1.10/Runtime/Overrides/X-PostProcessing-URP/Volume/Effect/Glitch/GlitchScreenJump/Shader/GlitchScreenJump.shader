

Shader "Hidden/PostProcessing/Glitch/ScreenJump"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }

    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"
    
    uniform half2 _Params; // x: indensity , y : time
    #define _JumpIndensity _Params.x
    #define _JumpTime _Params.y
    
    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float jump = lerp(i.uv.x, frac(i.uv.x + _JumpTime), _JumpIndensity);
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(jump, i.uv.y)));
        return sceneColor;
    }
    
    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {
        float jump = lerp(i.uv.y, frac(i.uv.y + _JumpTime), _JumpIndensity);
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(i.uv.x, jump)));
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
