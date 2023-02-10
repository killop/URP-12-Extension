using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "深度雾 (Depth Fog)")]
    public class DepthFog : VolumeSetting
    {
        public override bool IsActive() => fogStrength.value > 0;

        [Tooltip("雾浓度")]
        public ClampedFloatParameter fogStrength = new ClampedFloatParameter(0f, 0.00f, 1f, true);
        [Tooltip("雾颜色")]
        public ColorParameter fogColor = new ColorParameter(Color.white);

        [Tooltip("雾变化率")]
        public ClampedFloatParameter fogHeightScale = new ClampedFloatParameter(10f, 1f, 50f);
        [Tooltip("雾生效开始距离")]
        public FloatParameter fogEnableDistance = new FloatParameter(10);
        [Tooltip("雾生效渐变区域")]
        public FloatParameter fogEnableDistanceArea = new FloatParameter(20);
        [Tooltip("雾基础高度,浓度为1的高度")]
        public FloatParameter fogBaseLevel = new FloatParameter(0);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/DepthFog";
        }
    }


    public class DepthFogRenderer : VolumeRenderer<DepthFog>
    {
      
        private const string PROFILER_TAG = "Depth Fog";


     

        static class ShaderContants
        {
            public static readonly int fogColorID = Shader.PropertyToID("_DepthFogColor");
            public static readonly int fogParameterID = Shader.PropertyToID("_FogParameter");
            public static readonly int fogBaseLevelID = Shader.PropertyToID("_FogBaseLevel");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetColor(ShaderContants.fogColorID, settings.fogColor.value);
            Vector4 fogParam;
            fogParam.x = settings.fogStrength.value;
            fogParam.y = settings.fogHeightScale.value;
            fogParam.z = settings.fogEnableDistance.value;
            fogParam.w = settings.fogEnableDistanceArea.value;
            m_BlitMaterial.SetVector(ShaderContants.fogParameterID, fogParam);
            m_BlitMaterial.SetFloat(ShaderContants.fogBaseLevelID, settings.fogBaseLevel.value);
            cmd.Blit(source, target, m_BlitMaterial);
            // cmd.Blit(target, source);

            cmd.EndSample(PROFILER_TAG);

        }


    }
}