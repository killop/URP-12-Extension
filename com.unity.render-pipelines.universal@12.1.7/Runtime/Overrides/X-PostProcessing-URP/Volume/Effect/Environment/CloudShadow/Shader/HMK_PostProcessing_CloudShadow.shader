Shader "Hidden/PostProcessing/CloudShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }

        _CloudTex ("CloudTex", 2D) = "white" { }
        _CloudShadowColor ("XYZ:ColorMultiplier", Color) = (1, 0, 0, 1)
        _CloudTiling ("XY: Scale Z : Strength W: Distance ", Vector) = (1, 1, 1, 1)
        _WindFactor ("XY:WindDirection ZW;CutoffRange", Vector) = (1, 1, 1, 100)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct postVert
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
            };

            struct postFrag
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
            };

            
            bool _CloudShadowMode;
            
            float4 _CloudTiling;
            float4 _WindFactor;

            float4 _CloudShadowColor;
            float3 _LightDirection;
            uint _CloudHeight;

            TEXTURE2D(_CameraColorTexture);SAMPLER(sampler_CameraColorTexture);
            TEXTURE2D(_CloudTex);SAMPLER(sampler_CloudTex);

            

            postFrag vert(postVert v)
            {
                postFrag o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.vertex.w = 1;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            
            float3 GetWorldPosBYDepth(float2 uv)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                return worldPos;
            }
            

            float4 frag(postFrag i): SV_Target
            {
                
                float4 col = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, i.uv);
                
                float3 cameraPos = _WorldSpaceCameraPos.xyz;
                
                float3 worldPos = GetWorldPosBYDepth(i.uv);
                float dis = distance(cameraPos, worldPos);
                float disStrength = 1 - saturate(dis / _CloudTiling.w);
                
                Light light = GetMainLight();
                float3 ldir = light.direction;

                float2 cloud_uv;
                
                float2 offuv = float2(_Time.x * _WindFactor.x, _Time.x * _WindFactor.y) * 0.05f;
                
                
                if (_CloudShadowMode)
                {
                    //阴影受到光方向影响
                    float hd = _CloudHeight - worldPos.y;          //获取与云层的高度差
                    float t = hd / dot(ldir, float3(0, 1, 0));
                    float3 cloudCrossPos = ldir * t + worldPos;    //光源到像素的射线与云层交点
                    cloud_uv = cloudCrossPos.xz / _CloudTiling.xy;
                }
                else
                {
                    //垂直投影
                    cloud_uv.xy = worldPos.xz / _CloudTiling.xy;
                }
                cloud_uv += offuv;
                
                float cloud = (1 - SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, cloud_uv).r);
                
                cloud = smoothstep(_WindFactor.z, _WindFactor.w, cloud) * _CloudTiling.z;
                
                cloud = lerp(0, cloud, disStrength);
                float3 targetColor = cloud * (1 - _CloudShadowColor.rgb);
                
                targetColor = cloud * (1 - _CloudShadowColor.rgb);
                col.rgb -= targetColor * col.rgb;

                return col;//* float4(up,1);
                //return 1;

            }
            ENDHLSL

        }
    }
}
