using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "BleachBypass")]
    public class ColorAdjustmentBleachBypass : VolumeSetting
    {
        public override bool IsActive() => Indensity.value > 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, 0f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/BleachBypass";
        }
    }

    public class ColorAdjustmentBleachBypassRenderer : VolumeRenderer<ColorAdjustmentBleachBypass>
    {
        private const string PROFILER_TAG = "ColorAdjustmentBleachBypass";
      

        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}