using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "屏幕跳跃故障 (Screen Jump Glitch)")]
    public class GlitchScreenJump : VolumeSetting
    {
        public override bool IsActive() => ScreenJumpIndensity.value > 0;

        public DirectionParameter ScreenJumpDirection = new DirectionParameter(Direction.Vertical);

        public FloatParameter ScreenJumpIndensity = new ClampedFloatParameter(0f, 0f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/ScreenJump";
        }
    }

    public class GlitchScreenJumpRenderer : VolumeRenderer<GlitchScreenJump>
    {
        private const string PROFILER_TAG = "GlitchScreenJump";
      

        float ScreenJumpTime;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            ScreenJumpTime += Time.deltaTime * settings.ScreenJumpIndensity.value * 9.8f;
            Vector2 ScreenJumpVector = new Vector2(settings.ScreenJumpIndensity.value, ScreenJumpTime);

            m_BlitMaterial.SetVector(ShaderIDs.Params, ScreenJumpVector);

            cmd.Blit(source, target, m_BlitMaterial, (int)settings.ScreenJumpDirection.value);

            cmd.EndSample(PROFILER_TAG);
        }

    }

}