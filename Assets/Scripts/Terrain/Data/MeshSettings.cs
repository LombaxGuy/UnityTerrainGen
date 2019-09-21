using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshSettings", menuName = "CustomTerrain/Mesh Settings", order = 6)]
public class MeshSettings : UpdatableData
{
    public float unitsPerVertex = 0.25f;

    [Range(1, 10)]
    public int worldChunkWidth = 1;
    
    [SerializeField]
    [Range(100, 240)]
    private int chunkSize = 100;
     
    //public int chunksFromCenterToEdge { get { return worldChunkWidth - 1 / 2; } }

    //public int vertexWidth { get { return chunkSize + 1; } }

    // includes 2 extra verts that are excluded from final mesh but used for calculating normals
    public int numVertsPerLine { get { return chunkSize + 3; } }

    public float meshWorldSize { get { return (numVertsPerLine - 3) * unitsPerVertex; } }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}