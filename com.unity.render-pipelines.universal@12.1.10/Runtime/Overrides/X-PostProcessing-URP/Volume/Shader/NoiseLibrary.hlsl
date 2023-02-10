
//----------------------------------------------------------------------------------------------------------
//  XNoiseLibrary.hlsl
// A Collection of  2D/3D/4D Simplex Noise 、 2D/3D textureless classic Noise 、Re-oriented 4 / 8-Point BCC Noise
//
// Reference 1: Webgl Noise -  https://github.com/ashima/webgl-noise
// Reference 2: KdotJPG New Simplex Style Gradient Noise - https://github.com/KdotJPG/New-Simplex-Style-Gradient-Noise
// Reference 3: Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
// Reference 4: noiseSimplex.cginc - https://forum.unity.com/threads/2d-3d-4d-optimised-perlin-noise-cg-hlsl-library-cginc.218372/
// ----------------------------------------------------------------------------------------------------------


#pragma once

//==================================================================================================================================
// 0. Comon
//==================================================================================================================================
// 1 / 289
#define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744f

float mod289(float x)
{
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float2 mod289(float2 x)
{
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float3 mod289(float3 x)
{
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float4 mod289(float4 x)
{
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float4 mod(float4 x, float4 y)
{
    return x - y * floor(x / y);
}

float3 mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}

// ( x*34.0 + 1.0 )*x =x*x*34.0 + x
float permute(float x)
{
    return mod289(x * x * 34.0 + x);
}

float3 permute(float3 x)
{
    return mod289(x * x * 34.0 + x);
}

float4 permute(float4 x)
{
    return mod289(x * x * 34.0 + x);
}

float3 taylorInvSqrt(float3 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float4 taylorInvSqrt(float4 r)
{
    return 1.79284291400159 - r * 0.85373472095314;
}

float2 fade(float2 t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}


float3 fade(float3 t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

//----------------------------------------------------[1.1]  2D Simplex Noise ----------------------------------------------------


float snoise(float2 v)
{
    const float4 C = float4(0.211324865405187, // (3.0-sqrt(3.0))/6.0
    0.366025403784439, // 0.5*(sqrt(3.0)-1.0)
    - 0.577350269189626, // -1.0 + 2.0 * C.x
    0.024390243902439); // 1.0 / 41.0
    // First corner
    float2 i = floor(v + dot(v, C.yy));
    float2 x0 = v - i + dot(i, C.xx);
    
    // Other corners
    float2 i1;
    i1.x = step(x0.y, x0.x);
    i1.y = 1.0 - i1.x;
    
    // x1 = x0 - i1  + 1.0 * C.xx;
    // x2 = x0 - 1.0 + 2.0 * C.xx;
    float2 x1 = x0 + C.xx - i1;
    float2 x2 = x0 + C.zz;
    
    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0))
    + i.x + float3(0.0, i1.x, 1.0));
    
    float3 m = max(0.5 - float3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
    m = m * m;
    m = m * m;
    
    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    float3 x = 2.0 * frac(p * C.www) - 1.0;
    float3 h = abs(x) - 0.5;
    float3 ox = floor(x + 0.5);
    float3 a0 = x - ox;
    
    // Normalise gradients implicitly by scaling m
    m *= taylorInvSqrt(a0 * a0 + h * h);
    
    // Compute final noise value at P
    float3 g;
    g.x = a0.x * x0.x + h.x * x0.y;
    g.y = a0.y * x1.x + h.y * x1.y;
    g.z = a0.z * x2.x + h.z * x2.y;
    return 130.0 * dot(m, g);
}