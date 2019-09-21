using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

public enum BuildingType { None, Hut, House }

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building prefabs")]
    public GameObject hutPrefab;
    public GameObject housePrefab;

    [Header("References")]
    public Material previewMaterial;

    public MeshFilter terrainMeshFilter;
    public Transform world;
   

    [Header("Private fields (Don't edit)")]
    [SerializeField]
    private GameObject selectedObject;
    [SerializeField]
    private BuildingType selectedBuildingType = BuildingType.None;
    //private BoxCollider previewCollider;
    private BuildingPreview previewScript;
    private MeshRenderer previewRenderer;

    private Vector3 previewPosition;
    private Quaternion previewRotation = Quaternion.identity;
    //private Quaternion currentPreviewRot = Quaternion.identity;
    private Quaternion savedPreviewRotation = Quaternion.identity;

    private bool currentCanPlaceHere;
    private Color previewGreen = new Color(0, 1.0f, 0, 0.5f);
    private Color previewRed = new Color(1.0f, 0, 0, 0.5f);

    private bool buttonClickedThisUpdate = false;
    [SerializeField]
    private bool isDragging;

    private Vector3 debugPoint1;
    private Vector3 debugPoint2;
    private Vector3 debugPoint3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (debugPoint1 != Vector3.zero)
        {
            Debug.DrawRay(debugPoint1, Vector3.up * 5);
            Debug.DrawRay(debugPoint2, Vector3.up * 5);
            Debug.DrawRay(debugPoint3, Vector3.up * 5);
        }

        if (selectedObject != null)
        {
            Vector3 mousePosition = CalculateCurrentMouseWorldPosition();

            if (Input.GetMouseButtonDown(0) && !buttonClickedThisUpdate)
            {
                previewPosition = mousePosition;
                isDragging = true;
            }

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    previewRotation = CalculateCurrentPreviewRotation(previewPosition, mousePosition, savedPreviewRotation);
            //}

            if (Input.GetMouseButton(0) && !buttonClickedThisUpdate)
            {
                previewRotation = CalculateCurrentPreviewRotation(previewPosition, mousePosition, savedPreviewRotation);
            }

            if (isDragging)
            {
                UpdatePreview(previewPosition, previewRotation);
            }
            else
            {
                UpdatePreview(mousePosition, previewRotation);
            }

            if (Input.GetMouseButtonUp(0) && !buttonClickedThisUpdate && currentCanPlaceHere)
            {
                PlaceSelected(previewPosition, previewRotation);
                ResetPreviewValues();
                isDragging = false;
            }
            else if (Input.GetMouseButtonUp(0) && !currentCanPlaceHere)
            {
                savedPreviewRotation = previewRotation;
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                DeselectAll();
            }
        }

        buttonClickedThisUpdate = false;
    }

    private void PlaceSelected(Vector3 position, Quaternion rotation)
    {
        switch (selectedBuildingType)
        {
            case BuildingType.None:
                break;
            case BuildingType.Hut:
                Instantiate(hutPrefab, position, rotation);

                DeselectAll();
                break;
            case BuildingType.House:
                float newY = FlattenTerrain();
                //Debug.Log(newY);
                Vector3 newPos = new Vector3(position.x, newY, position.z);

                if (newY != 0)
                {
                    Instantiate(housePrefab, newPos, rotation);
                }

                DeselectAll();
                break;
            default:
                break;
        }
    }

    private float FlattenTerrain()
    {
        BuildingPreview previewScript = selectedObject.GetComponentInChildren<BuildingPreview>();

        List<GameObject> currentTerrain = previewScript.GetCurrentTerrains();
        List<Mesh> currentTerrainMeshes = new List<Mesh>();
        List<Vector3[]> currentTerrainVertices = new List<Vector3[]>();
        List<Vector3[]> modifiedTerrainVertices = new List<Vector3[]>();

        Bounds previewBounds = previewScript.GetNativeBounds();
        Vector2 previewBoundsCenter = new Vector2(previewBounds.center.x, previewBounds.center.z);

        Vector2 topLeft = new Vector2(previewBounds.center.x - previewBounds.extents.x, previewBounds.center.z + previewBounds.extents.z);
        Vector2 topRight = new Vector2(previewBounds.center.x + previewBounds.extents.x, previewBounds.center.z + previewBounds.extents.z);
        Vector2 botLeft = new Vector2(previewBounds.center.x - previewBounds.extents.x, previewBounds.center.z - previewBounds.extents.z);

        topLeft = HelpfullFunctions.RotatePointAroundPivot(topLeft, previewBoundsCenter, -previewRotation.eulerAngles.y);
        topRight = HelpfullFunctions.RotatePointAroundPivot(topRight, previewBoundsCenter, -previewRotation.eulerAngles.y);
        botLeft = HelpfullFunctions.RotatePointAroundPivot(botLeft, previewBoundsCenter, -previewRotation.eulerAngles.y);

        debugPoint1 = new Vector3(topLeft.x, 0, topLeft.y);
        debugPoint2 = new Vector3(topRight.x, 0, topRight.y);
        debugPoint3 = new Vector3(botLeft.x, 0, botLeft.y);

        float numberOfVertexes = 0;
        float totalY = 0;
        float averageY = 0;

        for (int i = 0; i < currentTerrain.Count; i++)
        {
            currentTerrainMeshes.Add(currentTerrain[i].GetComponent<MeshFilter>().sharedMesh);
            currentTerrainVertices.Add(currentTerrainMeshes[i].vertices);
        }

        for (int i = 0; i < currentTerrainMeshes.Count; i++)
        {
            Vector3[] terrainVertices = currentTerrainVertices[i];

            for (int j = 0; j < terrainVertices.Length; j++)
            {
                Vector3 terrainVertex = currentTerrain[i].transform.TransformPoint(terrainVertices[j]); 
                Vector2 vertex2D = new Vector2(terrainVertex.x, terrainVertex.z);

                if (HelpfullFunctions.IsPointInRectangle(topLeft, topRight, botLeft, vertex2D))
                {
                    totalY += terrainVertices[j].y;
                    numberOfVertexes++;
                }
            }
        }

        if (numberOfVertexes != 0)
        {
            averageY = totalY / numberOfVertexes;

            for (int i = 0; i < currentTerrainMeshes.Count; i++)
            {
                Vector3[] terrainVertices = currentTerrainVertices[i];

                for (int j = 0; j < terrainVertices.Length; j++)
                {
                    Vector3 terrainVertex = currentTerrain[i].transform.TransformPoint(terrainVertices[j]);
                    Vector2 vertex2D = new Vector2(terrainVertex.x, terrainVertex.z);

                    if (HelpfullFunctions.IsPointInRectangle(topLeft, topRight, botLeft, vertex2D))
                    {
                        terrainVertices[j] = new Vector3(terrainVertices[j].x, averageY, terrainVertices[j].z);
                    }
                }

                modifiedTerrainVertices.Add(terrainVertices);
            }

            for (int i = 0; i < modifiedTerrainVertices.Count; i++)
            {
                currentTerrainMeshes[i].vertices = modifiedTerrainVertices[i];
            }
        }

        return averageY;
    }

    public void ToggleHut()
    {
        buttonClickedThisUpdate = true;

        if (selectedBuildingType == BuildingType.Hut)
        {
            DeselectAll();
        }
        else
        {
            SelectNewBuildingType(hutPrefab, BuildingType.Hut);
        }
    }

    public void ToggleHouse()
    {
        buttonClickedThisUpdate = true;

        if (selectedBuildingType == BuildingType.House)
        {
            DeselectAll();
        }
        else
        {
            SelectNewBuildingType(housePrefab, BuildingType.House);
        }
    }

    private void UpdatePreview(Vector3 newPos, Quaternion newRot)
    {
        selectedObject.transform.position = newPos;
        selectedObject.transform.rotation = newRot;

        if (previewScript.CanBePlacedAtThisPosition() != currentCanPlaceHere)
        {
            currentCanPlaceHere = previewScript.CanBePlacedAtThisPosition();
            previewMaterial.color = GetCorrectColor(currentCanPlaceHere);
        }
    }

    private Color GetCorrectColor(bool canPlace)
    {
        if (canPlace)
        {
            return previewGreen;
        }
        else
        {
            return previewRed;
        }
    }

    private Vector3 CalculateCurrentMouseWorldPosition()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(inputRay, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));

        return hit.point;
    }

    private Quaternion CalculateCurrentPreviewRotation(Vector3 startPosition, Vector3 currentPosition, Quaternion currentRotation)
    {
        //Vector3 newForward = Quaternion.Euler(0, 90, 0) * selectedObject.transform.forward;
        //return Quaternion.LookRotation(newForward);

        Vector3 start = new Vector3(startPosition.x, 0, startPosition.z);
        Vector3 end = new Vector3(currentPosition.x, 0, currentPosition.z);
        Vector3 lookVector = (start - end).normalized;

        if (Vector3.Distance(start, end) < 0.05f)
        {
            return currentRotation;
        }

        return Quaternion.LookRotation(lookVector);
    }

    private void CreatePreview(GameObject previewPrefab)
    {
        Destroy(selectedObject);
        selectedObject = null;

        GameObject previewObject = new GameObject("PreviewObject");
        GameObject previewMesh = new GameObject("PreviewMesh");

        previewMesh.transform.parent = previewObject.transform;

        previewObject.layer = LayerMask.NameToLayer("BuildingPreview");
        previewMesh.layer = LayerMask.NameToLayer("BuildingPreview");

        MeshFilter meshFilter = previewMesh.AddComponent<MeshFilter>();
        previewRenderer = previewMesh.AddComponent<MeshRenderer>();

        meshFilter.mesh = previewPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        previewRenderer.material = previewMaterial;

        Rigidbody rigidbody = previewMesh.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        previewScript = previewMesh.AddComponent<BuildingPreview>();

        BoxCollider previewCollider = previewMesh.AddComponent<BoxCollider>();
        previewCollider.size = new Vector3(previewCollider.size.x + 1f, 50, previewCollider.size.z + 1f);
        previewCollider.isTrigger = true;

        previewScript.Initialize(terrainMeshFilter.sharedMesh);

        selectedObject = previewObject;
    }

    private void DeselectAll()
    {
        Destroy(selectedObject);
        selectedObject = null;
        selectedBuildingType = BuildingType.None;

        ResetPreviewValues();

        isDragging = false;
    }

    private void SelectNewBuildingType(GameObject prefab, BuildingType buildingType)
    {
        CreatePreview(prefab);
        selectedBuildingType = buildingType;

        ResetPreviewValues();
    }

    private void ResetPreviewValues()
    {
        previewRotation = Quaternion.identity;
        previewPosition = Vector3.zero;
        savedPreviewRotation = Quaternion.identity;

        isDragging = false;
    }
}
