

Shader "Hidden/PostProcessing/SunShaft"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
     
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    float4 Frag(VaryingsDefault i): SV_Target
    {

        return GetScreenColor(i.uv);
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