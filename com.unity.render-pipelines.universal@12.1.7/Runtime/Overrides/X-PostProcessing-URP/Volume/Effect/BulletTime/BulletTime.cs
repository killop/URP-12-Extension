using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Extra + "子弹时间 (Bullet Time)")]
    public class BulletTime : VolumeSetting
    {
        public override bool IsActive() => bulletTimeEnable.value;
        [Tooltip("启用特效")]
        public BoolParameter bulletTimeEnable = new BoolParameter(false);
        [Tooltip("持续时间")]
        public MinFloatParameter bulletTimesTime = new MinFloatParameter(5f, 0f);
        [Tooltip("扩散速度")]
        public MinFloatParameter bulletTimesBloomSpeed = new MinFloatParameter(1, 0);
        [Tooltip("径向模糊强度")]
        public ClampedFloatParameter bulletTimeRadialBlurPower = new ClampedFloatParameter(0.05f, 0f, 0.5f);
        [Tooltip("径向模糊质量")]
        public ClampedIntParameter bulletTimeRadialBlurQuality = new ClampedIntParameter(5, 0, 10);
        [Tooltip("过滤颜色")]
        public ColorParameter bulletTimeColor = new ColorParameter(Color.white);
        public bool m_TimeReset;

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/BulletTime";
        }

    }

    public class BulletTimeRenderer : VolumeRenderer<BulletTime>
    {
       
        private float timeCount = 0;
        private const string PROFILER_TAG = "Bullet Time";

        public override bool IsActive()
        {
            if (settings.m_TimeReset)
            {
                timeCount = 0;
                settings.m_TimeReset = false;
            }

            if (timeCount > settings.bulletTimesTime.value)
            {
                return false;
            }
            else
            {
                timeCount += Time.deltaTime;
            }
            //if(timeCount >= settings.bulletTimesTime.value)return false;
            return base.IsActive();
        }

        

        static class ShaderContants
        {
            public static readonly int bulletTimeOriginPositionID = Shader.PropertyToID("_OriginPos");
            public static readonly int bulletTimeBlurQualityID = Shader.PropertyToID("_BlurQuality");
            public static readonly int bulletTimeBlurPowerID = Shader.PropertyToID("_BlurPower");
            public static readonly int bulletTimeBlurCurveID = Shader.PropertyToID("_BulletTimeColor");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null) return;



            cmd.BeginSample(PROFILER_TAG);

            Vector4 vec = Vector4.zero;
            vec.w = timeCount * settings.bulletTimesBloomSpeed.value;
            m_BlitMaterial.SetVector(ShaderContants.bulletTimeOriginPositionID, vec);
            m_BlitMaterial.SetFloat(ShaderContants.bulletTimeBlurPowerID, settings.bulletTimeRadialBlurPower.value);
            m_BlitMaterial.SetInt(ShaderContants.bulletTimeBlurQualityID, settings.bulletTimeRadialBlurQuality.value);
            m_BlitMaterial.SetVector(ShaderContants.bulletTimeBlurCurveID, settings.bulletTimeColor.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}