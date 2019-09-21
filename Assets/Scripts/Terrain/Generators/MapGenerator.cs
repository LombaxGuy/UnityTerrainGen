using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static int minSeedValue = 0;
    public static int maxSeedValue = 999999;

    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, FalloffMap falloffMap, Vector2 sampleCenter)
    {
        float[,] values = NoiseGenerator.GeneratePerlinNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (settings.useFalloff)
                {
                    values[i, j] *= settings.heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier * falloffMap.values[i, j];
                }
                else
                {
                    values[i, j] *= settings.heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;
                }

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }

                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }

    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = NoiseGenerator.GeneratePerlinNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= settings.heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }

                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }

    public static HeightMap[,] GenerateHeightMapArray2D(HeightMap heightMap, int heightMapsPerLine)
    {
        HeightMap[,] heightMaps = new HeightMap[heightMapsPerLine, heightMapsPerLine];

        int totalHeightMapSize = heightMap.values.GetLength(0);
        int chunkHeightMapSize = totalHeightMapSize / heightMapsPerLine - 3;

        for (int x = 0; x < heightMapsPerLine; x++)
        {
            for (int z = 0; z < heightMapsPerLine; z++)
            {
                // + 4 for some magical reason, idk why
                float[,] current = new float[chunkHeightMapSize + 4, chunkHeightMapSize + 4];

                int offsetX = x * chunkHeightMapSize;
                int offsetZ = z * chunkHeightMapSize;

                for (int i = 0; i < chunkHeightMapSize + 4; i++)
                {
                    for (int j = 0; j < chunkHeightMapSize + 4; j++)
                    {

                        try
                        {
                            current[i, j] = heightMap.values[i + offsetX - 1, j + offsetZ - 1];
                        }
                        catch
                        {
                            current[i, j] = 0;
                        }
                    }
                }

                heightMaps[x, z] = new HeightMap(current, heightMap.minValue, heightMap.maxValue);
            }
        }

        return heightMaps;
    }

    public static FalloffMap GenerateFalloffMap(int size, FalloffMapSettings settings)
    {
        float[,] map = new float[size, size];

        float islandSize = settings.falloffSize / 100 * size;
        float falloffStrenght = settings.invertedStrength;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //float distanceX = Mathf.Abs(i - width * 0.5f);
                //float distanceY = Mathf.Abs(j - height * 0.5f);

                //float distance = 0;

                //switch (settings.falloffShape)
                //{
                //    case FalloffShape.Circular:
                //        distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);
                //        break;
                //    case FalloffShape.Square:
                //        distance = Mathf.Max(distanceX, distanceY);
                //        break;
                //}

                //float maxWidth = islandSizeX * 0.5f - 10f;
                //float maxHeight = islandSizeY * 0.5f - 10f;

                //float deltaX = distance / maxWidth;
                //float deltaY = distance / maxHeight;

                //float gradient = deltaX * deltaY;

                //float value = Mathf.Max(settings.invertedStrength, 1.0f - gradient);
                //map[i, j] = value;

                Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
                float distanceFromCenter = 0;

                switch (settings.falloffShape)
                {
                    case FalloffShape.Circular:
                        Vector2 currentPoint = new Vector2(i, j);

                        distanceFromCenter = Vector2.Distance(center, currentPoint);
                        break;

                    case FalloffShape.Square:
                        float distanceX = Mathf.Abs(i - center.x);
                        float distanceY = Mathf.Abs(j - center.y);

                        distanceFromCenter = Mathf.Max(distanceX, distanceY);
                        break;
                }

                float maxSize = islandSize * 0.5f;

                float deltaSize = distanceFromCenter / maxSize;

                float mapMaxValue = 1f;
                float falloffSteepness = -10f;

                // Logistic function (Sigmoid curve)
                float value = (mapMaxValue - falloffStrenght) / (1f + Mathf.Exp((-falloffSteepness) * (deltaSize - 0.7f))) + falloffStrenght;

                map[i, j] = value;
            }
        }

        return new FalloffMap(map);
    }

    public static BiomeMap GenerateBiomeMap(int width, int height, BiomeMapSettings settings, Vector2 sampleCenter, bool randomSeed = false)
    {
        NoiseSettings noiseSettings = settings.noiseSettings;

        if (randomSeed)
        {
            noiseSettings.seed = Random.Range(minSeedValue, maxSeedValue + 1);
        }

        float[,] noiseMap = NoiseGenerator.GeneratePerlinNoiseMap(width, height, noiseSettings, sampleCenter);

        BiomeMap biomeMap = new BiomeMap(noiseMap, settings.GetStartValues(), settings.biomes.Length);

        return biomeMap;
    }

    public static ClutterMap GenerateClutterMap(int width, int height, ClutterMapSettings settings, Vector2 sampleCenter, BiomeMap biomeMap, bool randomSeed = false)
    {
        //NoiseSettings noiseSettings = settings.noiseSettings;

        //if (randomSeed)
        //{
        //    noiseSettings.seed = Random.Range(minSeedValue, maxSeedValue + 1);
        //}

        //float[,] noiseMap = NoiseGenerator.GeneratePerlinNoiseMap(width, height, noiseSettings, sampleCenter);
        float[,] noiseMap = NoiseGenerator.GenerateRandomNoiseMap(width);  

        ClutterMap clutterMap = new ClutterMap(noiseMap, settings.density/*, settings.clutterStartValue, settings.clutterEndValue, settings.amplification*/, biomeMap);

        return clutterMap;
    }

    public static PreviewMap GeneratePreviewMap(int width, int height, float minValue, float maxValue, PreviewMapSettings settings, HeightMap heightMap, TextureData textureData/*, ClutterMap[] clutterMaps*/)
    {
        float[,] previewMapValues = new float[heightMap.values.GetLength(0), heightMap.values.GetLength(1)];
        int numberOfLayers = textureData.layers.Length;
        //int numberOfClutterMaps = clutterMaps.Length;
        Color[] colors = new Color[numberOfLayers/* + numberOfClutterMaps*/];
        float[] colorValues = new float[numberOfLayers/* + numberOfClutterMaps*/];

        for (int x = 0; x < heightMap.values.GetLength(0); x++)
        {
            for (int y = 0; y < heightMap.values.GetLength(1); y++)
            {
                previewMapValues[x, y] = Mathf.InverseLerp(minValue, maxValue, heightMap.values[x, y]);
            }
        }

        for (int x = 0; x < heightMap.values.GetLength(0); x++)
        {
            for (int y = 0; y < heightMap.values.GetLength(1); y++)
            {
                for (int i = numberOfLayers - 1; i >= 0; i--)
                {
                    if (previewMapValues[x, y] >= textureData.layers[i].startHeight)
                    {
                        previewMapValues[x, y] = i;
                        break;
                    }
                }

                //for (int i = 0; i < numberOfClutterMaps; i++)
                //{
                //    if (clutterMaps[i].values[x, y] == 1)
                //    {
                //        previewMapValues[x, y] = numberOfLayers + i;
                //    }
                //}
            }
        }

        for (int i = colors.Length - 1; i >= 0; i--)
        {
            colors[i] = settings.colors[i];
            colorValues[i] = i;
        }

        return new PreviewMap(previewMapValues, colorValues, colors);
    }
}
