using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PreviewMap
{
    public float[,] values;
    public Color[] colors;
    public float[] colorValues;

    public PreviewMap(float[,] values, float[] colorValues, Color[] colors)
    {
        this.values = values;
        this.colorValues = colorValues;
        this.colors = colors;
    }
}
