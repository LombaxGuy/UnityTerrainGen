using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public Vector2 coordinate;

    public GameObject meshObject;
    Vector2 offset;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    float[,] heightMap;
    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    MeshData meshData;

    public TerrainChunk(Vector2 coordinate, float[,] heightMap, MeshSettings meshSettings, Transform parent, Material material)
    {
        this.coordinate = coordinate;
        this.heightMap = heightMap;
        this.meshSettings = meshSettings;

        offset = new Vector2((meshSettings.meshWorldSize * (meshSettings.worldChunkWidth - 1)) / 2, (meshSettings.meshWorldSize * (meshSettings.worldChunkWidth - 1)) / 2);

        Vector2 position = coordinate * meshSettings.meshWorldSize - offset;

        meshObject = new GameObject("Terrain Chunck: " + coordinate);
        meshObject.layer = LayerMask.NameToLayer("Terrain");

        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshRenderer.material = material;
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        meshData = MeshGenerator.TestMeshData(heightMap, meshSettings);

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
}
