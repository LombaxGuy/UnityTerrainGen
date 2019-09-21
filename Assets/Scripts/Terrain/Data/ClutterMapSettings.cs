using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClutterMapSettings", menuName = "CustomTerrain/Clutter map settings", order = 4)]
public class ClutterMapSettings : UpdatableData
{
    [Range(0, 0.1f)]
    public float density = 0.05f;
    //public NoiseSettings noiseSettings = new NoiseSettings(1, 1, 0.5f, 2);

    //[Range(0, 5)]
    //public float amplification = 1;

    //[Range(0, 1)]
    //public float clutterStartValue = 0;
    //[Range(0, 1)]
    //public float clutterEndValue = 1;

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        //noiseSettings.ValidateValues();

        //clutterStartValue = Mathf.Min(clutterStartValue, clutterEndValue);
        //clutterEndValue = Mathf.Max(clutterEndValue, clutterStartValue);

        //amplification = Mathf.Max(0, amplification);

        base.OnValidate();
    }
#endif
}
