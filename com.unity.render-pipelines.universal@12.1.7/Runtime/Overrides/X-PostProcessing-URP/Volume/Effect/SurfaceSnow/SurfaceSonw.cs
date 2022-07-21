using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "顶面积雪 (SurfaceSnow)")]
    public class SurfaceSnow : VolumeSetting
    {
        public override bool IsActive() => snowStrength.value > 0;

        [Tooltip("积雪强度")]
        public ClampedFloatParameter snowStrength = new ClampedFloatParameter(0, 0, 1);
        [Tooltip("积雪贴图")]
        public TextureParameter snowTexture = new TextureParameter(null);
        [Tooltip("倾斜控制")]
        public ClampedFloatParameter snowBias = new ClampedFloatParameter(0.8f, 0, 1);
        [Tooltip("积雪贴图尺寸")]
        public FloatParameter snowSize = new FloatParameter(1);
        public ClampedFloatParameter debugValue = new ClampedFloatParameter(1, 0, 10);
        public IntParameter sampleOffset = new IntParameter(0);
        public MinIntParameter blurInt = new MinIntParameter(1, 1);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/SurfaceSnow";
        }
    }

    public class SurfaceSnowRenderer : VolumeRenderer<SurfaceSnow>
    {
        
        private const string PROFILER_TAG = "Surface Snow";
       

        static class ShaderContants
        {
            public static readonly int snowTextureID = Shader.PropertyToID("_SnowTexture");
            public static readonly int snowStrengthID = Shader.PropertyToID("_SnowStrength");
            public static readonly int snowBiasID = Shader.PropertyToID("_SnowBias");
            public static readonly int snowTexelSizeID = Shader.PropertyToID("_TexelSize");
            public static readonly int snowSizeID = Shader.PropertyToID("_SnowSize");
            public static readonly int debugValueID = Shader.PropertyToID("_DebugValue");
            public static readonly int sampleOffsetID = Shader.PropertyToID("_SampleOffset");
            public static readonly int snowBlurTexID = Shader.PropertyToID("_SnowBlurTex");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null) return;
            //var setting = VolumeManager.instance.stack.GetComponent<SurfaceSnowSetting>();			

            //if(!setting.IsActive())return;
            cmd.BeginSample(PROFILER_TAG);

            RenderTexture temp1 = RenderTexture.GetTemporary(Screen.width / settings.blurInt.value, Screen.height / settings.blurInt.value);



            Vector2 texelSize;
            texelSize.x = (1f / Screen.width);
            texelSize.y = (1f / Screen.height);
            m_BlitMaterial.SetVector(ShaderContants.snowTexelSizeID, texelSize);
            m_BlitMaterial.SetTexture(ShaderContants.snowTextureID, settings.snowTexture.value);
            m_BlitMaterial.SetFloat(ShaderContants.snowStrengthID, settings.snowStrength.value);
            m_BlitMaterial.SetFloat(ShaderContants.snowBiasID, settings.snowBias.value);
            m_BlitMaterial.SetFloat(ShaderContants.snowSizeID, settings.snowSize.value);
            m_BlitMaterial.SetFloat(ShaderContants.debugValueID, settings.debugValue.value);
            m_BlitMaterial.SetInt(ShaderContants.sampleOffsetID, settings.sampleOffset.value);

            cmd.Blit(source, temp1, m_BlitMaterial, 0);

            m_BlitMaterial.SetTexture(ShaderContants.snowBlurTexID, temp1);

            cmd.Blit(source, target, m_BlitMaterial, 1);
            //cmd.Blit(target,source);

            RenderTexture.ReleaseTemporary(temp1);


            cmd.EndSample(PROFILER_TAG);
        }
    }

}