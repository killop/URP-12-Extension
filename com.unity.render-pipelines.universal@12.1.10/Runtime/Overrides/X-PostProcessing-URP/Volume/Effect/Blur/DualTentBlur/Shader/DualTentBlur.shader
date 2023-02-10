

Shader "Hidden/PostProcessing/Blur/DualTentBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 _BlurOffset;
    
    // 9-tap tent filter
    half4 TentFilter_9Tap(float2 uv, float2 texelSize)
    {
        float4 d = texelSize.xyxy * float4(1.0, 1.0, -1.0, 0.0);
        
        half4 s;
        s = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d.xy);
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d.wy) * 2.0; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d.zy); // 1 MAD
        
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw) * 2.0; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * 4.0; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw) * 2.0; // 1 MAD
        
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy);
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.wy) * 2.0; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy);
        
        return s * (1.0 / 16.0);
    }
    
    float4 FragTentBlur(VaryingsDefault i): SV_Target
    {
        return TentFilter_9Tap(i.uv, _BlurOffset.xy).rgba;
    }
    
    float4 FragCombine(VaryingsDefault i): SV_Target
    {
        return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
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
            #pragma fragment FragTentBlur
            
            ENDHLSL

        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragCombine
            
            ENDHLSL

        }
    }
}