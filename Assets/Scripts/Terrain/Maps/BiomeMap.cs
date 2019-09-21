using UnityEngine;

public struct BiomeMap
{
    public readonly float[,] values;
    public readonly float[] biomeStartValues;

    public readonly int numberOfBiomes;


    public BiomeMap(float[,] values, float[] biomeStartValues, int numberOfBiomes)
    {
        this.values = new float[values.GetLength(0), values.GetLength(1)];
        this.numberOfBiomes = numberOfBiomes;
        this.biomeStartValues = biomeStartValues;

        for (int x = 0; x < values.GetLength(0); x++)
        {
            for (int y = 0; y < values.GetLength(1); y++)
            {
                for (int i = numberOfBiomes - 1; i >= 0; i--)
                {
                    if (values[x, y] > biomeStartValues[i])
                    {
                        values[x, y] = biomeStartValues[i];
                        break;
                    }
                }

                this.values[x, y] = values[x, y];
            }
        }
    }
}
