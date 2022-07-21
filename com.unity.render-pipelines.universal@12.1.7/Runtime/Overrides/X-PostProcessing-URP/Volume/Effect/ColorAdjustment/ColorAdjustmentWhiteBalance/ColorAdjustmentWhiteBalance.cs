using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "白平衡 (WhiteBalance)")]
    public class ColorAdjustmentWhiteBalance : VolumeSetting
    {
        public override bool IsActive() => temperature.value != 0;

        public FloatParameter temperature = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter tint = new ClampedFloatParameter(0f, -1f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/WhiteBalance";
        }
    }

    public class ColorAdjustmentWhiteBalanceRenderer : VolumeRenderer<ColorAdjustmentWhiteBalance>
    {
        private const string PROFILER_TAG = "ColorAdjustmentWhiteBalance";
       

        static class ShaderIDs
        {

            internal static readonly int Temperature = Shader.PropertyToID("_Temperature");
            internal static readonly int Tint = Shader.PropertyToID("_Tint");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Temperature, settings.temperature.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Tint, settings.tint.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}