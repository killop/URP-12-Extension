using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离V4 (RGB SplitV4)")]
    public class GlitchRGBSplitV4 : VolumeSetting
    {
        public override bool IsActive() => indensity.value != 0;

        public FloatParameter indensity = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter speed = new ClampedFloatParameter(10f, 0f, 100f);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/RGBSplitV4";
        }
    }


    public sealed class GlitchRGBSplitV4Renderer : VolumeRenderer<GlitchRGBSplitV4>
    {
        private const string PROFILER_TAG = "GlitchRGBSplitV4";
       

        private float TimeX = 1.0f;


       

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.indensity.value * 0.1f, Mathf.Floor(TimeX * settings.speed.value)));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}