using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class WorldBuilder : MonoBehaviour
{
    public float worldMaxHeight = 16;
    public float worldMinHeight = 0;
    public float seaLevel = 2;
    [Space]

    public Material worldMeshMaterial;
    public GameObject[] rockPrefabs;
    public GameObject[] treePrefabs;
    public GameObject waterPrefab;
    public GameObject seaFloorPrefab;
    public GameObject canvasMapGenerator;
    [Space]
    public PreviewMapSettings previewSettings;
    public MeshSettings meshSettings;
    [Space]
    public HeightMapSettings defaultHeightMapSettings;
    public FalloffMapSettings defaultFalloffMapSettings;
    public BiomeMapSettings defaultTreeBiomeMapSettings;
    public BiomeMapSettings defaultRockBiomeMapSettings;
    public ClutterMapSettings defaultTreeMapSettings;
    public ClutterMapSettings defaultRockMapSettings;

    [Space]
    public HeightMapSettings currentHeightMapSettings;
    public FalloffMapSettings currentFalloffMapSettings;
    public BiomeMapSettings currentTreeBiomeMapSettings;
    public BiomeMapSettings currentRockBiomeMapSettings;
    public ClutterMapSettings currentTreeMapSettings;
    public ClutterMapSettings currentRockMapSettings;
    public TextureData currentTextureSettings;
    [Space]
    public RawImage worldPreviewImage;
    public RawImage heightMapPreviewImage;
    public RawImage falloffMapPreviewImage;
    public RawImage treeBiomeMapPreviewImage;
    public RawImage treeClutterMapPreviewImage;
    public RawImage rockBiomeMapPreviewImage;
    public RawImage rockClutterMapPreviewImage;
    [Space]
    public InputField seedInputField;
    public Slider islandSizeSlider;
    public Dropdown isladShapeDropdown;
    public Slider treeDensitySlider;
    public Slider rockDensitySlider;

    public Text treeMapSeedText;
    public Text rockMapSeedText;
    public Text treeBiomeMapSeedText;
    public Text rockBiomeMapSeedText;

    private HeightMap heightMap;
    private HeightMap[,] chunkHeightMaps;
    private FalloffMap falloffMap;
    private FalloffMap normalizedFalloffMap;
    private BiomeMap treeBiomeMap;
    private BiomeMap rockBiomeMap;
    private ClutterMap treeClutterMap;
    private ClutterMap rockClutterMap;

    private PreviewMap previewMap;

    [HideInInspector]
    public int treeMapSeed;
    [HideInInspector]
    public int rockMapSeed;
    [HideInInspector]
    public int treeBiomeMapSeed;
    [HideInInspector]
    public int rockBiomeMapSeed;

    private int mapWidth;
    private int mapHeight;
    private int clutterMapWidth;

    private void OnEnable()
    {
        EventManager.OnFalloffMapSettingsChanged += OnFalloffMapSettingsChanged;
        EventManager.OnHeightMapSettingsChanged += OnHeightMapSettingsChanged;
    }

    private void OnDisable()
    {
        EventManager.OnFalloffMapSettingsChanged -= OnFalloffMapSettingsChanged;
        EventManager.OnHeightMapSettingsChanged -= OnHeightMapSettingsChanged;
    }

    private void Awake()
    {
        mapWidth = meshSettings.numVertsPerLine * meshSettings.worldChunkWidth;
        mapHeight = meshSettings.numVertsPerLine * meshSettings.worldChunkWidth;

        clutterMapWidth = Mathf.CeilToInt(meshSettings.meshWorldSize * meshSettings.worldChunkWidth);
    }

    private void Start()
    {
        CreateMaps();

        InitializeUI();

        LoadDefaultSettings();

        UpdateAllMaps();
    }

    private void InitializeUI()
    {
        seedInputField.GetComponent<UIController>().SetData(currentHeightMapSettings.noiseSettings.seed);

        islandSizeSlider.GetComponent<UIController>().SetData(currentFalloffMapSettings.falloffSize);
        isladShapeDropdown.GetComponent<UIController>().SetData(currentFalloffMapSettings.falloffShape);

        treeDensitySlider.GetComponent<UIController>().SetData(currentTreeMapSettings.density);
        rockDensitySlider.GetComponent<UIController>().SetData(currentRockMapSettings.density);

        treeBiomeMapSeedText.text = treeBiomeMapSeed.ToString();
        rockBiomeMapSeedText.text = rockBiomeMapSeed.ToString();

        treeMapSeedText.text = treeMapSeed.ToString();
        rockMapSeedText.text = rockMapSeed.ToString();
    }

    public void LoadDefaultSettings()
    {
        #region HeightMapSettings
        currentHeightMapSettings.noiseSettings.seed = defaultHeightMapSettings.noiseSettings.seed;
        #endregion

        #region FalloffSettings
        currentFalloffMapSettings.falloffShape = defaultFalloffMapSettings.falloffShape;
        currentFalloffMapSettings.falloffSize = defaultFalloffMapSettings.falloffSize;
        #endregion

        #region ClutterSettings
        currentTreeMapSettings.density = defaultTreeMapSettings.density;
        currentRockMapSettings.density = defaultRockMapSettings.density;

        currentTreeMapSettings.density = defaultTreeMapSettings.density;
        currentRockMapSettings.density = defaultRockMapSettings.density;
        #endregion

        CreateMaps();
        InitializeUI();
        UpdateAllMaps();
    }

    public void RandomizeHeightMapSeed()
    {
        currentHeightMapSettings.noiseSettings.seed = Random.Range(MapGenerator.minSeedValue, MapGenerator.maxSeedValue + 1);

        seedInputField.GetComponent<UIController>().SetData(currentHeightMapSettings.noiseSettings.seed);

        UpdateHeightMap();
    }

    public void GenerateWorld()
    {
        GameObject world = new GameObject("World");

        GenerateChunks(world.transform);

        currentTextureSettings.UpdateMeshHeights(worldMeshMaterial, worldMinHeight, worldMaxHeight);
        currentTextureSettings.ApplyToMaterial(worldMeshMaterial);

        Instantiate(waterPrefab, new Vector3(0, seaLevel, 0), Quaternion.identity, world.transform);

        Instantiate(seaFloorPrefab, new Vector3(0, -0.05f, 0), Quaternion.identity, world.transform);

        Transform clutterParent = new GameObject("Clutter").transform;
        clutterParent.parent = world.transform;

        GameObject[][] clutterPrefabs = new GameObject[2][];
        clutterPrefabs[0] = rockPrefabs;
        clutterPrefabs[1] = treePrefabs;

        ClutterPlacer.PlaceClutter(new ClutterMap[] { rockClutterMap, treeClutterMap }, meshSettings, clutterPrefabs, clutterParent);

        Camera.main.GetComponent<SimpleCameraController>().enabled = true;
        canvasMapGenerator.SetActive(false);
        gameObject.SetActive(false);
    }

    private List<GameObject> GenerateChunks(Transform parent)
    {
        chunkHeightMaps = MapGenerator.GenerateHeightMapArray2D(heightMap, meshSettings.worldChunkWidth);

        List<GameObject> chunks = new List<GameObject>();

        int maxIndex = meshSettings.worldChunkWidth - 1;

        for (int x = 0; x < meshSettings.worldChunkWidth; x++)
        {
            for (int z = 0; z < meshSettings.worldChunkWidth; z++)
            {
                Vector2 chunkCoordinate = new Vector2(x, maxIndex - z); // To get the correct setup for the map we need to inverte z
                TerrainChunk newChunk = new TerrainChunk(chunkCoordinate, chunkHeightMaps[x, z].values, meshSettings, parent, worldMeshMaterial);

                newChunk.meshObject.AddComponent<NavMeshSurface>();

                chunks.Add(newChunk.meshObject);
            }
        }

        return chunks;
    }

    #region Event handelers
    public void OnFalloffMapSettingsChanged()
    {
        currentFalloffMapSettings.falloffSize = (float)islandSizeSlider.GetComponent<UIController>().GetData();
        currentFalloffMapSettings.falloffShape = (FalloffShape)isladShapeDropdown.GetComponent<UIController>().GetData();

        UpdateFalloffMap();
    }

    public void OnHeightMapSettingsChanged()
    {
        currentHeightMapSettings.noiseSettings.seed = (int)seedInputField.GetComponent<UIController>().GetData();

        UpdateHeightMap();
    }

    public void OnTreeClutterMapSettingsChanged()
    {
        currentTreeMapSettings.density = (float)treeDensitySlider.GetComponent<UIController>().GetData();

        UpdateClutterMaps();
    }

    public void OnRockClutterMapSettingsChanged()
    {
        currentRockMapSettings.density = (float)rockDensitySlider.GetComponent<UIController>().GetData();

        UpdateClutterMaps();
    }

    #endregion

    #region Create functions
    private void CreateMaps()
    {
        falloffMap = MapGenerator.GenerateFalloffMap(mapWidth, currentFalloffMapSettings);
        heightMap = MapGenerator.GenerateHeightMap(mapWidth, mapHeight, currentHeightMapSettings, falloffMap, Vector2.zero);

        treeBiomeMap = MapGenerator.GenerateBiomeMap(clutterMapWidth, clutterMapWidth, currentTreeBiomeMapSettings, Vector2.zero, true);
        rockBiomeMap = MapGenerator.GenerateBiomeMap(clutterMapWidth, clutterMapWidth, currentRockBiomeMapSettings, Vector2.zero, true);
        treeClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentTreeMapSettings, Vector2.zero, treeBiomeMap, true);
        rockClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentRockMapSettings, Vector2.zero, rockBiomeMap, true);

        previewMap = MapGenerator.GeneratePreviewMap(mapWidth, mapHeight, worldMinHeight, worldMaxHeight, previewSettings, heightMap, currentTextureSettings/*, new ClutterMap[] { rockClutterMap, treeClutterMap }*/);

        //treeBiomeMapSeed = currentTreeBiomeMapSettings.noiseSettings.seed;
        //rockBiomeMapSeed = currentRockBiomeMapSettings.noiseSettings.seed;

        //treeMapSeed = currentTreeMapSettings.noiseSettings.seed;
        //rockMapSeed = currentRockMapSettings.noiseSettings.seed;
    }

    public void CreateNewBiomeMaps()
    {
        treeBiomeMap = MapGenerator.GenerateBiomeMap(clutterMapWidth, clutterMapWidth, currentTreeBiomeMapSettings, Vector2.zero, true);
        rockBiomeMap = MapGenerator.GenerateBiomeMap(clutterMapWidth, clutterMapWidth, currentRockBiomeMapSettings, Vector2.zero, true);

        treeBiomeMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(treeBiomeMap.values, currentTreeBiomeMapSettings.GetStartValues(), currentTreeBiomeMapSettings.GetColors());
        rockBiomeMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(rockBiomeMap.values, currentRockBiomeMapSettings.GetStartValues(), currentRockBiomeMapSettings.GetColors());

        treeBiomeMapSeed = currentTreeBiomeMapSettings.noiseSettings.seed;
        rockBiomeMapSeed = currentRockBiomeMapSettings.noiseSettings.seed;

        treeBiomeMapSeedText.text = treeBiomeMapSeed.ToString();
        rockBiomeMapSeedText.text = rockBiomeMapSeed.ToString();

        UpdateClutterMaps();
    }

    public void CreateNewClutterMaps()
    {
        treeClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentTreeMapSettings, Vector2.zero, treeBiomeMap, true);
        rockClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentRockMapSettings, Vector2.zero, rockBiomeMap, true);

        treeClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(treeClutterMap.values, Color.black, Color.white, true);
        rockClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(rockClutterMap.values, Color.black, Color.white, true);

        //treeMapSeed = currentTreeMapSettings.noiseSettings.seed;
        //rockMapSeed = currentRockMapSettings.noiseSettings.seed;

        treeMapSeedText.text = treeMapSeed.ToString();
        rockMapSeedText.text = rockMapSeed.ToString();

        UpdatePreviewMap();
    }

    #endregion

    #region Update functions
    private void UpdateAllMaps()
    {
        falloffMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(falloffMap.values, Color.black, Color.white);
        heightMapPreviewImage.texture = TextureGenerator.TextureFromHeightMap(heightMap, Color.black, Color.white);

        treeBiomeMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(treeBiomeMap.values, currentTreeBiomeMapSettings.GetStartValues(), currentTreeBiomeMapSettings.GetColors());
        treeClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(treeClutterMap.values, Color.black, Color.white, true);

        rockBiomeMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(rockBiomeMap.values, currentRockBiomeMapSettings.GetStartValues(), currentRockBiomeMapSettings.GetColors());
        rockClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(rockClutterMap.values, Color.black, Color.white, true);

        worldPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(previewMap.values, previewMap.colorValues, previewMap.colors);
    }

    private void UpdateFalloffMap()
    {
        falloffMap = MapGenerator.GenerateFalloffMap(mapWidth, currentFalloffMapSettings);

        falloffMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(falloffMap.values, Color.black, Color.white);

        UpdateHeightMap();
    }

    private void UpdateHeightMap()
    {
        heightMap = MapGenerator.GenerateHeightMap(mapWidth, mapHeight, currentHeightMapSettings, falloffMap, Vector2.zero);

        heightMapPreviewImage.texture = TextureGenerator.TextureFromHeightMap(heightMap, Color.black, Color.white);

        UpdateClutterMaps();
    }

    private void UpdatePreviewMap()
    {
        previewMap = MapGenerator.GeneratePreviewMap(mapWidth, mapHeight, worldMinHeight, worldMaxHeight, previewSettings, heightMap, currentTextureSettings/*, new ClutterMap[] { rockClutterMap, treeClutterMap }*/);

        worldPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(previewMap.values, previewMap.colorValues, previewMap.colors);
    }

    private void UpdateClutterMaps()
    {
        treeClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentTreeMapSettings, Vector2.zero, treeBiomeMap);
        rockClutterMap = MapGenerator.GenerateClutterMap(clutterMapWidth, clutterMapWidth, currentRockMapSettings, Vector2.zero, rockBiomeMap);

        treeClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(treeClutterMap.values, Color.black, Color.white, true);
        rockClutterMapPreviewImage.texture = TextureGenerator.TextureFromFloatArray2D(rockClutterMap.values, Color.black, Color.white, true);

        UpdatePreviewMap();
    }

    #endregion

    public void ExitApplication()
    {
        Application.Quit();
    }
}
