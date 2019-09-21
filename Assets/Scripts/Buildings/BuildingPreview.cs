using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    private Mesh terrainMesh;

    private List<Collider> otherColliders = new List<Collider>();
    private List<Collider> currentTerrain = new List<Collider>();

    private Collider previewCollider;

    private float maxTerrainSlope = 30;
    private float maxBuildingHeight = 12;

    public void Initialize(Mesh terrainMesh)
    {
        this.terrainMesh = terrainMesh;

        previewCollider = GetComponent<Collider>();
    }

    private bool CheckTerrainSlope()
    {
        Vector3[] vertecies = terrainMesh.vertices;
        Vector3[] normals = terrainMesh.normals;

        for (int i = 0; i < vertecies.Length; i++)
        {
            if (previewCollider.bounds.Contains(vertecies[i]))
            {
                if (Vector3.Angle(Vector3.up, normals[i]) > maxTerrainSlope)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public Bounds GetNativeBounds()
    {
        Quaternion rot = transform.rotation;
        Bounds nonRotatedBounds;

        transform.rotation = Quaternion.identity;
        nonRotatedBounds = previewCollider.bounds;

        transform.rotation = rot;

        return nonRotatedBounds;
    }

    public List<GameObject> GetCurrentTerrains()
    {
        List<GameObject> newList = new List<GameObject>();

        for (int i = 0; i < currentTerrain.Count; i++)
        {
            newList.Add(currentTerrain[i].gameObject);
        }

        return newList;
    }

    public bool CanBePlacedAtThisPosition()
    {
        if (otherColliders.Count == 0)
        {
            if (transform.position.y < maxBuildingHeight)
            {
                if (CheckTerrainSlope())
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            AddToListIfNotInList(other, otherColliders);
            //AddToColliderList(other);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            AddToListIfNotInList(other, currentTerrain);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            RemoveFromListIfInList(other, otherColliders);
            //RemoveFromColliderList(other);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            RemoveFromListIfInList(other, currentTerrain);
        }
    }

    private void AddToListIfNotInList<T>(T objectToAdd, List<T> list)
    {
        if (!list.Contains(objectToAdd))
        {
            list.Add(objectToAdd);
        }
    }

    private void RemoveFromListIfInList<T>(T objectToRemove, List<T> list)
    {
        if (list.Contains(objectToRemove))
        {
            list.Remove(objectToRemove);
        }
    }
}
