using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ImageProcessing + "SharpenV3")]
    public class SharpenV3 : VolumeSetting
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ImageProcessing/SharpenV3";
        }
    }

    public class SharpenV3Renderer : VolumeRenderer<SharpenV3>
    {
        private const string PROFILER_TAG = "SharpenV3";
        

        private float randomFrequency;


      

        static class ShaderIDs
        {
            internal static readonly int CentralFactor = Shader.PropertyToID("_CentralFactor");
            internal static readonly int SideFactor = Shader.PropertyToID("_SideFactor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.CentralFactor, 1.0f + (3.2f * settings.Sharpness.value));
            m_BlitMaterial.SetFloat(ShaderIDs.SideFactor, 0.8f * settings.Sharpness.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }

    }

}