

Shader "Hidden/PostProcessing/RainRipple"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
        _RippleTexture("RippleTexture",2D) = "white"{}
        _FlowDownTexture("_FlowDownTexture",2D) = "white"{}
        _FlowDownNormal("_FlowDownNormal",2D) = "wihte"{}

    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            
            
            Cull Back
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            float2 _TexelSize;            
            float _MaxDistance;
            float _RippleDiffusionFrequency;
            float _RippleSize;
            float _RippleStrength;
            float _RainIntensity;
            float _RippleBias;
            float _FlowSpeed;
            float _FlowStrength;
            float _RainEnableDistance;
            CBUFFER_END

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
            TEXTURE2D(_RippleTexture);SAMPLER(sampler_RippleTexture);
            TEXTURE2D(_FlowDownTexture);SAMPLER(sampler_FlowDownTexture);
            TEXTURE2D(_FlowDownNormal);SAMPLER(sampler_FlowDownNormal);
            
            struct Attributes
            {
                float4 positionOS: POSITION;
                float2 uv: TEXCOORD0;
                //float3 normalOS: NORMAL;
            };


            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float2 uv: TEXCOORD0;
                //float3 normalWS: NORMAL;
            };

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

            


            // float ComputeRipple(float2 UV, float CurrentTime,float Weight){
            //     float4 Ripple = SAMPLE_TEXTURE2D(_RippleTexture,sampler_RippleTexture,UV);

            //     float offtimeStart = Ripple.w + CurrentTime;
            //     float offtimeDis = Ripple.x;
            //     float offtime = frac(offtimeStart + offtimeDis);

            //     float re = saturate(4.2 * sin(offtime * PI) - 3.8) * Ripple.x;
            //     return re;
            // }

            float ComputeRipple(float2 UV, float CurrentTime,float Weight){
                float4 Ripple = SAMPLE_TEXTURE2D(_RippleTexture,sampler_RippleTexture,UV);
                float offtimeStart = frac(Ripple.w + CurrentTime);
                float offtime = offtimeStart - 1.0f + Ripple.x;
                float timeFactor = saturate(0.2 + 0.8 * Weight - offtime);
                float FinalFactor = timeFactor * Ripple.x * saturate(sin(clamp(offtime * 9.0f,0.0,3.0f) * PI));
                //float offtimeDis = Ripple.x;
                return FinalFactor * 0.35;
            }


            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                //output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;

                return output;
            }

            const static float4 TimeMul = float4(1.0f,0.85f,0.93f,1.13f);
            const static float4 TimeAdd = float4(0.0f,0.2f,0.45f,0.7f);

            float4 frag(Varyings input): SV_Target
            {
                
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float RainIntensity = _RainIntensity; //雨水强度
               
                float3 worldPos = GetWorldPosBYDepth(input.uv);
                float3 cameraPos = _WorldSpaceCameraPos.xyz;
                float dis = distance(cameraPos, worldPos);
                //float2 rippleUV = float2(input.uv); //转变为世界坐标
                float2 rippleUV = float2(worldPos.x,worldPos.z) * _RippleSize;
                
                float4 Times = (_Time * TimeMul + TimeAdd) * 1.6f;
               
                float re1 = ComputeRipple(rippleUV + float2(0.25f,0.0f),Times.x * 10 * _RippleDiffusionFrequency,1);
                float re2 = ComputeRipple(rippleUV + float2(-0.55f,0.3f),Times.y * _RippleDiffusionFrequency,1);
                float re3 = ComputeRipple(rippleUV + float2(0.6f,0.85f),Times.z * _RippleDiffusionFrequency,1);
                float re4 = ComputeRipple(rippleUV + float2(0.5f,-0.75f),Times.w * _RippleDiffusionFrequency,1);
                float3 nor = GetWorldNormalBYDepth(input.uv);

                float4 Weights = RainIntensity - float4(0,0.25,0.5,0.75);
                Weights = saturate(Weights * 4);
                float re = Weights.x * re1 + Weights.y * re2 + Weights.z * re3 + Weights.w * re4;
                float intensity = dot(nor,float3(0,1,0));

                float ripplePower = saturate((intensity - _RippleBias) * 1/(1 -_RippleBias)) ;
                float3 resultRipple = re * ripplePower * _RippleStrength;
                //var_MainTex.rgb += re * saturate(1 - dis/300) * ripplePower;

                //墙面水流下效果                               
                float2 flowUVX = float2(worldPos.x,worldPos.y) * 0.2;
                float2 flowUVZ = float2(worldPos.z,worldPos.y) * 0.2;

                float2 flowMaskTime = float2(0,_Time.y) * 0.1f * _FlowSpeed;
                float flowRX = SAMPLE_TEXTURE2D(_FlowDownTexture,sampler_FlowDownTexture,flowUVX * 2.5 + flowMaskTime).g;               
                float flowRZ = SAMPLE_TEXTURE2D(_FlowDownTexture,sampler_FlowDownTexture,flowUVZ * 2.5 + flowMaskTime).g;

                float2 flowOffTime = float2(0,_Time.x) * 0.4f * _FlowSpeed;
                
                float3 flowNormalX = SAMPLE_TEXTURE2D(_FlowDownNormal,sampler_FlowDownNormal,flowUVX + flowOffTime).xyz * 2 - 1;
                float3 flowNormalZ = SAMPLE_TEXTURE2D(_FlowDownNormal,sampler_FlowDownNormal,flowUVZ + flowOffTime).xyz * 2 - 1;

                float NX = abs(dot(nor,float3(1,0,0)));                
                float NZ = abs(dot(nor,float3(0,0,1)));
                float NH = NX/(NX + NZ);
                //float3 flowNormal = normalize(flowNormalX * NZ * flowRX + flowNormalZ * NX * flowRZ);
                float3 flowNormal = lerp(flowNormalX * flowRX,flowNormalZ * flowRZ,NH);

                //var_MainTex.rgb += resultRipple;

                float3 resultFlow = 0;
                if(_RippleBias - intensity > 0){
                    resultFlow = saturate(flowNormal.z) * _FlowStrength;
                }
                var_MainTex.rgb += (resultRipple * saturate(1 - (dis/_RainEnableDistance)) + resultFlow);
                
                return var_MainTex;                
            }
            
            ENDHLSL
            
        }
    }
    FallBack "Diffuse"
}
