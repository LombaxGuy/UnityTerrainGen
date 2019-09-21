using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FalloffMapSettings", menuName = "CustomTerrain/Falloff map settings", order = 2)]
public class FalloffMapSettings : UpdatableData
{
    public FalloffShape falloffShape = FalloffShape.Circular;

    [Range(0, 1)]
    [SerializeField]
    private float strenght = 0.95f;

    [HideInInspector]
    public float invertedStrength = 0.05f;

    [Range(0, 100)]
    public float falloffSize = 100f;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        invertedStrength = 1 - strenght;

        base.OnValidate();
    }
#endif
}

public enum FalloffShape { Circular, Square };