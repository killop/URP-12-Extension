

Shader "Hidden/PostProcessing/Glitch/AnalogNoise"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"
    
    uniform half4 _Params;
    #define _Speed _Params.x
    #define _Fading _Params.y
    #define _LuminanceJitterThreshold _Params.z
    #define _TimeX _Params.w

    
    float randomNoise(float2 c)
    {
        return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 Frag(VaryingsDefault i): SV_Target
    {

        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half4 noiseColor = sceneColor;

        half luminance = dot(noiseColor.rgb, half3(0.22, 0.707, 0.071));
        if (randomNoise(float2(_TimeX * _Speed, _TimeX * _Speed)) > _LuminanceJitterThreshold)
        {
            noiseColor = float4(luminance, luminance, luminance, luminance);
        }

        float noiseX = randomNoise(_TimeX * _Speed + i.uv / float2(-213, 5.53));
        float noiseY = randomNoise(_TimeX * _Speed - i.uv / float2(213, -5.53));
        float noiseZ = randomNoise(_TimeX * _Speed + i.uv / float2(213, 5.53));

        noiseColor.rgb += 0.25 * float3(noiseX, noiseY, noiseZ) - 0.125;

        noiseColor = lerp(sceneColor, noiseColor, _Fading);
        
        return noiseColor;
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
