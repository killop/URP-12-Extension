using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "径向模糊 (Radial Blur)")]
    public class RadialBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(10, 2, 30);
        public FloatParameter RadialCenterX = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter RadialCenterY = new ClampedFloatParameter(0.5f, 0f, 1f);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/RadialBlur";
        }
    }


    public class RadialBlurRenderer : VolumeRenderer<RadialBlur>
    {
        private const string PROFILER_TAG = "RadialBlur";
      

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.BlurRadius.value * 0.02f, settings.Iteration.value, settings.RadialCenterX.value, settings.RadialCenterY.value));

            cmd.Blit(source, target, m_BlitMaterial, 0);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}