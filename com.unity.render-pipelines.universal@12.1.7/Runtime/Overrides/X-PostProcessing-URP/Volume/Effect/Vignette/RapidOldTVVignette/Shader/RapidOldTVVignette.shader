

Shader "Hidden/PostProcessing/Vignette/RapidOldTVVignette"
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
    half2 _VignetteCenter;
    half4 _VignetteColor;
    
    
    VertexOutput Vert(AttributesDefault v)
    {
        VertexOutput o;
        o.vertex = TransformObjectToHClip(v.positionOS.xyz);
        o.texcoord.xy = v.uv;
        // uv [0, 1] ->[-0.5, 0.5]
        o.texcoord.zw = o.texcoord.xy - _VignetteCenter;
        
        return o;
    }
    
    float4 Frag(VertexOutput i): SV_Target
    {
        float4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy);
        
        //普通vignette曲线 -> Old TV曲线
        i.texcoord.zw *= i.texcoord.zw;
        
        //求解vignette强度
        float vignetteIndensity = saturate(1.0 - dot(i.texcoord.zw, i.texcoord.zw) * _VignetteIndensity * 20);
        
        return vignetteIndensity * finalColor;
    }
    
    
    float4 Frag_ColorAdjust(VertexOutput i): SV_Target
    {
        float4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy);
        
        //普通vignette曲线 -> Old TV曲线
        i.texcoord.zw *= i.texcoord.zw;
        
        //求解vignette强度
        float vignetteIndensity = saturate(1.0 - dot(i.texcoord.zw, i.texcoord.zw) * _VignetteIndensity * 20);
        
        //基于vignette强度，插值VignetteColor颜色和场景颜色
        finalColor.rgb = lerp(_VignetteColor.rgb, finalColor.rgb, vignetteIndensity);
        
        return half4(finalColor.rgb, _VignetteColor.a);
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

            #pragma vertex Vert
            #pragma fragment Frag
            
            ENDHLSL

        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_ColorAdjust
            
            ENDHLSL

        }
    }
}