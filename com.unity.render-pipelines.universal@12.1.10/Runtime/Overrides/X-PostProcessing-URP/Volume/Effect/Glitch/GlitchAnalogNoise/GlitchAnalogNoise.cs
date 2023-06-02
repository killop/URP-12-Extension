using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "模拟噪点故障 (Analog Noise Glitch)")]
    public class GlitchAnalogNoise : VolumeSetting
    {
        public override bool IsActive() => NoiseFading.value > 0;

        public FloatParameter NoiseFading = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter NoiseSpeed = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter LuminanceJitterThreshold = new ClampedFloatParameter(0.8f, 0f, 1f);

        public Vector4Parameter UVRect = new Vector4Parameter(new Vector4(0f,1f,0f,1f));

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/AnalogNoise";
        }
    }


    public class GlitchAnalogNoiseRenderer : VolumeRenderer<GlitchAnalogNoise>
    {
        private const string PROFILER_TAG = "GlitchAnalogNoise";
       

        private float TimeX = 1.0f;


        

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int UVRect = Shader.PropertyToID("_UVRect");
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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.NoiseSpeed.value, settings.NoiseFading.value, settings.LuminanceJitterThreshold.value, TimeX));
            m_BlitMaterial.SetVector(ShaderIDs.UVRect, settings.UVRect.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }


    }

}