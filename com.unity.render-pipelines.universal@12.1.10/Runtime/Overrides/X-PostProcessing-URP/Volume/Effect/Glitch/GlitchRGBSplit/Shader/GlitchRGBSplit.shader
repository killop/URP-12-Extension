

Shader "Hidden/PostProcessing/Glitch/RGBSplit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }


    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 _Params;
    half3 _Params2;

    #define _Fading _Params.x
    #define _Amount _Params.y
    #define _Speed _Params.z
    #define _CenterFading _Params.w
    #define _TimeX _Params2.x
    #define _AmountR _Params2.y
    #define _AmountB _Params2.z



    half4 Frag_Horizontal(VaryingsDefault i): SV_Target
    {
        float2 uv = i.uv;
        half time = _TimeX * 6 * _Speed;
        half splitAmount = (1.0 + sin(time)) * 0.5;
        splitAmount *= 1.0 + sin(time * 2) * 0.5;
        splitAmount = pow(splitAmount, 3.0);
        splitAmount *= 0.05;
        float distance = length(uv - float2(0.5, 0.5));
        splitAmount *= _Fading * _Amount;
        splitAmount *= lerp(1, distance, _CenterFading);

        half3 colorR = GetScreenColor(float2(uv.x + splitAmount * _AmountR, uv.y));//SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x + splitAmount * _AmountR, uv.y)).rgb;
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half3 colorB = GetScreenColor(float2(uv.x - splitAmount * _AmountB, uv.y));//SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - splitAmount * _AmountB, uv.y)).rgb;

        half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
        half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);
        // return half4(uv, 0, 1);
        // return sceneColor;
        return half4(finalColor, 1.0);
    }


    half4 Frag_Vertical(VaryingsDefault i): SV_Target
    {

        float2 uv = i.uv.xy;
        half time = _TimeX * 6 * _Speed;
        half splitAmount = (1.0 + sin(time)) * 0.5;
        splitAmount *= 1.0 + sin(time * 2) * 0.5;
        splitAmount = pow(splitAmount, 3.0);
        splitAmount *= 0.05;
        float distance = length(uv - float2(0.5, 0.5));
        splitAmount *= _Fading * _Amount;
        splitAmount *= _Fading * _Amount;

        half3 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x, uv.y + splitAmount * _AmountR)).rgb;
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        half3 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x, uv.y - splitAmount * _AmountB)).rgb;

        half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
        half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);

        return half4(finalColor, 1.0);
    }

    half4 Frag_Horizontal_Vertical(VaryingsDefault i): SV_Target
    {

        float2 uv = i.uv.xy;
        half time = _TimeX * 6 * _Speed;
        half splitAmount = (1.0 + sin(time)) * 0.5;
        splitAmount *= 1.0 + sin(time * 2) * 0.5;
        splitAmount = pow(splitAmount, 3.0);
        splitAmount *= 0.05;
        float distance = length(uv - float2(0.5, 0.5));
        splitAmount *= _Fading * _Amount;
        splitAmount *= _Fading * _Amount;

        float splitAmountR = splitAmount * _AmountR;
        float splitAmountB = splitAmount * _AmountB;

        half3 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x + splitAmountR, uv.y + splitAmountR)).rgb;
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        half3 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - splitAmountB, uv.y - splitAmountB)).rgb;

        half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
        half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);

        return half4(finalColor, 1.0);
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
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_Horizontal

            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_Vertical

            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag_Horizontal_Vertical

            ENDHLSL

        }
    }
}
