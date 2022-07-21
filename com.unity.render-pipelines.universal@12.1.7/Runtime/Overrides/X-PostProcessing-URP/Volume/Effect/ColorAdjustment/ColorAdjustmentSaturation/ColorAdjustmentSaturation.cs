using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Saturation")]
    public class ColorAdjustmentSaturation : VolumeSetting
    {
        public override bool IsActive() => saturation.value > 0;

        public FloatParameter saturation = new ClampedFloatParameter(0, 0f, 1f);// { value = 1f };

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/Saturation";
        }
    }


    public class ColorAdjustmentSaturationRenderer : VolumeRenderer<ColorAdjustmentSaturation>
    {
        private const string PROFILER_TAG = "ColorAdjustmentSaturation";
       

        static class ShaderIDs
        {
            internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Saturation, settings.saturation.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}