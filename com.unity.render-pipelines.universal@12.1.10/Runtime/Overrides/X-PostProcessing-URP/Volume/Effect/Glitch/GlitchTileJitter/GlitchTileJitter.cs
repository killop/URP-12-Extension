using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "图块抖动故障 (Tile Jitter Glitch)")]
    public class GlitchTileJitter : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter jitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);

        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);

        public DirectionParameter splittingDirection = new DirectionParameter(Direction.Vertical);

        public FloatParameter splittingNumber = new ClampedFloatParameter(5f, 0f, 20f);
        public FloatParameter amount = new ClampedFloatParameter(10f, 0f, 100f);
        public FloatParameter speed = new ClampedFloatParameter(0.35f, 0f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/TileJitter";
        }
    }

    public class GlitchTileJitterRenderer : VolumeRenderer<GlitchTileJitter>
    {
        private const string PROFILER_TAG = "GlitchTileJitter";
        

        private float randomFrequency;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            UpdateFrequency(settings);

            if (settings.jitterDirection.value == Direction.Horizontal)
            {
                m_BlitMaterial.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.splittingNumber.value, settings.amount.value, settings.speed.value * 100f,
                 settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value));

            cmd.Blit(source, target, m_BlitMaterial, settings.splittingDirection.value == Direction.Horizontal ? 0 : 1);

            cmd.EndSample(PROFILER_TAG);
        }

        void UpdateFrequency(GlitchTileJitter settings)
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