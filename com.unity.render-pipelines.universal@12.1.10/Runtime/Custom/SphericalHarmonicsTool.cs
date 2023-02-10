using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalHarmonicsTool 
{
	private Color rightColor;
	private Color leftColor;
	private Color topColor;
	private Color bottomColor;
	private Color frontColor;
	private Color backColor;
	private float currSixSideExposure = 1;
	private float currSixSideDegree = 0;

	public Cubemap currCubemap;
	private float currCubemapExposure = 1;
	private float currCubemapDegree = 0;

	public Vector4[] result;

	public Vector4[] GetSHParam(Color right,Color left,Color top,Color bottom,Color front,Color back,float sixSideExposure,float sixSideDegree,
		 Cubemap cubemap,float cubemapExposure, float cubemapDegree)
	{
		if(right != rightColor || left != leftColor || top != topColor ||
			bottom != bottomColor || front != frontColor || back != backColor ||
			sixSideExposure != currSixSideExposure || sixSideDegree != currSixSideDegree ||
			cubemap != currCubemap || cubemapExposure != currCubemapExposure || cubemapDegree != currCubemapDegree)
		{
			rightColor = right;
			leftColor = left;
			topColor = top;
			bottomColor = bottom;
			frontColor = front;
			backColor = back;
			currSixSideExposure = sixSideExposure;
			currSixSideDegree = sixSideDegree;
			currCubemap = cubemap;
			currCubemapExposure = cubemapExposure;
			currCubemapDegree = cubemapDegree;

			Vector3[] sh = new Vector3[9];
			for (int i = 0; i < 9; i++)
			{
				sh[i] = Vector3.zero;
			}

			ComputeSixSide(ref sh);

			if(currCubemap)
				ComputeCubemap(ref sh);

			result = new Vector4[7];
			SphericalHarmonicsCommon.SetSHConstants(ref sh, ref result);
			return result;
		}
		else
		{
			return result;
		}		
	}

	void ComputeSixSide(ref Vector3[] sh)
	{
		int size = 16;

		Color[] colors = new Color[6];
		colors[0] = rightColor;
		colors[1] = leftColor;
		colors[2] = topColor;
		colors[3] = bottomColor;
		colors[4] = frontColor;
		colors[5] = backColor;

		//cycle on all 6 faces of the cubemap
		for (int face = 0; face < 6; ++face)
		{
			//cycle all the texels
			for (int texel = 0; texel < size * size; ++texel)
			{
				float u = (texel % size) / (float)size;
				float v = ((int)(texel / size)) / (float)size;

				ComputeDifferential(colors[face].linear, face, u, v, currSixSideDegree, currSixSideExposure, size, ref sh);
			}
		}
	}
	void ComputeCubemap(ref Vector3[] sh)
	{
		Color[] input_face;
		int cubemapSize = currCubemap.width;

		//cycle on all 6 faces of the cubemap
		for (int face = 0; face < 6; ++face)
		{
			input_face = currCubemap.GetPixels((CubemapFace)face);

			//cycle all the texels
			for (int texel = 0; texel < cubemapSize * cubemapSize; ++texel)
			{
				float u = (texel % cubemapSize) / (float)cubemapSize;
				float v = ((int)(texel / cubemapSize)) / (float)cubemapSize;

				Color radiance = input_face[texel];

				ComputeDifferential(radiance, face, u, v, currCubemapDegree, currCubemapExposure, cubemapSize, ref sh);
			}
		}
	}
	void ComputeDifferential(Color radiance,int face,float u,float v,float degree,float exposure,int size,ref Vector3[] sh)
	{
		radiance *= exposure;
		//get the direction vector
		Vector3 dir = SphericalHarmonicsCommon.DirectionFromCubemapTexel(face, u, v);

		float alpha = degree * Mathf.PI / 180.0f;
		float sina = Mathf.Sin(alpha);
		float cosa = Mathf.Cos(alpha);

		Vector3 oldDir = dir;

		dir.x = oldDir.x * cosa + oldDir.z * (-sina);
		dir.z = oldDir.x * sina + oldDir.z * cosa;

		dir = dir.normalized;

		//compute the differential solid angle
		float d_omega = SphericalHarmonicsCommon.DifferentialSolidAngle(size, u, v);

		float[] shhh = new float[9];
		SphericalHarmonicsCommon.SHEvalDirection9(dir, ref shhh);

		//cycle for 9 coefficients
		for (int c = 0; c < 9; ++c)
		{
			//compute shperical harmonic
			//float shhh = SphericalHarmonicsBasis.Eval[c](dir);
			sh[c][0] += radiance.r * d_omega * shhh[c];
			sh[c][1] += radiance.g * d_omega * shhh[c];
			sh[c][2] += radiance.b * d_omega * shhh[c];
		}
	}

}
