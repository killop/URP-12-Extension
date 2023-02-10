

Shader "Hidden/PostProcessing/ColorAdjustment/Tint"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Indensity;
    uniform half4 _ColorTint;
    
    half4 Frag(VaryingsDefault i): SV_Target
    {
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half3 finalColor = lerp(sceneColor.rgb, sceneColor.rgb * _ColorTint.rgb, _Indensity);
        
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
            #pragma fragment Frag

            ENDHLSL

        }
    }
}