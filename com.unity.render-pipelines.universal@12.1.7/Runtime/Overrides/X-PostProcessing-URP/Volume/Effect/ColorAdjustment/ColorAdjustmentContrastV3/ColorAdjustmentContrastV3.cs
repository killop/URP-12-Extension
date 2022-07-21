using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "ContrastV3")]
    public class ColorAdjustmentContrastV3 : VolumeSetting
    {
        public override bool IsActive() => contrast.value != Vector4.zero;
        public Vector4Parameter contrast = new Vector4Parameter(Vector4.zero);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ColorAdjustment/ContrastV3";
        }
    }

    public class ColorAdjustmentContrastV3Renderer : VolumeRenderer<ColorAdjustmentContrastV3>
    {
        private const string PROFILER_TAG = "ColorAdjustmentContrastV3";
     

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetVector(ShaderIDs.Contrast, settings.contrast.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}