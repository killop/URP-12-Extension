

Shader "Hidden/PostProcessing/BulletTime"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        HLSLINCLUDE

        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM
            
            #pragma vertex VertDefault
            #pragma fragment frag

            #include "../../../Shader/PostProcessing.hlsl"
                                    
            CBUFFER_START(UnityPerMaterial)
            float4 _OriginPos;
            float _BlurQuality;
            float _BlurPower;
            float4 _BulletTimeColor;            
            CBUFFER_END

            float3 RGBtoHSV(float3 c){
                float4 K = float4(0.0f,-1.0f/3.0f,2.0f/3.0f,-1.0f);
                float4 p = lerp(float4(c.bg,K.wz),float4(c.gb,K.xy),step(c.b,c.g));
                float4 q = lerp(float4(p.xyw,c.r),float4(c.r,p.yzx),step(p.x,c.r));
                float d = q.x - min(q.w,q.y);
                float le = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y)/(6.0 * d + le)),d/(q.x + le),q.x);
            }

            float3 HSVtoRGB(float3 c){
                float4 K = float4(1.0,2.0/3.0,1.0/3.0,3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx,saturate(p - K.xxx),c.y);
            }


            
            float4 frag(VaryingsDefault input): SV_Target
            {
                //half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float3 center = float3(0.5,0.5f,0);

                float2 d = input.uv - center;
                float2 dir = d * d * d;

                float4 outColor = 0;
                for(int j = 0; j < _BlurQuality; ++j){
                    float2 uv = input.uv + dir * j * _BlurPower;
                    outColor += SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,uv);                    
                }

                outColor /= _BlurQuality;
                //outColor.rgb = 1 - outColor.rgb * 4;

                // float3 hsvColor = RGBtoHSV(outColor.rgb);
                // hsvColor.x += lerp(0,0.2,sin(PI * frac(_Time.y * 0.5)));
                // hsvColor.x = frac(hsvColor.x);
        
                // outColor.rgb = 1 - HSVtoRGB(hsvColor.rgb) * 8;
                outColor *= _BulletTimeColor;
                //float dis =  Length2(center - float3(input.uv,0));
                
                //if(dis < _OriginPos.w)
                //   var_MainTex.rgb = 1 - var_MainTex.rgb * 5;  
                                            
                return outColor;
            }
            
            ENDHLSL
            
        }
    }
    FallBack "Diffuse"
}
