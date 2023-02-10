using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离 (RGB Split)")]
    public class GlitchRGBSplit : VolumeSetting
    {
        public override bool IsActive() => Fading.value > 0;

        [Tooltip("强度")]
        public FloatParameter Fading = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0f, 0f, 10f);
        public ClampedFloatParameter CenterFading = new ClampedFloatParameter(0f, 0f, 1f);
        // [Tooltip("")]
        public ClampedFloatParameter AmountR = new ClampedFloatParameter(1f, 0f, 5f);

        // [Tooltip("强度")]
        public ClampedFloatParameter AmountB = new ClampedFloatParameter(1f, 0f, 5f);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/RGBSplit";
        }
    }


    public sealed class GlitchRGBSplitRenderer : VolumeRenderer<GlitchRGBSplit>
    {
        private const string PROFILER_TAG = "GlitchRGBSplit";
        

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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.Fading.value, settings.Amount.value, settings.Speed.value, settings.CenterFading.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector3(TimeX, settings.AmountR.value, settings.AmountB.value));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}