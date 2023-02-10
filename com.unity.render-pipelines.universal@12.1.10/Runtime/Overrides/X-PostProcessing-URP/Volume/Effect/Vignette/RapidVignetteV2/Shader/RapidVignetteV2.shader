

Shader "Hidden/PostProcessing/Vignette/RapidVignetteV2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    struct VertexOutput
    {
        float4 vertex: SV_POSITION;
        float4 texcoord: TEXCOORD0;
    };
    
    half _VignetteIndensity;
    half _VignetteSharpness;
    half2 _VignetteCenter;
    half4 _VignetteColor;
    
    
    float4 Frag(VertexOutput i): SV_Target
    {
        
        float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy);
        
        half indensity = distance(i.texcoord.xy, _VignetteCenter.xy);
        indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
        return sceneColor * indensity;
    }
    
    
    float4 Frag_ColorAdjust(VertexOutput i): SV_Target
    {
        
        float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy);
        
        half indensity = distance(i.texcoord.xy, _VignetteCenter.xy);
        indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
        
        half3 finalColor = lerp(_VignetteColor.rgb, sceneColor.rgb, indensity);
        
        return float4(finalColor.rgb, _VignetteColor.a);
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
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_ColorAdjust
            
            ENDHLSL

        }
    }
}