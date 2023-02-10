using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{

    [VolumeComponentMenu(VolumeDefine.Blur + "光圈模糊V2 (Iris BlurV2)")]
    public class IrisBlurV2 : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;

        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 3f);
        public IntParameter Iteration = new ClampedIntParameter(60, 8, 128);
        public FloatParameter centerOffsetX = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter centerOffsetY = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter AreaSize = new ClampedFloatParameter(8f, 0f, 50f);
        public BoolParameter showPreview = new BoolParameter(false);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/IrisBlurV2";
        }

    }

    public sealed class IrisBlurV2Renderer : VolumeRenderer<IrisBlurV2>
    {
        private const string PROFILER_TAG = "IrisBlurV2";
   
        private Vector4 mGoldenRot = new Vector4();

        public override bool Init()
        {
           

           bool success=  base.Init();
            if (success)
            {
                float c = Mathf.Cos(2.39996323f);
                float s = Mathf.Sin(2.39996323f);
                mGoldenRot.Set(c, s, -s, c);
            }
            return success;
            // Precompute rotations

        }

        static class ShaderIDs
        {
            internal static readonly int GoldenRot = Shader.PropertyToID("_GoldenRot");
            internal static readonly int Gradient = Shader.PropertyToID("_Gradient");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetVector(ShaderIDs.GoldenRot, mGoldenRot);
            m_BlitMaterial.SetVector(ShaderIDs.Gradient, new Vector3(settings.centerOffsetX.value, settings.centerOffsetY.value, settings.AreaSize.value * 0.1f));
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.Iteration.value, settings.BlurRadius.value, 1f / Screen.width, 1f / Screen.height));

            cmd.Blit(source, target, m_BlitMaterial, settings.showPreview.value ? 1 : 0);

            cmd.EndSample(PROFILER_TAG);
        }


    }

}