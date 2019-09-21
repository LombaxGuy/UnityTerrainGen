using UnityEngine;

public struct FalloffMap
{
    public readonly float[,] values;

    public FalloffMap(float[,] values)
    {
        this.values = values;
    }
}