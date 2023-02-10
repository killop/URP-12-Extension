
Shader "Hidden/PostProcessing/TransitionSingleFocusSoft"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 2.5)) = 0
		_Falloff("Cutoff Falloff", Range(0, 1)) = 0
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		[HideInInspector]_FocusX("Focus Position X", float) = 0
		[HideInInspector]_FocusY("Focus Position Y", float) = 0
	}

		SubShader
	{
		Tags{ "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

			Cull Off
			ZWrite Off
			ZTest Always


			Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment frag
			#pragma shader_feature USE_TEXTURE

			 #include "./PostProcessing.hlsl"

			
			float4 _Color;
			half _Cutoff, _Falloff;
			float _FocusX, _FocusY;

			float4 frag(VaryingsDefault i) : SV_Target
			{
				float2 uv = i.uv;

				if (_MainTex_TexelSize.x > _MainTex_TexelSize.y)
				{
					float r = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
					uv.x *= r;
					_FocusX *= r;
				}
				else
				{
					float r = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
					uv.y *= r;
					_FocusY *= r;
				}

				float4 bg = _Color;

				#if USE_TEXTURE
					bg = tex2D(_Texture, i.uv);
				#endif

				float l = length(uv - float2(_FocusX, _FocusY));

				if (l > (2.5 - _Cutoff))
					return bg;

				float4 tex = GetScreenColor(i.uv);
				float t = 1 - (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
				float s = smoothstep(0, 1, t);

				return lerp(bg, tex, s);
			}
			ENDHLSL

		}
	}
}