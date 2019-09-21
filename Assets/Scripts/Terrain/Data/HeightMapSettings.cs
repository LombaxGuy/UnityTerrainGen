using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeightMapSettings", menuName = "CustomTerrain/Height map settings", order = 1)]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;

    public bool useFalloff = false;
    public FalloffMapSettings falloffMapSettings;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeigth { get { return heightMultiplier * heightCurve.Evaluate(0); } }
    public float maxHeigth { get { return heightMultiplier * heightCurve.Evaluate(1); } }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }
#endif
}
