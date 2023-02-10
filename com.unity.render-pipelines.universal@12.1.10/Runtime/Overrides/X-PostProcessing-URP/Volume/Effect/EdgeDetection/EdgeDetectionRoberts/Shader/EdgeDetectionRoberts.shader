

Shader "Hidden/PostProcessing/EdgeDetection/Roberts"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }

    HLSLINCLUDE
    
    #include "../../../../Shader/PostProcessing.hlsl"
    
    
    half2 _Params;
    half4 _EdgeColor;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _BackgroundFade _Params.y

    
    float intensity(in float4 color)
    {
        return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
    }
    
    float sobel(float stepx, float stepy, float2 center)
    {
        // get samples around pixel
        float topLeft = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(-stepx, stepy)));
        float bottomLeft = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(-stepx, -stepy)));
        float topRight = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(stepx, stepy)));
        float bottomRight = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, center + float2(stepx, -stepy)));
        
        // Roberts Operator
        //X = -1   0      Y = 0  -1
        //     0   1          1   0
        
        // Gx = sum(kernelX[i][j]*image[i][j])
        float Gx = -1.0 * topLeft + 1.0 * bottomRight;
        
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float Gy = -1.0 * topRight + 1.0 * bottomLeft;
        
        
        float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return sobelGradient;
    }
    
    
    
    half4 Frag(VaryingsDefault i): SV_Target
    {
        
        half4 sceneColor = GetScreenColor(i.uv);
        
        float sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.uv);
        
        half4 backgroundColor = lerp(sceneColor, _BackgroundColor, _BackgroundFade);
        
        float3 edgeColor = lerp(backgroundColor.rgb, _EdgeColor.rgb, sobelGradient);
        
        return float4(edgeColor, 1);
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
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag
            
            ENDHLSL

        }
    }
}
