using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "散景模糊 (Bokeh Blur)")]
    public class BokehBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 3f);
        public IntParameter Iteration = new ClampedIntParameter(32, 8, 128);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(2f, 1f, 10f);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/BokehBlur";
        }
    }

    public class BokehBlurRenderer : VolumeRenderer<BokehBlur>
    {
        private const string PROFILER_TAG = "BokehBlur";
     
        private Vector4 mGoldenRot = new Vector4();

        public override bool Init()
        {
            bool sc = base.Init();
            if (sc)
            {
                // Precompute rotations
                float c = Mathf.Cos(2.39996323f);
                float s = Mathf.Sin(2.39996323f);
                mGoldenRot.Set(c, s, -s, c);
            }
            return sc;
        }

        
        static class ShaderIDs
        {
            internal static readonly int GoldenRot = Shader.PropertyToID("_GoldenRot");
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // downsample screen copy into smaller RT
            cmd.Blit(source, ShaderIDs.BufferRT1);

            m_BlitMaterial.SetVector(ShaderIDs.GoldenRot, mGoldenRot);
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.Iteration.value, settings.BlurRadius.value, 1f / Screen.width, 1f / Screen.height));
            cmd.Blit(ShaderIDs.BufferRT1, target, m_BlitMaterial);

            // release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}