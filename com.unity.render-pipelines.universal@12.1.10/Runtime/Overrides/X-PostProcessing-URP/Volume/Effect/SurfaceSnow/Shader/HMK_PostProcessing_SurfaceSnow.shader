

Shader "Hidden/PostProcessing/SurfaceSnow"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
        _SnowTexture("SnowTexture",2D) = "white"{}
        _SnowBlurTex("SnowBlurTex",2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "../../../Shader/PostProcessing.hlsl"
        //#include "./NoiseLibrary.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float2 _TexelSize;
            float _SnowBias;
            float _SnowStrength;
            float _SnowSize;
            float _DebugValue;
            uint _SampleOffset;
        CBUFFER_END
        ENDHLSL


        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM
            
            #pragma vertex VertDefault
            #pragma fragment frag


            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
            
            
            

            TEXTURE2D(_SnowTexture);SAMPLER(sampler_SnowTexture);

            //float4 _MainTex_TexelSize;

            float3 GetWorldPosBYDepth(float2 uv)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                return worldPos;
            }

            float3 GetWorldNormalBYDepth(float2 uv){
                
                float2 offUV[4] = {
                    float2(-_TexelSize.x,0),
                    float2( _TexelSize.x,0),
                    float2(0, _TexelSize.y),
                    float2(0,-_TexelSize.y)
                };
                float3 posWS[4];
                for(uint i = 0; i < 4; i++){
                    posWS[i] = GetWorldPosBYDepth(uv + offUV[i]);
                }
                
                float3 vV = posWS[1] - posWS[0];
                float3 vH = posWS[3] - posWS[2];

                float3 nor = normalize(cross(vV,vH));
                return nor;
            }

            

                                    
            float4 frag(VaryingsDefault input): SV_Target
            {
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                //return 1;

                float3 worldPos = GetWorldPosBYDepth(input.uv);
                float3 nor = GetWorldNormalBYDepth(input.uv);

                float2 snowUV = float2(worldPos.x,worldPos.z) * _SnowSize;    

                float3 snowTex = SAMPLE_TEXTURE2D(_SnowTexture,sampler_SnowTexture,snowUV).rgb + 0.1;
                
                float Ndot = dot(nor,float3(0,1,0));
                float intensity = saturate((Ndot - _SnowBias) * 1/(1 - _SnowBias));

                var_MainTex.rgb = lerp(var_MainTex.rgb,0.98,saturate(snowTex.r * intensity * _DebugValue));
                
                float3 re = saturate(snowTex.r * intensity * _DebugValue);
                //return var_MainTex;
                return float4(re,1);
            }
            
            ENDHLSL
            
        }

        pass{
            ZTest Off  
            Cull Off  
            ZWrite Off  

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            
            TEXTURE2D(_SnowBlurTex);SAMPLER(sampler_SnowBlurTex);
            const static float4 offset = float4(1,1,-1,-1);
            //const static float4 offset2 = float4(-1,-1,1,1);

            float4 frag(VaryingsDefault input): SV_Target
            {
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                

                // float2 uv1 = offset.xy * _TexelSize + input.uv;
                // float2 uv2 = offset.zw * _TexelSize + input.uv;
                // float2 uv3 = offset.xy * _TexelSize * 2 + input.uv;
                // float2 uv4 = offset.zw * _TexelSize * 2 + input.uv;
                // float2 uv5 = offset.xy * _TexelSize * 3 + input.uv;
                // float2 uv6 = offset.zw * _TexelSize * 3 + input.uv;

                
                float4 col = 0;
                col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,input.uv) * 1;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv1) * 0.15;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv2) * 0.15;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv3) * 0.10;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv4) * 0.10;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv5) * 0.05;
                // col += SAMPLE_TEXTURE2D(_SnowBlurTex,sampler_SnowBlurTex,uv6) * 0.05;

                var_MainTex += col;
                
                return var_MainTex;
                //return float4(nor,1);
            }
            ENDHLSL

        }


    }
    FallBack "Diffuse"
}
