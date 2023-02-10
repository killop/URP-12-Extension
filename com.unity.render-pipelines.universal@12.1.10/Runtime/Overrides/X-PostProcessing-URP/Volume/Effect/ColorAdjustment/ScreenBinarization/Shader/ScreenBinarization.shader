Shader "Hidden/PostProcessing/ScreenBinarization"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "../../../../Shader/PostProcessing.hlsl"

            float _BinarizationAmount;

            float4 frag(VaryingsDefault i): SV_Target
            {
                float4 col = GetScreenColor(i.uv);
                
                float binarization = 0.299 * col.r + 0.587 * col.g + 0.114 * col.b;
                col = lerp(col, binarization, _BinarizationAmount);
                return col;
            }
            ENDHLSL

        }
    }
}
