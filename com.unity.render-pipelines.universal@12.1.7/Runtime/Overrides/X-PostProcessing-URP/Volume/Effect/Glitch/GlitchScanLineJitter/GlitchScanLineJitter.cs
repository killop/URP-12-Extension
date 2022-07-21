using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "扫描线抖动故障 (Scane Line Jitter Glitch)")]
    public class GlitchScanLineJitter : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public FloatParameter JitterIndensity = new ClampedFloatParameter(0.1f, 0f, 1f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/ScanLineJitter";
        }
    }

    public class GlitchScanLineJitterRenderer : VolumeRenderer<GlitchScanLineJitter>
    {
        private const string PROFILER_TAG = "GlitchScanLineJitter";
      

        private float randomFrequency;


      

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int JitterIndensity = Shader.PropertyToID("_ScanLineJitter");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            // if (!settings.IsActive())
            //     return;


            cmd.BeginSample(PROFILER_TAG);

            UpdateFrequency(settings);

            float displacement = 0.005f + Mathf.Pow(settings.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - settings.JitterIndensity.value * 1.2f);

            //sheet.properties.SetVector(ShaderIDs.Params, new Vector3(settings.amount, settings.speed, );

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(displacement, threshold, settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value));

            cmd.Blit(source, target, m_BlitMaterial, (int)settings.JitterDirection.value);
            // cmd.Blit(target, source);

            cmd.EndSample(PROFILER_TAG);
        }

        void UpdateFrequency(GlitchScanLineJitter settings)
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