using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HappyElements.AdaptivePerformance
{
    public static class AdaptivePerformanceConfig
    {
        private static APUnityHook _instance = null;
        private static APUnityHook Instance

        {

            get
            {
                if (_instance == null)
                {
                    var gm = new GameObject("[AdaptivePerformanceConfig]");
                    gm.hideFlags = HideFlags.HideInHierarchy;
                    UnityEngine.Object.DontDestroyOnLoad(gm);
                    _instance = gm.AddComponent<APUnityHook>();
                }
                return _instance;
            }
        }
        public static string GetRawConfig(string key)
        {
            return Instance.GetRawConfig(key);
        }
        private static bool HaveConfig(string key)
        {
            return Instance.HaveConfig(key);
        }

        public static bool SetConfig(string key, string value)
        {
            return Instance.SetConfig(key, value);
        }

        public static bool GetBoolConfig(string key,bool defaultValue)
        {
            if (!Application.isPlaying)
            {
                return defaultValue;
            }
            return Instance.GetBoolConfig(key, defaultValue);
           
        }

        public static int GetIntConfig(string key, int defaultValue)
        {
            if (!Application.isPlaying)
            {
                return defaultValue;
            }
            return Instance.GetIntConfig(key, defaultValue);
           
        }
        public static float GetFloatConfig(string key, float defaultValue)
        {
            if (!Application.isPlaying)
            {
                return defaultValue;
            }
            return Instance.GetFloatConfig(key, defaultValue);
        }

        public static string GetStringConfig(string key,string defaultValue)
        {
            if (!Application.isPlaying)
            {
                return defaultValue;
            }
            return Instance.GetStringConfig(key, defaultValue);
        }
        public static void AddOnConfigChangeCallBack(Action<string> action)
        {
             Instance.AddOnConfigChangeCallBack(action);
        }

        public static void ClearOnConfigChangeCallBack()
        {
            Instance.ClearOnConfigChangeCallBack();
        }
        public class APUnityHook : MonoBehaviour
        {
            public static Dictionary<string, string> config = new Dictionary<string, string>();


            private static Dictionary<string, bool> dirtyCache = new Dictionary<string, bool>();
            private static Dictionary<string, bool> boolCache = new Dictionary<string, bool>();
            private static Dictionary<string, int> intCache = new Dictionary<string, int>();
            private static Dictionary<string, float> floatCache = new Dictionary<string, float>();



            public static Action<string> OnConfigChange;

            public  string GetRawConfig(string key)
            {
                string value = "";
                config.TryGetValue(key, out value);
                return value;
            }
            public bool HaveConfig(string key)
            {
                return config.ContainsKey(key);
            }

            public  bool SetConfig(string key, string value)
            {

                bool change = true;
                bool find = config.TryGetValue(key, out string oldString);
                if (find)
                {
                    if (oldString == value)
                    {
                        change = false;

                    }
                }
                if (change)
                {
                    config[key] = value;
                    dirtyCache[key] = true;
                    if (OnConfigChange != null)
                    {
                        OnConfigChange(key);
                    }
                }
                return change;
            }

            public  bool GetBoolConfig(string key,bool defaultValue)
            {
                
                bool result = false;
                if (dirtyCache.ContainsKey(key))
                {
                    var value = GetRawConfig(key);

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value == "1")
                        {
                            result = true;
                        }
                        else if (value == "0")
                        {
                            result = false;
                        }
                        else
                        {
                            result = bool.Parse(value);
                        }
                    }
                    boolCache[key] = result;
                    dirtyCache.Remove(key);
                }
                else
                {
                    if (!boolCache.ContainsKey(key)) {
                        return defaultValue;
                    }
                    return boolCache[key];
                }
                return result;

            }

            public  int GetIntConfig(string key,int defaultValue)
            {
               
                int result = 0;
                if (dirtyCache.ContainsKey(key))
                {
                    var value = GetRawConfig(key);

                    if (!string.IsNullOrEmpty(value))
                    {
                        result = int.Parse(value);
                    }
                    intCache[key] = result;
                    dirtyCache.Remove(key);
                }
                else
                {
                    if (!intCache.ContainsKey(key))
                    {
                        return defaultValue;
                    }
                    return intCache[key];
                }
                return result;
            }
            public  float GetFloatConfig(string key,float defaultValue)
            {
                
                float result = 0;
                if (dirtyCache.ContainsKey(key))
                {
                    var value = GetRawConfig(key);

                    if (!string.IsNullOrEmpty(value))
                    {
                        result = float.Parse(value);
                    }
                    floatCache[key] = result;
                    dirtyCache.Remove(key);
                }
                else
                {
                    if (!floatCache.ContainsKey(key))
                    {
                        return defaultValue;
                    }
                    return floatCache[key];
                }
                return result;
            }

            public string GetStringConfig(string key, string defaultValue)
            {
                string result = "";
                if (dirtyCache.ContainsKey(key))
                {
                    result = GetRawConfig(key);
                }
                else
                {
                    result=  defaultValue;
                }
                return result;
            }

            public void AddOnConfigChangeCallBack(Action<string> action)
            {
                OnConfigChange -= action;
                OnConfigChange += action;
            }

            public void ClearOnConfigChangeCallBack()
            {
                OnConfigChange = null;
            }

            public void OnDisable()
            {
                ClearOnConfigChangeCallBack();
            }

            public void OnDestroy()
            {
                ClearOnConfigChangeCallBack();
            }

            public void OnApplicationQuit()
            {
                ClearOnConfigChangeCallBack();
            }
        }

    }
}

