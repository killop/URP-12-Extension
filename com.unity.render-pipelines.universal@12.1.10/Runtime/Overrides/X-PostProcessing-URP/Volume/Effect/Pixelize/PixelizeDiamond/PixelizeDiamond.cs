using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "菱形像素化 (PixelizeDiamond)")]
    public class PixelizeDiamond : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1.0f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Pixelate/PixelizeDiamond";
        }
    }

    public class PixelizeDiamondRenderer : VolumeRenderer<PixelizeDiamond>
    {
        private const string PROFILER_TAG = "PixelizeDiamond";
       

        static class ShaderIDs
        {
            internal static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.PixelSize, settings.pixelSize.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }


    }
}

