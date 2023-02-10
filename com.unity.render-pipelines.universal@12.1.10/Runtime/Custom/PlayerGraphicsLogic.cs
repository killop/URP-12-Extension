using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PlayerGraphicsLogic: MonoBehaviour
{
    public int[] ResolutionLevel = new int[] { 1920, 1334, 1024, 800 };

    private float updateInterval = 0.5f;
    private float lastInterval;
    private float fps;
    private const int fpsArrayLen = 15;
    private float[] fpsArray = new float[fpsArrayLen];

    private int Width_1080p = 1920;
    private int Height_1080p = 1080;
    private int Width_750p = 1334;
    private int Height_750p = 750;
    private int Width_576p = 1024;
    private int Height_576p = 576;


   //int i_Frames = 0;
   //private float f_UpdateInterval = 0.5f;
   //private float f_LastInterval;
   //public float f_Fps;

    private void Awake()
    {
        
    }

    public void SetData(int frameRate, ResolutionLevel level,bool enableAA)
    {
        Application.targetFrameRate = frameRate;

       int resolution = ResolutionLevel[(int)level];
      
       if(Screen.width > Screen.height)
           Screen.SetResolution(resolution, resolution * Screen.height / Screen.width, true);
       else
           Screen.SetResolution(resolution * Screen.width / Screen.height, resolution, true);
      
       //var reflection = FindObjectOfType<MirrorReflectionLogic>();
       //if (reflection)
       //{
       //   reflection.SetCurrentPlane(reflection.currentIndex);
       //}

        if(Camera.main)
        {
            Camera.main.TryGetComponent<UniversalAdditionalCameraData>(out var baseCameraAdditionalData);
            if (baseCameraAdditionalData != null)
            {
                baseCameraAdditionalData.antialiasing = enableAA ? AntialiasingMode.FastApproximateAntialiasing : AntialiasingMode.None;
                baseCameraAdditionalData.antialiasingQuality = AntialiasingQuality.Medium;
            }                
        }       
    }

   //void Update()
   //{
   //    ++i_Frames;
   //
   //    if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
   //    {
   //        f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);
   //
   //        i_Frames = 0;
   //
   //        f_LastInterval = Time.realtimeSinceStartup;
   //    }
   //}
}
