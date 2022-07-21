using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "屏幕抖动故障 (Screen Shake Glitch)")]
    public class GlitchScreenShake : VolumeSetting
    {
        public override bool IsActive() => ScreenShakeIndensity.value > 0;
        public DirectionParameter ScreenShakeDirection = new DirectionParameter(Direction.Horizontal);
        public FloatParameter ScreenShakeIndensity = new ClampedFloatParameter(0f, 0f, 1f);

        public override string GetShaderName()
        {

            return "Hidden/PostProcessing/Glitch/ScreenShake";
        }
    }


    public class GlitchScreenShakeRenderer : VolumeRenderer<GlitchScreenShake>
    {
        private const string PROFILER_TAG = "GlitchScreenShake";
       

        static class ShaderIDs
        {
            internal static readonly int ScreenShakeIndensity = Shader.PropertyToID("_ScreenShake");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);


            m_BlitMaterial.SetFloat(ShaderIDs.ScreenShakeIndensity, settings.ScreenShakeIndensity.value * 0.25f);
            cmd.Blit(source, target, m_BlitMaterial, (int)settings.ScreenShakeDirection.value);

            cmd.EndSample(PROFILER_TAG);
        }

    }

}