using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyElements.AdaptivePerformance;


public class URPAdaptivePerformance
{
    public const string URP_APKEY_SCREEN_SCALE_RADIO = "ScreenScaleRadio";
    public const string URP_APKEY_SHADOW = "Shadow";
    public const string URP_AA = "AA";
    public const string URP_APKEY_CLOUD_SHADOW = "CloudShadow";
    public const string URP_APKEY_DYNAMIC_LIGHT = "DynamicLight";
    public const string URP_APKEY_DOF = "DOF";
    public const string URP_APKEY_RADIAL_BLUR = "RadialBlur";
    public const string URP_APKEY_MOTION_BLUR = "MotionBlur";
    public const string URP_APKEY_SHADOW_RESOLUTION = "ShadowResolution";
    public const string URP_APKEY_BLOOM = "Bloom";

    private static float? ScreenScaleRadio = null;
    private static bool? ShadowEnable = null;
    private static bool? AAEnable = null;
    private static bool? CloudShadowEnable = null;
    private static bool? DynamicLightEnable = null;
    private static bool? DOFEnable = null;
    private static bool? RadialBlurEnable = null;
    private static bool? MotionBlurEnable = null;
    private static int? ShadowResolution = null;
    private static bool? BloomEnable = null;

    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        AdaptivePerformanceConfig.AddOnConfigChangeCallBack(OnConfigChange);
    }

    public static bool GetScreenScaleRadio(float defaultValue,out float result)
    {
        if (ScreenScaleRadio.HasValue)
        {
            result = ScreenScaleRadio.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }

    public static bool GetShadowEnable(bool defaultValue,out bool result)
    {
        if (ShadowEnable.HasValue)
        {
            result = ShadowEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false ;
        }
    }

    public static bool GetAAEnable(bool defaultValue, out bool result)
    {
        if (AAEnable.HasValue)
        {
            result = AAEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }
    public static bool GetCloudShadowEnable(bool defaultValue,out bool result)
    {
        if (CloudShadowEnable.HasValue)
        {
            result = CloudShadowEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }

   

    public static bool GetDOFEnable(bool defaultValue, out bool result)
    {
       
        if (DOFEnable.HasValue)
        {
            result = DOFEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }

    public static bool GetRadialBlurEnable(bool defaultValue, out bool result)
    {
       
        if (RadialBlurEnable.HasValue)
        {
            result = RadialBlurEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }


    public static bool GetMotionBlurEnable(bool defaultValue, out bool result)
    {
        
        if (MotionBlurEnable.HasValue)
        {
            result = MotionBlurEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }

    public static bool GetShadowResolution(int defaultValue, out int result)
    {

        if (ShadowResolution.HasValue)
        {
            result = ShadowResolution.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }
    public static bool GetBloomEnable(bool defaultValue, out bool result)
    {
        if (BloomEnable.HasValue)
        {
            result = BloomEnable.Value;
            return true;
        }
        else
        {
            result = defaultValue;
            return false;
        }
    }


    public static void OnConfigChange(string key)
    {
        switch (key)
        {
            case (URP_APKEY_SCREEN_SCALE_RADIO):
            {
               ScreenScaleRadio = AdaptivePerformanceConfig.GetFloatConfig(URP_APKEY_SCREEN_SCALE_RADIO,1f);
               break;
            }
            case (URP_APKEY_SHADOW):
                {
                    ShadowEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_SHADOW, true);
                    break;
                }
            case (URP_AA):
                {
                    AAEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_AA, true);
                    break;
                }
            case (URP_APKEY_CLOUD_SHADOW):
                {
                    CloudShadowEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_CLOUD_SHADOW, true);
                    break;
                }
            case (URP_APKEY_DYNAMIC_LIGHT):
                {
                    DynamicLightEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_DYNAMIC_LIGHT, true);
                    break;                                                                                  
                }                                                                                            
            case (URP_APKEY_DOF):                                                             
                {
                    DOFEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_DOF, true);
                    break;                                                                                  
                }                                                                                           
            case (URP_APKEY_RADIAL_BLUR):                                                            
                {
                    RadialBlurEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_RADIAL_BLUR, true);
                    break;                                                                                  
                }                                                                                           
            case (URP_APKEY_MOTION_BLUR):                                                            
                {
                    MotionBlurEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_MOTION_BLUR, true);
                    break;                                                                                   
                }
            case (URP_APKEY_SHADOW_RESOLUTION):
                {
                    ShadowResolution = AdaptivePerformanceConfig.GetIntConfig(URP_APKEY_SHADOW_RESOLUTION, 512);
                    break;
                }
            case (URP_APKEY_BLOOM):
                {
                    BloomEnable = AdaptivePerformanceConfig.GetBoolConfig(URP_APKEY_BLOOM, true);
                    break;
                }
            default:
                break;
        }
    }
}
