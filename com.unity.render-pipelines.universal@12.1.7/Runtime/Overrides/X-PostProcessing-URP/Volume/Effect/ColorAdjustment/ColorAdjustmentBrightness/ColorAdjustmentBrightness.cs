using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Brightness")]
    public class ColorAdjustmentBrightness : VolumeSetting
    {
        public override bool IsActive() => Indensity.value != 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, -0.9f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/Brightness";
        }
    }


    public class ColorAdjustmentBrightnessRenderer : VolumeRenderer<ColorAdjustmentBrightness>
    {
        private const string PROFILER_TAG = "ColorAdjustmentBrightness";
      

        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Brightness");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value + 1f);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}