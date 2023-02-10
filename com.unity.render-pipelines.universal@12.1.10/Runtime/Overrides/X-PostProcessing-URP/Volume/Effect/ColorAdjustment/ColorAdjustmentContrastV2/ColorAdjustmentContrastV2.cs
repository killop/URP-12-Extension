using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "ContrastV2")]
    public class ColorAdjustmentContrastV2 : VolumeSetting
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1f, 5f);
        public FloatParameter ContrastFactorR = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter ContrastFactorG = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter ContrastFactorB = new ClampedFloatParameter(0f, -1f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/ContrastV2";
        }
    }

    public class ColorAdjustmentContrastV2Renderer : VolumeRenderer<ColorAdjustmentContrastV2>
    {
        private const string PROFILER_TAG = "ColorAdjustmentContrastV2";
   

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
            internal static readonly int ContrastFactorRGB = Shader.PropertyToID("_ContrastFactorRGB");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Contrast, settings.contrast.value + 1f);
            m_BlitMaterial.SetVector(ShaderIDs.ContrastFactorRGB, new Vector3(settings.ContrastFactorR.value, settings.ContrastFactorG.value, settings.ContrastFactorB.value));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}