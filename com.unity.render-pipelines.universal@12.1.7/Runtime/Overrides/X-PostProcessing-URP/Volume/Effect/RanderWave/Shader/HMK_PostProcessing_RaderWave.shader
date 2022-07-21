

Shader "Hidden/PostProcessing/RaderWave"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM
            
            #pragma vertex VertDefault
            #pragma fragment frag
            
            #include "../../../Shader/PostProcessing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _OriginPosition;
            float4 _WaveColor;
            float4 _RaderAreaColor;
            //float _MaxDistance;
            float _RaderWaveWidth;
            CBUFFER_END

            float3 GetWorldPosBYDepth(float2 uv)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                return worldPos;
            }                   

            float4 frag(VaryingsDefault input): SV_Target
            {
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float3 worldPos = GetWorldPosBYDepth(input.uv);
                
                float dis = distance(worldPos,_OriginPosition.xyz);
                float power = saturate(1 - abs(dis - _OriginPosition.w)/_RaderWaveWidth);
                float areaPower = saturate(dis - _OriginPosition.w) + 0.5f;

                var_MainTex.rgb *= saturate(areaPower);
                var_MainTex.rgb += _WaveColor.rgb * power;
                //var_MainTex.rgb *= _RaderAreaColor * (1 - areaPower);
                

                return var_MainTex;
            }
            
            ENDHLSL
            
        }
    }
    FallBack "Diffuse"
}
