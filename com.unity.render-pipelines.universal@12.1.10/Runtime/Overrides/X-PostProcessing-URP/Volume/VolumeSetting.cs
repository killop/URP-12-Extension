using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [Serializable]
    public abstract class VolumeSetting : VolumeComponent, IPostProcessComponent
    {
        public abstract bool IsActive();
        public bool IsTileCompatible() => false;
        public abstract string GetShaderName();
        
        //面板上打钩是回复到默认值的意思 
        //需要默认就关闭效果 强度为0
    }

}