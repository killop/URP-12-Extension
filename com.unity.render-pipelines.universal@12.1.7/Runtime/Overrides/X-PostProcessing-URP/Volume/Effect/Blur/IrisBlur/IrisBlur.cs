using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public enum IrisBlurQualityLevel
    {
        High_Quality = 0,
        Normal_Quality = 1,
    }

    [System.Serializable]
    public sealed class IrisBlurQualityLevelParameter : VolumeParameter<IrisBlurQualityLevel> { public IrisBlurQualityLevelParameter(IrisBlurQualityLevel value, bool overrideState = false) : base(value, overrideState) { } }

    [VolumeComponentMenu(VolumeDefine.Blur + "光圈模糊 (Iris Blur)")]
    public class IrisBlur : VolumeSetting
    {
        public override bool IsActive() => AreaSize.value > 0;

        public IrisBlurQualityLevelParameter QualityLevel = new IrisBlurQualityLevelParameter(IrisBlurQualityLevel.Normal_Quality);
        public FloatParameter AreaSize = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0.0f, 1.0f)]
        public FloatParameter BlurRadius = new ClampedFloatParameter(1.0f, 0f, 1f);

        [Range(1, 8)]
        public IntParameter Iteration = new ClampedIntParameter(2, 1, 8);

        [Range(1, 2)]
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1.0f, 1.0f, 2.0f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Blur/IrisBlur";
        }

    }

    public sealed class IrisBlurRenderer : VolumeRenderer<IrisBlur>
    {
        private const string PROFILER_TAG = "IrisBlur";
       


    

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BlurredTex = Shader.PropertyToID("_BlurredTex");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            if (settings.Iteration == 1)
            {
                HandleOneBlitBlur(cmd, source, target);
            }
            else
            {
                HandleMultipleIterationBlur(cmd, source, target, settings.Iteration.value);
            }

            cmd.EndSample(PROFILER_TAG);
        }

        void HandleOneBlitBlur(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            // Get RT
            int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // Set Property
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.AreaSize.value, settings.BlurRadius.value));

            cmd.Blit(source, ShaderIDs.BufferRT1, m_BlitMaterial, (int)settings.QualityLevel.value);

            // Final Blit
            cmd.SetGlobalTexture(ShaderIDs.BlurredTex, ShaderIDs.BufferRT1);
            cmd.Blit(source, target, m_BlitMaterial, 2);

            // Release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
        }


        void HandleMultipleIterationBlur(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, int Iteration)
        {


            // Get RT
            int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // Set Property
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.AreaSize.value, settings.BlurRadius.value));

            RenderTargetIdentifier finalBlurID = ShaderIDs.BufferRT1;
            RenderTargetIdentifier firstID = source;
            RenderTargetIdentifier secondID = ShaderIDs.BufferRT1;
            for (int i = 0; i < Iteration; i++)
            {
                // Do Blit
                cmd.Blit(firstID, secondID, m_BlitMaterial, (int)settings.QualityLevel.value);

                finalBlurID = secondID;
                firstID = secondID;
                secondID = (secondID == ShaderIDs.BufferRT1) ? ShaderIDs.BufferRT2 : ShaderIDs.BufferRT1;
            }

            // Final Blit
            cmd.SetGlobalTexture(ShaderIDs.BlurredTex, finalBlurID);
            cmd.Blit(source, target, m_BlitMaterial, 2);

            // Release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);
        }
    }

}