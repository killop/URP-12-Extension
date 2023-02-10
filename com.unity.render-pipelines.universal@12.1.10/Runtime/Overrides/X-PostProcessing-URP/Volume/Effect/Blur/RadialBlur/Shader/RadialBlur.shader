

Shader "Hidden/PostProcessing/Blur/RadialBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half4 _Params;
    
    #define _BlurRadius _Params.x
    #define _Iteration _Params.y
    #define _RadialCenter _Params.zw
    
    
    half4 RadialBlur(VaryingsDefault i)
    {
        float2 blurVector = (_RadialCenter - i.uv.xy) * _BlurRadius;
        
        half4 acumulateColor = half4(0, 0, 0, 0);
        
        [unroll(30)]
        for (int j = 0; j < _Iteration; j++)
        {
            acumulateColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            i.uv.xy += blurVector;
        }
        
        return acumulateColor / _Iteration;
    }
    
    half4 Frag(VaryingsDefault i): SV_Target
    {
        return RadialBlur(i);
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