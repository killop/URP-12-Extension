#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//Always present in every shader
TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);float4 _MainTex_TexelSize;

half4 GetScreenColor(float2 uv)
{
    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
}


struct AttributesDefault
{
    float4 positionOS: POSITION;
    float2 uv: TEXCOORD0;
};


struct VaryingsDefault
{
    float4 positionCS: SV_POSITION;
    float2 uv: TEXCOORD0;
};


VaryingsDefault VertDefault(AttributesDefault input)
{
    VaryingsDefault output;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.uv;

    return output;
}



//------------------------------------------------------------------------------------------------------
// Generic functions
//------------------------------------------------------------------------------------------------------

float rand(float n)
{
    return frac(sin(n) * 13758.5453123 * 0.01);
}

float rand(float2 n)
{
    return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
}


