using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XPostProcessing
{
    public class VolumeUtility
    {


        //-----------------------------------------------------------------------------------------------------
        static int resetFrameCount = 0;
        static Color srcColor;
        static Color dstColor;
        public static Color GetRandomLerpColor(int RandomFrameCount, float lerpSpeed)
        {
            // Color version
            if (resetFrameCount == 0)
            {
                srcColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            }
            float lerp = lerpSpeed;

            dstColor = Color.Lerp(dstColor, srcColor, lerp);
            resetFrameCount++;
            if (resetFrameCount > RandomFrameCount)
            {
                resetFrameCount = 0;
            }

            return dstColor;
        }

        public static Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value, Random.value);
        }

    }
}