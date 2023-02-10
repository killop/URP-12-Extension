using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Technicolor")]
    public class ColorAdjustmentTechnicolor : VolumeSetting
    {
        public override bool IsActive() => indensity.value > 0;

        public FloatParameter indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter exposure = new ClampedFloatParameter(4f, 0f, 8f);
        public FloatParameter colorBalanceR = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceG = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceB = new ClampedFloatParameter(0.2f, 0f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/Technicolor";
        }
    }

    public class ColorAdjustmentTechnicolorRenderer : VolumeRenderer<ColorAdjustmentTechnicolor>
    {
        private const string PROFILER_TAG = "ColorAdjustmentTechnicolor";
       

        static class ShaderIDs
        {
            internal static readonly int exposure = Shader.PropertyToID("_Exposure");
            internal static readonly int colorBalance = Shader.PropertyToID("_ColorBalance");
            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.exposure, 8f - settings.exposure.value);
            m_BlitMaterial.SetVector(ShaderIDs.colorBalance, Vector3.one - new Vector3(settings.colorBalanceR.value, settings.colorBalanceG.value, settings.colorBalanceB.value));
            m_BlitMaterial.SetFloat(ShaderIDs.indensity, settings.indensity.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}