using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "错位图块故障 (Image Block Glitch)")]
    public class GlitchImageBlock : VolumeSetting
    {
        public override bool IsActive() => Fade.value > 0;

        public ClampedFloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter Amount = new ClampedFloatParameter(1f, 0f, 10f);// { value = 1f };
        public FloatParameter BlockLayer1_U = new ClampedFloatParameter(9f, 0f, 50f);// { value = 9f };
        public FloatParameter BlockLayer1_V = new ClampedFloatParameter(9f, 0f, 50f);
        public FloatParameter BlockLayer2_U = new ClampedFloatParameter(5f, 0f, 50f);
        public FloatParameter BlockLayer2_V = new ClampedFloatParameter(5f, 0f, 50f);
        public FloatParameter BlockLayer1_Indensity = new ClampedFloatParameter(8f, 0f, 50f);
        public FloatParameter BlockLayer2_Indensity = new ClampedFloatParameter(4f, 0f, 50f);
        public FloatParameter RGBSplitIndensity = new ClampedFloatParameter(0.5f, 0f, 50f);


        public BoolParameter BlockVisualizeDebug = new BoolParameter(false);// { value = false };

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/ImageBlock";
        }
    }

    public sealed class GlitchImageBlockRenderer : VolumeRenderer<GlitchImageBlock>
    {
        private const string PROFILER_TAG = "GlitchImageBlock";
      

        private float TimeX = 1.0f;


     

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
            internal static readonly int Params3 = Shader.PropertyToID("_Params3");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {

            if (m_BlitMaterial == null)
                return;

            // if (!settings.IsActive())
            //     return;

            cmd.BeginSample(PROFILER_TAG);

            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(TimeX * settings.Speed.value, settings.Amount.value, settings.Fade.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector4(settings.BlockLayer1_U.value, settings.BlockLayer1_V.value, settings.BlockLayer2_U.value, settings.BlockLayer2_V.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params3, new Vector3(settings.RGBSplitIndensity.value, settings.BlockLayer1_Indensity.value, settings.BlockLayer2_Indensity.value));

            if (settings.BlockVisualizeDebug.value)
            {
                //debug
                cmd.Blit(source, target, m_BlitMaterial, 1);
            }
            else
            {
                cmd.Blit(source, target, m_BlitMaterial, 0);
            }

            // cmd.Blit(target, source);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}