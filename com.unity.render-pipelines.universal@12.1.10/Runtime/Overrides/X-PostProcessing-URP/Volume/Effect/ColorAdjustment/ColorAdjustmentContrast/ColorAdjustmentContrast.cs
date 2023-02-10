using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Contrast")]
    public class ColorAdjustmentContrast : VolumeSetting
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1f, 2f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/Contrast";
        }
    }

    public class ColorAdjustmentContrastRenderer : VolumeRenderer<ColorAdjustmentContrast>
    {
        private const string PROFILER_TAG = "ColorAdjustmentContrast";
     

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Contrast, settings.contrast.value + 1f);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}