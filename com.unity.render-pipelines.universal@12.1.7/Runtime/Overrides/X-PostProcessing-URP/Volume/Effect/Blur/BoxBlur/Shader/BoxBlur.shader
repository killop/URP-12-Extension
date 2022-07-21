

Shader "Hidden/PostProcessing/Blur/BoxBlur"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 _BlurOffset;
    
    half4 BoxFilter_4Tap(float2 uv, float2 texelSize)
    {
        float4 d = texelSize.xyxy * float4(-1.0, -1.0, 1.0, 1.0);
        
        half4 s = 0;
        s = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy) * 0.25h;  // 1 MUL
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy) * 0.25h; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xw) * 0.25h; // 1 MAD
        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zw) * 0.25h; // 1 MAD
        
        return s;
    }
    
    
    float4 FragBoxBlur(VaryingsDefault i): SV_Target
    {
        return BoxFilter_4Tap(i.uv, _BlurOffset.xy).rgba;
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
            #pragma fragment FragBoxBlur
            
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