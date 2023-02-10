

Shader "Hidden/PostProcessing/ColorAdjustment/LensFilter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Indensity;
    uniform half4 _LensColor;
    
    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        half lum = luminance(sceneColor.rgb);

        // Interpolate with half4(0.0, 0.0, 0.0, 0.0) based on luminance
        half4 filterColor = lerp(half4(0.0, 0.0, 0.0, 0.0), _LensColor, saturate(lum * 2.0));

        // Interpolate withhalf4(1.0, 1.0, 1.0, 1.0) based on luminance
        filterColor = lerp(filterColor, half4(1.0, 1.0, 1.0, 1.0), saturate(lum - 0.5) * 2.0);

        filterColor = lerp(sceneColor, filterColor, saturate(lum * _Indensity));

        return half4(filterColor.rgb, sceneColor.a);
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