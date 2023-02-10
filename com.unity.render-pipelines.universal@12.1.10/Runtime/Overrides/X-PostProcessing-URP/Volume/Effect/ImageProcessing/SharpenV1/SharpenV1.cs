using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ImageProcessing + "SharpenV1")]
    public class SharpenV1 : VolumeSetting
    {
        public override bool IsActive() => Strength.value > 0;
        public FloatParameter Strength = new ClampedFloatParameter(0f, 0f, 5f);
        public FloatParameter Threshold = new ClampedFloatParameter(0.1f, 0f, 1);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/ImageProcessing/SharpenV1";
        }
    }

    public class SharpenV1Renderer : VolumeRenderer<SharpenV1>
    {
        private const string PROFILER_TAG = "SharpenV1";
       
        private float randomFrequency;


       

        static class ShaderIDs
        {
            internal static readonly int Strength = Shader.PropertyToID("_Strength");
            internal static readonly int Threshold = Shader.PropertyToID("_Threshold");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Strength, settings.Strength.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Threshold, settings.Threshold.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }

    }

}