using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;

public enum ResolutionLevel
{
    HIGH = 0,
    NORMAL,
    LOW,
    VERYLOW,
};

public class PlayerGraphicsSetting: MonoBehaviour
{ 
    //锁定上限帧率
    public int frameRate = 60;
    //shaderLod
    public int shaderLod = 500;
    //分辨率设置
    public ResolutionLevel resolutionLevel = ResolutionLevel.HIGH;
    //显示帧数和平均帧率
    public bool displayFPS = false;
    //动态分辨率
    public bool dynamicQuality = false;
    //HDR
    public bool hdr = true;
    //Shadow
    public bool shadow = true;
    //LowShadow
    public bool lowShadow = true;
    //角色分层 
    public bool playerLayer = true;
    //特效分层 
    public bool alphaBuffer = true;
    //扭曲
    public bool distortion = true;
    //bloom
    public bool sceneBloom = true;
    public bool alphaBloom = true;
    public bool playerBloom = true;
    //色彩校正
    public bool sceneColorLookup = true;
    public bool alphaColorLookup = true;
    public bool playerColorLookup = true;
    //遮罩
    public bool mask = true;
    //抗锯齿
    public bool antialiasing = true;   
    //vignette
    public bool depthOfField = true;
    //AmbientOcclusion
    public bool ambientOcclusion = true;
    //volumetricFog
    public bool volumetricFog = true;
    //vignette
    public bool vignette = true;
    //border
    public bool border = true;
    //radialBlur
    public bool radialBlur = true;
    //cloudShadow
    public bool cloudShadow = true;
    //heightFog
    public bool heightFog = true;
    //chromaticAberration
    public bool chromaticAberration = true;
    //CSPOINTLIGHT
    public bool csPointLight = false;
    //ReflectionMap
    public bool reflectionMap = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (PlayerPrefs.HasKey("PlayerSetting"))
        {
            CheckKeyExist();
            Read();
        }            
        else
            Init();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    public void Init()
    {      
        frameRate = 60;
        shaderLod = 500;
        resolutionLevel = ResolutionLevel.NORMAL;
        displayFPS = false;
        dynamicQuality = false;
        hdr = true;
        shadow = true;
        lowShadow = false;
        playerLayer = true;
        alphaBuffer = true;
        distortion = true;
        sceneBloom = true;
        alphaBloom = true;
        playerBloom = true;
        sceneColorLookup = true;
        alphaColorLookup = true;
        playerColorLookup = true;
        mask = true;
        antialiasing = true;
        depthOfField = true;
        ambientOcclusion = true;
        volumetricFog = true;
        vignette = true;
        border = true;
        radialBlur = true;
        cloudShadow = true;
        heightFog = true;
        chromaticAberration = true;
        csPointLight = true;
        reflectionMap = true;
    }
    public void Save()
    {
        PlayerPrefs.SetInt("PlayerSetting", 1);
       
        PlayerPrefs.SetInt("frameRate", frameRate);
        PlayerPrefs.SetInt("shaderLod", shaderLod);
        PlayerPrefs.SetInt("resolutionLevel", (int)resolutionLevel);
        PlayerPrefs.SetInt("dynamicQuality", dynamicQuality ? 1 : 0);
        PlayerPrefs.SetInt("displayFPS", displayFPS ? 1 : 0);
        PlayerPrefs.SetInt("hdr", hdr ? 1 : 0);
        PlayerPrefs.SetInt("mrt", hdr ? 1 : 0);
        PlayerPrefs.SetInt("shadow", shadow ? 1 : 0);
        PlayerPrefs.SetInt("lowShadow", lowShadow ? 1 : 0);
        PlayerPrefs.SetInt("playerLayer", playerLayer ? 1 : 0);
        PlayerPrefs.SetInt("alphaBuffer", alphaBuffer ? 1 : 0);
        PlayerPrefs.SetInt("distortion", distortion ? 1 : 0);
        PlayerPrefs.SetInt("sceneBloom", sceneBloom ? 1 : 0);
        PlayerPrefs.SetInt("alphaBloom", alphaBloom ? 1 : 0);
        PlayerPrefs.SetInt("playerBloom", playerBloom ? 1 : 0);
        PlayerPrefs.SetInt("sceneColorLookup", sceneColorLookup ? 1 : 0);
        PlayerPrefs.SetInt("alphaColorLookup", alphaColorLookup ? 1 : 0);
        PlayerPrefs.SetInt("playerColorLookup", playerColorLookup ? 1 : 0);
        PlayerPrefs.SetInt("mask", mask ? 1 : 0);
        PlayerPrefs.SetInt("antialiasing", antialiasing ? 1 : 0);
        PlayerPrefs.SetInt("depthOfField", depthOfField ? 1 : 0);
        PlayerPrefs.SetInt("ambientOcclusion", ambientOcclusion ? 1 : 0);
        PlayerPrefs.SetInt("volumetricFog", volumetricFog ? 1 : 0);
        PlayerPrefs.SetInt("vignette", vignette ? 1 : 0);
        PlayerPrefs.SetInt("border", border ? 1 : 0);
        PlayerPrefs.SetInt("radialBlur", radialBlur ? 1 : 0);
        PlayerPrefs.SetInt("cloudShadow", cloudShadow ? 1 : 0);
        PlayerPrefs.SetInt("heightFog", heightFog ? 1 : 0);
        PlayerPrefs.SetInt("chromaticAberration", chromaticAberration ? 1 : 0);
        PlayerPrefs.SetInt("csPointLight", csPointLight ? 1 : 0);
        PlayerPrefs.SetInt("reflectionMap", reflectionMap ? 1 : 0);
    }
    public void CheckKeyExist()
    {
        if(!PlayerPrefs.HasKey("frameRate"))
        {
            frameRate = 60;
        }
        if (!PlayerPrefs.HasKey("shaderLod"))
        {
            shaderLod = 500;
        }
        if (!PlayerPrefs.HasKey("resolutionLevel"))
        {
            resolutionLevel = ResolutionLevel.NORMAL;
        }
        if (!PlayerPrefs.HasKey("dynamicQuality"))
        {
            dynamicQuality = true;
        }
        if (!PlayerPrefs.HasKey("displayFPS"))
        {
            displayFPS = true;
        }
        if (!PlayerPrefs.HasKey("hdr"))
        {
            hdr = true;
        }
        if (!PlayerPrefs.HasKey("shadow"))
        {
            shadow = true;
        }
        if (!PlayerPrefs.HasKey("lowShadow"))
        {
            lowShadow = false;
        }
        if (!PlayerPrefs.HasKey("playerLayer"))
        {
            playerLayer = true;
        }
        if (!PlayerPrefs.HasKey("alphaBuffer"))
        {
            alphaBuffer = true;
        }
        if (!PlayerPrefs.HasKey("distortion"))
        {
            distortion = true;
        }
        if (!PlayerPrefs.HasKey("sceneBloom"))
        {
            sceneBloom = true;
        }
        if (!PlayerPrefs.HasKey("alphaBloom"))
        {
            alphaBloom = true;
        }
        if (!PlayerPrefs.HasKey("playerBloom"))
        {
            playerBloom = true;
        }
        if (!PlayerPrefs.HasKey("sceneColorLookup"))
        {
            sceneColorLookup = true;
        }
        if (!PlayerPrefs.HasKey("alphaColorLookup"))
        {
            alphaColorLookup = true;
        }
        if (!PlayerPrefs.HasKey("playerColorLookup"))
        {
            playerColorLookup = true;
        }
        if (!PlayerPrefs.HasKey("mask"))
        {
            mask = true;
        }
        if (!PlayerPrefs.HasKey("antialiasing"))
        {
            antialiasing = true;
        }
        if (!PlayerPrefs.HasKey("depthOfField"))
        {
            depthOfField = true;
        }
        if (!PlayerPrefs.HasKey("ambientOcclusion"))
        {
            ambientOcclusion = true;
        }
        if (!PlayerPrefs.HasKey("volumetricFog"))
        {
            volumetricFog = true;
        }
        if (!PlayerPrefs.HasKey("vignette"))
        {
            vignette = true;
        }
        if (!PlayerPrefs.HasKey("border"))
        {
            border = true;
        }
        if (!PlayerPrefs.HasKey("radialBlur"))
        {
            radialBlur = true;
        }
        if (!PlayerPrefs.HasKey("cloudShadow"))
        {
            cloudShadow = true;
        }
        if (!PlayerPrefs.HasKey("heightFog"))
        {
            heightFog = true;
        }
        if (!PlayerPrefs.HasKey("chromaticAberration"))
        {
            chromaticAberration = true;
        }
        if (!PlayerPrefs.HasKey("csPointLight"))
        {
            csPointLight = true;
        }
        if (!PlayerPrefs.HasKey("reflectionMap"))
        {
            reflectionMap = true;
        }
    }
    public void Read()
    {
        frameRate = PlayerPrefs.GetInt("frameRate");
        shaderLod = PlayerPrefs.GetInt("shaderLod");
        resolutionLevel = (ResolutionLevel)PlayerPrefs.GetInt("resolutionLevel");
        dynamicQuality = PlayerPrefs.GetInt("dynamicQuality") > 0 ? true : false;       
        displayFPS = PlayerPrefs.GetInt("displayFPS") > 0 ? true : false;
        hdr = PlayerPrefs.GetInt("hdr") > 0 ? true : false;
        shadow = PlayerPrefs.GetInt("shadow") > 0 ? true : false;
        lowShadow = PlayerPrefs.GetInt("lowShadow") > 0 ? true : false;
        playerLayer = PlayerPrefs.GetInt("playerLayer") > 0 ? true : false;
        alphaBuffer = PlayerPrefs.GetInt("alphaBuffer") > 0 ? true : false;
        distortion = PlayerPrefs.GetInt("distortion") > 0 ? true : false;
        sceneBloom = PlayerPrefs.GetInt("sceneBloom") > 0 ? true : false;
        alphaBloom = PlayerPrefs.GetInt("alphaBloom") > 0 ? true : false;
        playerBloom = PlayerPrefs.GetInt("playerBloom") > 0 ? true : false;
        sceneColorLookup = PlayerPrefs.GetInt("sceneColorLookup") > 0 ? true : false;
        alphaColorLookup = PlayerPrefs.GetInt("alphaColorLookup") > 0 ? true : false;
        playerColorLookup = PlayerPrefs.GetInt("playerColorLookup") > 0 ? true : false;
        mask = PlayerPrefs.GetInt("mask") > 0 ? true : false;
        antialiasing = PlayerPrefs.GetInt("antialiasing") > 0 ? true : false;
        depthOfField = PlayerPrefs.GetInt("depthOfField") > 0 ? true : false;
        ambientOcclusion = PlayerPrefs.GetInt("ambientOcclusion") > 0 ? true : false;
        volumetricFog = PlayerPrefs.GetInt("volumetricFog") > 0 ? true : false;
        vignette = PlayerPrefs.GetInt("vignette") > 0 ? true : false;
        border = PlayerPrefs.GetInt("border") > 0 ? true : false;
        radialBlur = PlayerPrefs.GetInt("radialBlur") > 0 ? true : false;
        cloudShadow = PlayerPrefs.GetInt("cloudShadow") > 0 ? true : false;
        heightFog = PlayerPrefs.GetInt("heightFog") > 0 ? true : false;
        chromaticAberration = PlayerPrefs.GetInt("chromaticAberration") > 0 ? true : false;
        csPointLight = PlayerPrefs.GetInt("csPointLight") > 0 ? true : false;
        reflectionMap = PlayerPrefs.GetInt("reflectionMap") > 0 ? true : false;   
    }
}
