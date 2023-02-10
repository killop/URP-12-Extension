using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "粒状模糊 (Grainy Blur)")]
    public class GrainyBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 50f);
        public IntParameter Iteration = new ClampedIntParameter(4, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 10f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/GrainyBlur";
        }
    }

    public class GrainyBlurRenderer : VolumeRenderer<GrainyBlur>
    {
        private const string PROFILER_TAG = "GrainyBlur";
     

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BufferRT = Shader.PropertyToID("_BufferRT");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            if (settings.RTDownScaling.value > 1)
            {
                int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
                int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
                cmd.GetTemporaryRT(ShaderIDs.BufferRT, RTWidth, RTHeight, 0, FilterMode.Bilinear);
                // downsample screen copy into smaller RT
                cmd.Blit(source, ShaderIDs.BufferRT);
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.BlurRadius.value / Screen.height, settings.Iteration.value));

            if (settings.RTDownScaling.value > 1)
            {
                cmd.Blit(ShaderIDs.BufferRT, target, m_BlitMaterial, 0);
            }
            else
            {
                cmd.Blit(source, target, m_BlitMaterial, 0);
            }

            // release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}