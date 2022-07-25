using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class TextureHelpers
{
    public static Texture2D BackTexture;

    private static Texture2D[] frontTextures;
    private static Vector3[] frontTexturesColors;

    public static void InitFrontTextures()
    {
        BackTexture = (Texture2D)Resources.Load("back");

        frontTextures = new Texture2D[]
        {
            (Texture2D)Resources.Load("black"),
            (Texture2D)Resources.Load("white"),
            (Texture2D)Resources.Load("red"),
            (Texture2D)Resources.Load("green"),
            (Texture2D)Resources.Load("blue"),
        };

        frontTexturesColors = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
        };
    }

    public static Texture2D GetClosestByColorFrontTexture(Texture2D texture)
    {
        const int pixelSampleStep = 200;

        var buckets = new int[frontTextures.Length];
        var pixels = texture.GetPixels32();

        for (int i = 0; i < pixels.Length; i += pixelSampleStep)
        {
            var color = new Vector3(pixels[i].r / 255f, pixels[i].g / 255f, pixels[i].b / 255f);

            int bucketIndex = -1;
            float minimumColorDistance = float.MaxValue;

            for (int j = 0; j < buckets.Length; j++)
            {
                var colorDistance = (frontTexturesColors[j] - color).magnitude;
                if (colorDistance < minimumColorDistance)
                {
                    minimumColorDistance = colorDistance;
                    bucketIndex = j;
                }
            }

            buckets[bucketIndex]++;
        }

        var correctedBuckets = new float[buckets.Length];

        // coefficients are from test run
        correctedBuckets[0] = buckets[0] * 1.0f;    // black
        correctedBuckets[1] = buckets[1] * 1.25f;   // white
        correctedBuckets[2] = buckets[2] * 7.5f;    // red
        correctedBuckets[3] = buckets[3] * 150.0f;  // green
        correctedBuckets[4] = buckets[4] * 30.0f;   // blue

        float maxValue = float.MinValue;
        int maxValueIndex = -1;

        for (int i = 0; i < correctedBuckets.Length; i++)
        {
            if (correctedBuckets[i] > maxValue)
            {
                maxValue = correctedBuckets[i];
                maxValueIndex = i;
            }
        }

        return frontTextures[maxValueIndex];
    }
}
