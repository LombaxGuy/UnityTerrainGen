using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveMeshTest : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateMeshAsset();
        }
    }

    private void CreateMeshAsset()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        AssetDatabase.CreateAsset(mesh, "Assets/Prefabs/TerrainMesh.asset");
        Debug.Log("TerrainMesh asset created.");
    }
}
