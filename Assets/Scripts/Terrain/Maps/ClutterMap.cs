using UnityEngine;

public struct ClutterMap
{
    public readonly float[,] values;
    public readonly float density;
    
    //public readonly float clutterStartValue;
    //public readonly float clutterEndValue;
    //public readonly float amplification;

    public ClutterMap(float[,] values, float density/*, float clutterStartValue, float clutterEndValue, float amplification*/, BiomeMap biomeMap)
    {
        this.values = new float[values.GetLength(0), values.GetLength(1)];
        //this.clutterStartValue = clutterStartValue;
        //this.clutterEndValue = clutterEndValue;
        //this.amplification = amplification;

        //float amplificationModifier = (amplification - 1) / (biomeMap.numberOfBiomes - 1);

        this.density = 0.05f;

        for (int x = 0; x < values.GetLength(0); x++)
        {
            for (int y = 0; y < values.GetLength(1); y++)
            {
                for (int i = biomeMap.numberOfBiomes - 1; i >= 0; i--)
                {
                    // Biome 0 always have no clutter
                    if (i == 0)
                    {
                        values[x, y] = 0;
                    }
                    else if (biomeMap.values[x, y] >= biomeMap.biomeStartValues[i])
                    {
                        if (i == 1)
                        {
                            this.values[x, y] = (values[x, y] <= 1 - density * 0.1f) ? 0 : 1;
                        }
                        else if (i == 2)
                        {
                            this.values[x, y] = (values[x, y] <= 1 - density) ? 0 : 1;
                        }
                        //values[x, y] = Mathf.Clamp01(values[x, y] * (1 + amplificationModifier * i));
                        break;
                    }
                }

                //this.values[x, y] = (values[x, y] <= 1 - density) ? 0 : 1;
            }
        }
    }
}
