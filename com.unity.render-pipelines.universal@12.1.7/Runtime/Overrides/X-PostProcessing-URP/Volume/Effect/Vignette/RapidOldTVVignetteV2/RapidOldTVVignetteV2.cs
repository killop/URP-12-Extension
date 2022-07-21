using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Vignette + "老式TV渐晕V2 (RapidOldTVVignetteV2)")]
    public class RapidOldTVVignetteV2 : VolumeSetting
    {
        public override bool IsActive() => vignetteSize.value > 0;

        public VignetteTypeParameter vignetteType = new VignetteTypeParameter(VignetteType.ClassicMode);
        public FloatParameter vignetteSize = new ClampedFloatParameter(0, 1f, 5000f);
        public FloatParameter sizeOffset = new ClampedFloatParameter(0.2f, 0f, 1f);
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f), true, true, true);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Vignette/RapidOldTVVignetteV2";
        }
    }

    public class RapidOldTVVignetteV2Renderer : VolumeRenderer<RapidOldTVVignetteV2>
    {
        private const string PROFILER_TAG = "RapidOldTVVignetteV2";
       
        static class ShaderIDs
        {
            internal static readonly int VignetteSize = Shader.PropertyToID("_VignetteSize");
            internal static readonly int SizeOffset = Shader.PropertyToID("_SizeOffset");
            internal static readonly int VignetteColor = Shader.PropertyToID("_VignetteColor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.VignetteSize, settings.vignetteSize.value);
            m_BlitMaterial.SetFloat(ShaderIDs.SizeOffset, settings.sizeOffset.value);
            if (settings.vignetteType.value == VignetteType.ColorMode)
            {
                m_BlitMaterial.SetColor(ShaderIDs.VignetteColor, settings.vignetteColor.value);
            }

            cmd.Blit(source, target, m_BlitMaterial, (int)settings.vignetteType.value);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}