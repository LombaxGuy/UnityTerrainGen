using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(HeightMap heightMap, Color minValueColor, Color maxValueColor)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colorMap[y * width + x] = Color.Lerp(minValueColor, maxValueColor, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromFloatArray2D(float[,] floatArray2D, Color minValueColor, Color maxValueColor, bool makeSpotted = false)
    {
        int width = floatArray2D.GetLength(0);
        int height = floatArray2D.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (makeSpotted)
                {
                    colorMap[y * width + x] = (floatArray2D[x, y] != 1) ? minValueColor : maxValueColor;
                }
                else
                {
                    colorMap[y * width + x] = Color.Lerp(minValueColor, maxValueColor, Mathf.InverseLerp(0, 1, floatArray2D[x, y]));
                }
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromFloatArray2D(float[,] floatArray2D, float[] startValues, Color[] colors)
    {
        int width = floatArray2D.GetLength(0);
        int height = floatArray2D.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < startValues.Length; z++)
                {
                    if (startValues[z] <= floatArray2D[x, y])
                    {
                        colorMap[y * width + x] = colors[z];
                    }
                }
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}