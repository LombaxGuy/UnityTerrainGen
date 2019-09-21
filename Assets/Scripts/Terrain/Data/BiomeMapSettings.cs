using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeMapSettings", menuName = "CustomTerrain/Biome map settings", order = 3)]
public class BiomeMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings = new NoiseSettings(60, 4, 0.5f, 1.5f);

    public Biome[] biomes;

    public float[] GetStartValues()
    {
        if (biomes == null)
        {
            Debug.Log("No biomes defined.");
            return new float[0];
        }

        float[] biomeStartValues = new float[biomes.Length];

        for (int i = 0; i < biomes.Length; i++)
        {
            biomeStartValues[i] = biomes[i].startValue;
        }

        return biomeStartValues;
    }

    public Color[] GetColors()
    {
        if (biomes == null)
        {
            Debug.Log("No biomes defined.");
            return new Color[0];
        }

        Color[] colors = new Color[biomes.Length];

        for (int i = 0; i < biomes.Length; i++)
        {
            colors[i] = biomes[i].color;
        }

        return colors;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }
#endif
}

[System.Serializable]
public class Biome
{
    public string name;
    public Color color;
    [Range(0, 1)]
    public float startValue;
}
