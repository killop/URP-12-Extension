using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "高斯模糊 (Gaussian Blur)")]
    public class GaussianBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 5f);
        public IntParameter Iteration = new ClampedIntParameter(6, 1, 15);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 8f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/GaussianBlur";
        }
    }


    public class GaussianBlurRenderer : VolumeRenderer<GaussianBlur>
    {
        private const string PROFILER_TAG = "GaussianBlur";
       

        static class ShaderIDs
        {
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // downsample screen copy into smaller RT
            cmd.Blit(source, ShaderIDs.BufferRT1);


            for (int i = 0; i < settings.Iteration.value; i++)
            {
                // horizontal blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(settings.BlurRadius.value / Screen.width, 0, 0, 0));
                cmd.Blit(ShaderIDs.BufferRT1, ShaderIDs.BufferRT2, m_BlitMaterial);

                // vertical blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(0, settings.BlurRadius.value / Screen.height, 0, 0));
                cmd.Blit(ShaderIDs.BufferRT2, ShaderIDs.BufferRT1, m_BlitMaterial);
            }

            // Render blurred texture in blend pass
            cmd.Blit(ShaderIDs.BufferRT1, target, m_BlitMaterial);

            // release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}