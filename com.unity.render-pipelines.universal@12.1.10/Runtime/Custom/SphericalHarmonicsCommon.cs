using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalHarmonicsCommon : MonoBehaviour
{
	public static void SHEvalDirection9(Vector3 dir, ref float[] outsh)
	{
		// Core i7 920, VS2008 Release, FPU code:

		// 114 clocks
		//D3DXSHEvalDirection (outsh, 3, &D3DXVECTOR3(x,y,z));

		/*
		// 314 clocks
		// Reference implementation from Stupid Spherical Harmonics Tricks
		// http://www.ppsloan.org/publications/StupidSH36.pdf
		const float kSqrtPI = sqrtf(kPI);
		const float kSqrt3 = sqrtf(3.0f);
		const float kSqrt15 = sqrtf(15.0f);
		outsh[0] = 1.0f / (2.0f * kSqrtPI);
		outsh[1] = -(kSqrt3 * y) / (2.0f * kSqrtPI);
		outsh[2] = (kSqrt3 * z) / (2.0f * kSqrtPI);
		outsh[3] = -(kSqrt3 * x) / (2.0f * kSqrtPI);
		outsh[4] = (kSqrt15 * x * y) / (2.0f * kSqrtPI);
		outsh[5] = -(kSqrt15 * y * z) / (2.0f * kSqrtPI);
		outsh[6] = (sqrtf(5.0f) * (3.0f*z*z-1.0f)) / (4.0f * kSqrtPI);
		outsh[7] = -(kSqrt15 * x * z) / (2.0f * kSqrtPI);
		outsh[8] = (kSqrt15 * (x*x - y*y)) / (4.0f * kSqrtPI);
		*/

		// 86 clocks
		// Make sure all constants are never computed at runtime
		const float kInv2SqrtPI = 0.28209479177387814347403972578039f; // 1 / (2*sqrt(kPI))
		const float kSqrt3Div2SqrtPI = 0.48860251190291992158638462283835f; // sqrt(3) / (2*sqrt(kPI))
		const float kSqrt15Div2SqrtPI = 1.0925484305920790705433857058027f; // sqrt(15) / (2*sqrt(kPI))
		const float k3Sqrt5Div4SqrtPI = 0.94617469575756001809268107088713f; // 3 * sqrtf(5) / (4*sqrt(kPI))
		const float kSqrt15Div4SqrtPI = 0.54627421529603953527169285290135f; // sqrt(15) / (4*sqrt(kPI))
		const float kOneThird = 0.3333333333333333333333f; // 1.0/3.0
		outsh[0] = kInv2SqrtPI;
		outsh[1] = -dir.y * kSqrt3Div2SqrtPI;
		outsh[2] = dir.z * kSqrt3Div2SqrtPI;
		outsh[3] = -dir.x * kSqrt3Div2SqrtPI;
		outsh[4] = dir.x * dir.y * kSqrt15Div2SqrtPI;
		outsh[5] = -dir.y * dir.z * kSqrt15Div2SqrtPI;
		outsh[6] = (dir.z * dir.z - kOneThird) * k3Sqrt5Div4SqrtPI;
		outsh[7] = -dir.x * dir.z * kSqrt15Div2SqrtPI;
		outsh[8] = (dir.x * dir.x - dir.y * dir.y) * kSqrt15Div4SqrtPI;
	}
	public static void SHEvalDirectionalLight9(Vector3 dir,Color color,ref Vector3[] coeff)
	{
		// Core i7 920, VS2008 Release, FPU code:

		// 397 clocks
		//D3DXSHEvalDirectionalLight (3, &D3DXVECTOR3(x,y,z), colorR, colorG, colorB, outR, outG, outB);

		// 300 clocks
		float[] sh = new float[9];
		SHEvalDirection9(dir, ref sh);
		// Normalization factor from http://www.ppsloan.org/publications/StupidSH36.pdf
		const float kNormalization = 2.9567930857315701067858823529412f; // 16*kPI/17
		float rscale = color.r * kNormalization;
		float gscale = color.g * kNormalization;
		float bscale = color.b * kNormalization;
		for (int i = 0; i < 9; ++i)
		{
			float c = sh[i];
			coeff[i].x = c * rscale;
			coeff[i].y = c * gscale;
			coeff[i].z = c * bscale;
		}
	}
	public static void AddLightToSH(Vector3 lightDir,Color color,ref Vector3[] sh)
	{
		Vector3[] coeff = new Vector3[9];	

		SHEvalDirectionalLight9(lightDir, color, ref coeff);
		for (int i = 0; i < 9; ++i)
		{
			sh[i].x += coeff[i].x;
			sh[i].y += coeff[i].y;
			sh[i].z += coeff[i].z;
		}
	}
	public static void SetSHConstants(ref Vector3[] sh,ref Vector4[] result)
	{
		Vector4[] vCoeff = new Vector4[3];
 		float s_fSqrtPI = (float)Mathf.Sqrt(3.14159274f);
		float fC0 = 1.0f / (2.0f * s_fSqrtPI);
		float fC1 = (float)Mathf.Sqrt(3.0f) / (3.0f * s_fSqrtPI);
		float fC2 = (float)Mathf.Sqrt(15.0f) / (8.0f * s_fSqrtPI);
		float fC3 = (float)Mathf.Sqrt(5.0f) / (16.0f * s_fSqrtPI);
		float fC4 = 0.5f * fC2;

		int iC;
		for(iC=0; iC<3; iC++ )
		{
			vCoeff[iC].x = -fC1* sh[3][iC];
			vCoeff[iC].y = -fC1* sh[1][iC];
			vCoeff[iC].z =  fC1* sh[2][iC];
			vCoeff[iC].w =  fC0* sh[0][iC] - fC3* sh[6][iC];
		}

		result[0]= vCoeff[0];
		result[1] = vCoeff[1];
		result[2] = vCoeff[2];		

		for(iC=0; iC<3; iC++ )
		{
			vCoeff[iC].x =      fC2* sh[4][iC];
			vCoeff[iC].y =     -fC2* sh[5][iC];
			vCoeff[iC].z = 3.0f* fC3* sh[6][iC];
			vCoeff[iC].w =     -fC2* sh[7][iC];
		}

		result[3] = vCoeff[0];
		result[4] = vCoeff[1];
		result[5] = vCoeff[2];		

		vCoeff[0].x = fC4* sh[8][0];
		vCoeff[0].y = fC4* sh[8][1];
		vCoeff[0].z = fC4* sh[8][2];
		vCoeff[0].w = 1.0f;

		result[6] = vCoeff[0];
	}
	public static float AreaElement(float x, float y)
	{
		return Mathf.Atan2(x * y, Mathf.Sqrt(x * x + y * y + 1));
	}
	public static float DifferentialSolidAngle(int textureSize, float U, float V)
	{
		float inv = 1.0f / textureSize;
		float u = 2.0f * (U + 0.5f * inv) - 1;
		float v = 2.0f * (V + 0.5f * inv) - 1;
		float x0 = u - inv;
		float y0 = v - inv;
		float x1 = u + inv;
		float y1 = v + inv;
		return AreaElement(x0, y0) - AreaElement(x0, y1) - AreaElement(x1, y0) + AreaElement(x1, y1);
	}
	public static Vector3 DirectionFromCubemapTexel(int face, float u, float v)
	{
		Vector3 dir = Vector3.zero;

		switch (face)
		{
			case 0: //+X
				dir.x = 1;
				dir.y = v * -2.0f + 1.0f;
				dir.z = u * -2.0f + 1.0f;
				break;

			case 1: //-X
				dir.x = -1;
				dir.y = v * -2.0f + 1.0f;
				dir.z = u * 2.0f - 1.0f;
				break;

			case 2: //+Y
				dir.x = u * 2.0f - 1.0f;
				dir.y = 1.0f;
				dir.z = v * 2.0f - 1.0f;
				break;

			case 3: //-Y
				dir.x = u * 2.0f - 1.0f;
				dir.y = -1.0f;
				dir.z = v * -2.0f + 1.0f;
				break;

			case 4: //+Z
				dir.x = u * 2.0f - 1.0f;
				dir.y = v * -2.0f + 1.0f;
				dir.z = 1;
				break;

			case 5: //-Z
				dir.x = u * -2.0f + 1.0f;
				dir.y = v * -2.0f + 1.0f;
				dir.z = -1;
				break;
		}

		//dir.x = -dir.x;
		//dir.y = -dir.y;

		return dir.normalized;
	}
	public static RenderTextureFormat ConvertRenderFormat(TextureFormat input_format)
	{
		RenderTextureFormat output_format = RenderTextureFormat.ARGB32;

		switch (input_format)
		{
			case TextureFormat.RGBA32:
				output_format = RenderTextureFormat.ARGB32;
				break;

			case TextureFormat.RGBAHalf:
				output_format = RenderTextureFormat.ARGBHalf;
				break;

			case TextureFormat.RGBAFloat:
				output_format = RenderTextureFormat.ARGBFloat;
				break;

			default:
				string format_string = System.Enum.GetName(typeof(TextureFormat), input_format);
				int format_int = (int)System.Enum.Parse(typeof(RenderTextureFormat), format_string);
				output_format = (RenderTextureFormat)format_int;
				break;
		}

		return output_format;
	}
}