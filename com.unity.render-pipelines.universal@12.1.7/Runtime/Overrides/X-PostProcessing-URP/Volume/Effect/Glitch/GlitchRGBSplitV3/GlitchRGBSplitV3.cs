using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离V3 (RGB SplitV3)")]
    public class GlitchRGBSplitV3 : VolumeSetting
    {
        public override bool IsActive() => Frequency.value > 0;

        public DirectionEXParameter SplitDirection = new DirectionEXParameter(DirectionEX.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter Frequency = new ClampedFloatParameter(0f, 0.1f, 25f);
        public FloatParameter Amount = new ClampedFloatParameter(30f, 0f, 200f);
        public FloatParameter Speed = new ClampedFloatParameter(20f, 0f, 20f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/RGBSplitV3";
        }

    }


    public sealed class GlitchRGBSplitV3Renderer : VolumeRenderer<GlitchRGBSplitV3>
    {
        private const string PROFILER_TAG = "GlitchRGBSplitV3";


        private float randomFrequency;
        private int frameCount = 0;



     

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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.Frequency.value, settings.Amount.value, settings.Speed.value));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }

        void UpdateFrequency(GlitchRGBSplitV3 settings)
        {
            if (settings.intervalType.value == IntervalType.Random)
            {
                if (frameCount > settings.Frequency.value)
                {

                    frameCount = 0;
                    randomFrequency = UnityEngine.Random.Range(0, settings.Frequency.value);
                }
                frameCount++;
            }

            if (settings.intervalType.value == IntervalType.Infinite)
            {
                m_BlitMaterial.EnableKeyword("USING_Frequency_INFINITE");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("USING_Frequency_INFINITE");
            }
        }
    }

}