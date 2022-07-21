Shader "Hidden/PostProcessing/DepthFog"
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

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            //基础颜色
            float4 _DepthFogColor;
            //x雾浓度
            //y高浓度区间调整值
            //z最小生效距离
            //w渐变区域距离
            float4 _FogParameter;
            
            float _FogBaseLevel;
            //float _FogConMax;


            TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);


            postFrag vert(postVert v)
            {
                postFrag o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.vertex.w = 1;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //o.uv =

                return o;
            }

            float4 frag(postFrag i): SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
                // apply fog
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv);
                
                float3 worldPos = ComputeWorldSpacePosition(i.uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                float3 cameraPos = _WorldSpaceCameraPos.xyz;

                float fogConcentration = _FogParameter.x;
                float fogHeight = _FogParameter.y * 0.001;
                float fogEnableDistance = _FogParameter.z;
                float fogEnableDistanceArea = _FogParameter.w;
                float fogCacHeight = worldPos.y - _FogBaseLevel;
                

                float3 CtoV = cameraPos - worldPos;
                
                float ConcentrationByDistance = saturate((length(CtoV) - fogEnableDistance) / fogEnableDistanceArea);
                float ConcentrationByCamera = saturate(1 - exp2(-CtoV.y * fogHeight));
                float ConcentrationByVertexHeight = saturate(exp(-fogCacHeight * fogHeight));
                float fogPower = fogConcentration * ConcentrationByDistance * ConcentrationByCamera * ConcentrationByVertexHeight;// * _FogGlobalCon;

                
                //col.rgb += fogPower * _DepthFogColor.rgb;
                col.rgb = lerp(col.rgb, _DepthFogColor.rgb, fogPower);
                //return 1;
                return col;
            }
            ENDHLSL

        }
    }
}
