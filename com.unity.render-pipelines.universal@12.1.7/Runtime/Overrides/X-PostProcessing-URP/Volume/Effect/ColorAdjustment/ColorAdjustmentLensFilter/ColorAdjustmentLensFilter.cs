using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "镜头滤光 (LensFilter)")]
    public class ColorAdjustmentLensFilter : VolumeSetting
    {
        public override bool IsActive() => Indensity.value > 0;

        public FloatParameter Indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter LensColor = new ColorParameter(new Color(1.0f, 1.0f, 0.1f, 1), true, true, true);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/LensFilter";
        }
    }


    public class ColorAdjustmentLensFilterRenderer : VolumeRenderer<ColorAdjustmentLensFilter>
    {
        private const string PROFILER_TAG = "ColorAdjustmentLensFilter";
      

        static class ShaderIDs
        {
            internal static readonly int LensColor = Shader.PropertyToID("_LensColor");
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value);
            m_BlitMaterial.SetColor(ShaderIDs.LensColor, settings.LensColor.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}