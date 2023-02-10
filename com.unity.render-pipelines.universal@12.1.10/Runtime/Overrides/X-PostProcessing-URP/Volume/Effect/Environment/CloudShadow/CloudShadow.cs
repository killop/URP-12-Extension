using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "云投影 (Cloud Shadow)")]
    public class CloudShadow : VolumeSetting
    {
        public override bool IsActive() => shadowStrength.value > 0;

        [Tooltip("阴影强度")]
        public ClampedFloatParameter shadowStrength = new ClampedFloatParameter(0f, 0, 2);
        //调整参数
        [Tooltip("阴影照射方式")]
        public BoolParameter shadowForLightDirection = new BoolParameter(false);
        public TextureParameter cloudTexture = new TextureParameter(null);
        public FloatParameter cloudScale = new FloatParameter(1000.0f);
        public ColorParameter shadowColor = new ColorParameter(Color.black);

        [Tooltip("云采样最小值")]
        public ClampedFloatParameter shadowCutoffMin = new ClampedFloatParameter(0.5f, 0, 1);
        [Tooltip("云采样最大值")]
        public ClampedFloatParameter shadowCutoffMax = new ClampedFloatParameter(0.6f, 0, 1);
        [Tooltip("最大生效距离,阴影强度线性减弱")]
        public FloatParameter maxDistance = new FloatParameter(100.0f);
        [Tooltip("云的移动方向和速度")]
        public Vector2Parameter windSpeedDirection = new Vector2Parameter(Vector2.one);
        [Tooltip("云层高度")]
        public IntParameter cloudHeight = new IntParameter(1000);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/CloudShadow";
        }
    }

    public class CloudShadowRenderer : VolumeRenderer<CloudShadow>
    {

        private const string PROFILER_TAG = "Cloud Shadow";





        static class ShaderContants
        {
            public static readonly int cloudShadowModeID = Shader.PropertyToID("_CloudShadowMode");
            public static readonly int cloudTextureID = Shader.PropertyToID("_CloudTex");
            public static readonly int cloudShadowColorID = Shader.PropertyToID("_CloudShadowColor");
            public static readonly int cloudTilingID = Shader.PropertyToID("_CloudTiling");
            public static readonly int windFactorID = Shader.PropertyToID("_WindFactor");
            public static readonly int cloudHeightID = Shader.PropertyToID("_CloudHeight");
        }
        public override bool IsActive()
        {
            bool active= base.IsActive();
            var hasValue = URPAdaptivePerformance.GetCloudShadowEnable(active, out var result);
            if (hasValue && !result)
            {
                return false;
            }
            return active; 
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            if (settings.shadowForLightDirection.value)
            {
                m_BlitMaterial.SetInt(ShaderContants.cloudShadowModeID, 1);
            }
            else
            {
                m_BlitMaterial.SetInt(ShaderContants.cloudShadowModeID, 0);
            }
            m_BlitMaterial.SetTexture(ShaderContants.cloudTextureID, settings.cloudTexture.value);
            m_BlitMaterial.SetColor(ShaderContants.cloudShadowColorID, settings.shadowColor.value);
            Vector4 cloud;// = new Vector4(m_Settings.cloudScale.x, m_Settings.cloudScale.y,m_Settings.shodowStrength,m_Settings.maxDistance);

            cloud.x = settings.cloudScale.value;
            cloud.y = settings.cloudScale.value;
            cloud.z = settings.shadowStrength.value;
            cloud.w = settings.maxDistance.value;

            m_BlitMaterial.SetVector(ShaderContants.cloudTilingID, cloud);
            Vector4 wind;// = new Vector4(m_Settings.windSpeedDirection.x, m_Settings.windSpeedDirection.y, m_Settings.shodowCutoffMin, m_Settings.shodowCutoffMax);

            wind.x = settings.windSpeedDirection.value.x;
            wind.y = settings.windSpeedDirection.value.y;
            wind.z = settings.shadowCutoffMin.value;
            wind.w = settings.shadowCutoffMax.value;
            m_BlitMaterial.SetVector(ShaderContants.windFactorID, wind);
            m_BlitMaterial.SetInt(ShaderContants.cloudHeightID, settings.cloudHeight.value);
            cmd.Blit(source, target, m_BlitMaterial);
            // cmd.Blit(target, source);

            cmd.EndSample(PROFILER_TAG);

        }
    }

}