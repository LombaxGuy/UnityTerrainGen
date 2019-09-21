//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MapDisplay : MonoBehaviour
//{
//    public enum DrawMode { HeightMap, FalloffMap, BiomeMap, TreeMap, RockMap, Perview2D, Mesh };

//    [Header("Rendering")]
//    public DrawMode drawMode = DrawMode.HeightMap;

//    [Header("References")]
//    public GameObject mapPreviewObject;
//    public GameObject terrainMeshObject;

//    [Space]
//    public Material terrainMaterial;

//    [Space]
//    public HeightMapSettings heightMapSettings;
//    public FalloffMapSettings falloffMapSettings;
//    public BiomeMapSettings biomeMapSettings;
//    public BiomeMapSettings rockBiomeMapSettings;
//    public ClutterMapSettings treeMapSettings;
//    public ClutterMapSettings rockMapSettings;
//    public TextureData textureSettings;
//    public PreviewMapSettings previewMapSettings;
//    public MeshSettings meshSettings;

//    [Space]
//    public Transform clutterParent;

//    [Space]
//    public GameObject[] treePrefabs;
//    public GameObject[] rockPrefabs;

//    [Header("Variables")]
//    public string mapPreviewObjectName = "Preview";
//    public string terrainObjectName = "TerrainObject";
//    public bool autoUpdate;

//    private ClutterMap savedTreeMap;
//    private ClutterMap savedRockMap;

//    private void Start()
//    {
//        textureSettings.ApplyToMaterial(terrainMaterial);
//        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeigth, heightMapSettings.maxHeigth);
//    }

//    public void DrawMapInEditor()
//    {
//        FalloffMap falloffMap = MapGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, falloffMapSettings);
//        HeightMap heightMap = MapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, falloffMap, Vector2.zero);
//        BiomeMap biomeMap = MapGenerator.GenerateBiomeMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, biomeMapSettings, Vector2.zero);
//        BiomeMap rockBiomeMap = MapGenerator.GenerateBiomeMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, rockBiomeMapSettings, Vector2.zero);
//        ClutterMap treeMap = MapGenerator.GenerateClutterMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, treeMapSettings, Vector2.zero, biomeMap);
//        ClutterMap rockMap = MapGenerator.GenerateClutterMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, rockMapSettings, Vector2.zero, rockBiomeMap);
//        PreviewMap preview = MapGenerator.GeneratePreviewMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, previewMapSettings, heightMap, textureSettings, new ClutterMap[] {rockMap, treeMap });

//        textureSettings.UpdateMeshHeights(terrainMaterial, heightMap.minValue, heightMap.maxValue);
//        textureSettings.ApplyToMaterial(terrainMaterial);

//        savedTreeMap = treeMap;
//        savedRockMap = rockMap;

//        switch (drawMode)
//        {
//            case DrawMode.HeightMap:
//                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap, Color.black, Color.white));
//                break;

//            case DrawMode.FalloffMap:
//                DrawTexture(TextureGenerator.TextureFromFloatArray2D(falloffMap.values, Color.black, Color.white));
//                break;

//            case DrawMode.BiomeMap:
//                DrawTexture(TextureGenerator.TextureFromFloatArray2D(biomeMap.values, biomeMapSettings.GetStartValues(), biomeMapSettings.GetColors()));
//                break;

//            case DrawMode.TreeMap:
//                DrawTexture(TextureGenerator.TextureFromFloatArray2D(treeMap.values, Color.black, Color.white, true));
//                break;

//            case DrawMode.RockMap:
//                DrawTexture(TextureGenerator.TextureFromFloatArray2D(rockMap.values, Color.black, Color.white, true));
//                break;

//            case DrawMode.Perview2D:
//                DrawTexture(TextureGenerator.TextureFromFloatArray2D(preview.values, preview.colorValues, preview.colors));
//                break;

//            case DrawMode.Mesh:
//                DrawMesh(MeshGenerator.GenerateTerrainMeshData(heightMap.values, meshSettings));
//                break;

//            default:
//                break;
//        }
//    }

//    public void PlaceClutter()
//    {
//        BakeCollisions();

//        ClutterMap[] clutterMaps = new ClutterMap[] { savedRockMap, savedTreeMap };
//        GameObject[][] clutterPrefabs = new GameObject[2][];
//        clutterPrefabs[0] = rockPrefabs;
//        clutterPrefabs[1] = treePrefabs;

//        ClutterPlacer.PlaceClutter(clutterMaps, meshSettings, clutterPrefabs, clutterParent);
//    }

//    public void BakeCollisions()
//    {
//        MeshGenerator.BakeCollisions(terrainMeshObject);
//    }

//    public void DrawTexture(Texture2D texture)
//    {
//        Renderer textureRenderer = mapPreviewObject.GetComponent<Renderer>() ;

//        textureRenderer.sharedMaterial.mainTexture = texture;
//        textureRenderer.transform.localScale = new Vector3(-texture.width / 10f * meshSettings.unitsPerVertex, 1, texture.height / 10f * meshSettings.unitsPerVertex);

//        mapPreviewObject.SetActive(true);
//        terrainMeshObject.SetActive(false);
//    }

//    public void DrawMesh(MeshData meshData)
//    {
//        Mesh mesh = meshData.CreateMesh();

//        MeshGenerator.UpdateMesh(terrainMeshObject, mesh, terrainMaterial);

//        mapPreviewObject.SetActive(false);
//        terrainMeshObject.SetActive(true);
//    }

//    private void OnValuesUpdated()
//    {
//        if (!Application.isPlaying)
//        {
//            DrawMapInEditor();
//        }
//    }

//    private void OnTextureValuesUpdated()
//    {
//        textureSettings.ApplyToMaterial(terrainMaterial);
//    }

//    private void OnValidate()
//    {

//        if (heightMapSettings != null)
//        {
//            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (falloffMapSettings != null)
//        {
//            falloffMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            falloffMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (biomeMapSettings != null)
//        {
//            biomeMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            biomeMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (treeMapSettings != null)
//        {
//            treeMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            treeMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (rockMapSettings != null)
//        {
//            rockMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            rockMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (textureSettings != null)
//        {
//            textureSettings.OnValuesUpdated -= OnTextureValuesUpdated;
//            textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
//        }

//        if (previewMapSettings != null)
//        {
//            previewMapSettings.OnValuesUpdated -= OnValuesUpdated;
//            previewMapSettings.OnValuesUpdated += OnValuesUpdated;
//        }

//        if (meshSettings != null)
//        {
//            meshSettings.OnValuesUpdated -= OnValuesUpdated;
//            meshSettings.OnValuesUpdated += OnValuesUpdated;
//        }
//    }
//}
