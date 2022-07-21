using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "波动抖动故障 (Wave Jitter Glitch)")]
    public class GlitchWaveJitter : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter jitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter RGBSplit = new ClampedFloatParameter(20f, 0f, 50f);
        public FloatParameter speed = new ClampedFloatParameter(0.25f, 0f, 1f);
        public FloatParameter amount = new ClampedFloatParameter(1f, 0f, 2f);

        public BoolParameter customResolution = new BoolParameter(false);

        public Vector2Parameter resolution = new Vector2Parameter(new Vector2(640f, 480f));
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/WaveJitter";
        }
    }

    public sealed class GlitchWaveJitterRenderer : VolumeRenderer<GlitchWaveJitter>
    {
        private const string PROFILER_TAG = "GlitchTileJitter";
      

        private float randomFrequency;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Resolution = Shader.PropertyToID("_Resolution");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            UpdateFrequency(settings);

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value
                   , settings.RGBSplit.value, settings.speed.value, settings.amount.value));
            m_BlitMaterial.SetVector(ShaderIDs.Resolution, settings.customResolution.value ? settings.resolution.value : new Vector2(Screen.width, Screen.height));

            cmd.Blit(source, target, m_BlitMaterial, (int)settings.jitterDirection.value);

            cmd.EndSample(PROFILER_TAG);
        }

        void UpdateFrequency(GlitchWaveJitter settings)
        {
            if (settings.intervalType.value == IntervalType.Random)
            {
                randomFrequency = UnityEngine.Random.Range(0, settings.frequency.value);
            }

            if (settings.intervalType.value == IntervalType.Infinite)
            {
                m_BlitMaterial.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }
    }

}