using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoisMap : MonoBehaviour
{
    [Header("Tile Prefabs")]
    // Dictionaries to store prefabs and their groups.
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;

    public GameObject prefab_light_stone; // The wall of the map, where player can't go
    public RuleTile ruletile; // Using RuleTile for Dark stone
    public GameObject prefab_Wall_test;
    public GameObject prefab_Vines;
    public GameObject prefab_Enemy;
    public GameObject prefab_Portal;
    public List<GameObject> prefab_Cameras = new List<GameObject>();
    public int chancetospawncamera = 10;
    public List<GameObject> grassVariants = new List<GameObject>(); // List of grass prefabs

    public int enemyCount = 5;

    // Tilemap for the light stone RuleTile.
    public Tilemap lightStoneTilemap;
    // % of vines to light stone tiles

    [Header("Vine Settings")]
    public float vinesToLightStoneRatio = 0.1f;
    public int minVineLength = 1;
    public int maxVineLength = 6;

    [Header("Map Settings")]
    public int mapWidth = 160;
    public int mapHeight = 90;

    [Header("Noise Settings")]
    public Dictionary<(int, int), GameObject> tile_Grid = new Dictionary<(int, int), GameObject>();
    public float magnitude = 10.0f;
    public float frequency = 1.0f;
    public float noiseThreshold = 0.5f;

    [Header("Seed Settings")]
    public int seed = -1;
    int x_offset = 0;
    int y_offset = 0;

    [Header("Erosion Settings")]
    public int numberOfDroplets = 1000;
    public float initialWaterAmount = 0.1f;
    public float evaporationRate = 0.99f;
    public float sedimentCapacity = 0.1f;
    public float erosionStrength = 0.01f;
    
    // Public variable to adjust weighted spawn chance decay for grass variants.
    public float spawnDecay = 0.5f; 

    [Header("Character Settings")]
    public List<CharacterDefinition> allCharacters;     // ← new


    void Start()
    {
        Debug.Log("Starting map generation...");

        // Generate a random seed if not provided.
        if (seed == -1)
            seed = Random.Range(0, 10000);

        x_offset = seed;
        y_offset = seed;

        Debug.Log("Using Seed: " + seed);

        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        SimulateErosion();
        PlaceGrassOnSurface();
        PlaceCamerasOnBottom();
        PlaceVinesOnRandomGrayStone();
        CreateBarrier();
        PlacePortal();
        SummonEnemy();

        Debug.Log("Map generation complete.");
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        // Assuming tile ID 0 is for dark stone (instantiated prefab)
        // and ID 1 is for light stone (using the RuleTile on the Tilemap).
        tileset.Add(0, prefab_light_stone); 
    }

    void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileGroup = new GameObject(prefab_pair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = Vector3.zero;
            tileGroups.Add(prefab_pair.Key, tileGroup);
        }
    }

    void GenerateMap()
    {
        Debug.Log("Generating Map...");
        // Create tileset and tile groups (if needed again)
        CreateTileset();
        CreateTileGroups();

        for (int x = -mapWidth / 2; x < mapWidth / 2; x++)
        {
            for (int y = -mapHeight / 2; y < mapHeight / 2; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                CreateTile(tile_id, x, y);
            }
        }
        Debug.Log("Map generation complete.");
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float raw = Mathf.PerlinNoise(
            (x + x_offset) / (magnitude * frequency),
            (y + y_offset) / (magnitude * frequency)
        );
        // If the noise value is less than the threshold, return 0 (dark stone); otherwise 1 (light stone).
        return (raw < noiseThreshold) ? 0 : 1;
    }

    void CreateTile(int tile_id, int x, int y)
    {
        if (tile_id == 1)
        {
            // For light stone, use the RuleTile on the assigned Tilemap.
            if (lightStoneTilemap != null)
            {
                lightStoneTilemap.SetTile(new Vector3Int(x, y, 0), ruletile);
            }
            else
            {
                Debug.LogWarning("Light stone Tilemap is not assigned!");
            }
        }
        else
        {
            // For dark stone, instantiate the prefab.
            GameObject tile_prefab = tileset[tile_id];
            GameObject tile_group = tileGroups[tile_id];
            GameObject tile = Instantiate(tile_prefab, tile_group.transform);
            tile.name = $"Tile_x{x}_y{y}";
            tile.transform.localPosition = new Vector3(x, y, 0);
            
            // Ensure TileProperties is set correctly
            TileProperties tp = tile.GetComponent<TileProperties>();
            if (tp == null)
            {
                tp = tile.AddComponent<TileProperties>();
            }
            tp.tileID = tile_id;
            
            tile_Grid[(x, y)] = tile;
        }
    }

    void SimulateErosion()
    {
        for (int i = 0; i < numberOfDroplets; i++)
        {
            // Initialize droplet at a random position within the map dimensions.
            Vector2 startPos = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            Droplet droplet = new Droplet(startPos, initialWaterAmount);

            while (droplet.waterAmount > 0.01f)
            {
                (int x, int y) = ((int)droplet.position.x, (int)droplet.position.y);
                if (tile_Grid.ContainsKey((x, y)))
                {
                    GameObject tile = tile_Grid[(x, y)];
                    TileProperties properties = tile.GetComponent<TileProperties>();
                    if (properties != null)
                    {
                        properties.height -= erosionStrength * droplet.sedimentAmount;
                        droplet.sedimentAmount *= 0.9f;
                    }
                }

                // Find the neighbor with the steepest descent.
                Vector2[] neighbors = {
                    new Vector2(droplet.position.x + 1, droplet.position.y),
                    new Vector2(droplet.position.x - 1, droplet.position.y),
                    new Vector2(droplet.position.x, droplet.position.y + 1),
                    new Vector2(droplet.position.x, droplet.position.y - 1)
                };

                Vector2 steepestDescent = neighbors[0];
                float steepestHeight = float.MaxValue;

                foreach (Vector2 neighbor in neighbors)
                {
                    (int nx, int ny) = ((int)neighbor.x, (int)neighbor.y);
                    if (tile_Grid.ContainsKey((nx, ny)))
                    {
                        GameObject neighborTile = tile_Grid[(nx, ny)];
                        TileProperties neighborProperties = neighborTile.GetComponent<TileProperties>();
                        if (neighborProperties != null && neighborProperties.height < steepestHeight)
                        {
                            steepestHeight = neighborProperties.height;
                            steepestDescent = neighbor;
                        }
                    }
                }

                droplet.position = steepestDescent;
                droplet.waterAmount *= evaporationRate;
            }
        }
    }

    void PlacePortal()
    {
        Debug.Log("Placing portal...");

        // Build dictionary of column x-values to available y-values where a light stone tile exists.
        Dictionary<int, List<int>> columnYValues = new Dictionary<int, List<int>>();

        // Scan the entire map width and height.
        for (int x = -mapWidth / 2; x < mapWidth / 2; x++)
        {
            for (int y = -mapHeight / 2; y < mapHeight / 2; y++)
            {
                if (lightStoneTilemap.HasTile(new Vector3Int(x, y, 0))) // Light stone tile detected.
                {
                    if (!columnYValues.ContainsKey(x))
                        columnYValues[x] = new List<int>();

                    columnYValues[x].Add(y);
                }
            }
        }

        if (columnYValues.Count == 0)
        {
            Debug.LogWarning("No light stone tiles detected on the tilemap.");
            return;
        }

        List<Vector3Int> possiblePortalPositions = new List<Vector3Int>();

        foreach (var entry in columnYValues)
        {
            int x = entry.Key;
            List<int> yValues = entry.Value;
            yValues.Sort();

            for (int i = 1; i < yValues.Count; i++)
            {
                int prevY = yValues[i - 1];
                int currentY = yValues[i];

                if (currentY - prevY > 1)
                {
                    int portalY = prevY + 1;
                    possiblePortalPositions.Add(new Vector3Int(x, portalY, 0));
                }
            }
        }

        if (possiblePortalPositions.Count > 0)
        {
            // Select a random position from the list of possible portal positions.
            Vector3Int selectedPosition = possiblePortalPositions[Random.Range(0, possiblePortalPositions.Count)];

            // Place the portal at the selected position.
            GameObject portal = Instantiate(prefab_Portal, new Vector3(selectedPosition.x, selectedPosition.y, -0.1f), Quaternion.identity);
            Debug.Log($"Portal spawned at {selectedPosition}");

            SummonPlayer(selectedPosition);
        }
        else
        {
            Debug.LogWarning("No valid portal positions found!");
        }
    }

    void SummonEnemy()
    {
        List<KeyValuePair<(int, int), GameObject>> grayStoneTiles = new List<KeyValuePair<(int, int), GameObject>>();

        // Find all gray stone tiles by tile ID (assuming 0 is dark/gray stone).
        foreach (var entry in tile_Grid)
        {
            TileProperties tileProperties = entry.Value.GetComponent<TileProperties>();
            if (tileProperties != null && tileProperties.tileID == 0)
            {
                grayStoneTiles.Add(entry);
            }
        }

        if (grayStoneTiles.Count == 0)
        {
            Debug.LogWarning("No gray stone tiles found.");
            return;
        }

        Debug.Log($"Number of gray stone tiles found: {grayStoneTiles.Count}");

        int spawnedEnemies = 0;
        int attempts = 0;
        int maxAttempts = enemyCount * 10; // Prevent potential infinite loops

        while (spawnedEnemies < enemyCount && attempts < maxAttempts)
        {
            attempts++;
            KeyValuePair<(int, int), GameObject> randomTile = grayStoneTiles[Random.Range(0, grayStoneTiles.Count)];
            (int x, int y) = randomTile.Key;

            // Ensure no light stone tile is present above.
            if (lightStoneTilemap.HasTile(new Vector3Int(x, y + 1, 0)))
            {
                continue;
            }

            int enemyY = y + 1; // Spawn enemy above the gray stone tile

            // Check again if a light stone tile is present at the enemy spawn position.
            if (lightStoneTilemap.HasTile(new Vector3Int(x, enemyY, 0)))
            {
                continue;
            }

            if (prefab_Enemy == null)
            {
                Debug.LogWarning("Enemy prefab is not assigned in the inspector.");
                return;
            }

            GameObject enemy = Instantiate(prefab_Enemy, new Vector3(x, enemyY, -0.01f), Quaternion.identity);
            enemy.name = $"Enemy_x{x}_y{enemyY}";
            tile_Grid[(x, enemyY)] = enemy;
            spawnedEnemies++;
        }

        if (spawnedEnemies < enemyCount)
        {
            Debug.LogWarning($"Only spawned {spawnedEnemies} out of the requested {enemyCount} enemies.");
        }
    }

// put this at class scope (same level as PlacePortal)
void SummonPlayer(Vector3Int portalPos)
{
    /* 1) which character? */
    int idx = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
    if (idx < 0 || idx >= allCharacters.Count) idx = 0;

    /* 2) definition */
    CharacterDefinition def = allCharacters[idx];

    /* 3) spawn prefab */
    GameObject player = Instantiate(def.characterPrefab, transform);
    player.transform.position = new Vector3(portalPos.x, portalPos.y + 0.1f, 0f);

    /* 4) apply base stats (optional) */
    var stats = player.GetComponent<PlayerStatsEffects>();
    if (stats != null)
    {
        stats.moveSpeed  = def.baseMoveSpeed;
        stats.jumpHeight = def.baseJumpHeight;
        stats.ResetStatsToBase();
    }

    /* 5) sorting layer tweak */
    var sr = player.GetComponent<SpriteRenderer>();
    if (sr) { sr.sortingLayerName = "Player"; sr.sortingOrder = 1; }
}



    public class Droplet
    {
        public Vector2 position;
        public float waterAmount;
        public float sedimentAmount;

        public Droplet(Vector2 startPos, float initialWaterAmount)
        {
            position = startPos;
            waterAmount = initialWaterAmount;
            sedimentAmount = 0;
        }
    }

    public class TileProperties : MonoBehaviour
    {
        public int tileID;
        public float height = 0.1f;
    }

    void PlaceGrassOnSurface()
    {
        if (grassVariants == null || grassVariants.Count == 0)
        {
            Debug.LogWarning("No grass variants assigned. Assign prefabs in the Inspector.");
            return;
        }

        Dictionary<int, List<int>> columnYValues = new Dictionary<int, List<int>>();

        // Scan the entire map width and height.
        for (int x = -mapWidth / 2; x < mapWidth / 2; x++)
        {
            for (int y = -mapHeight / 2; y < mapHeight / 2; y++)
            {
                if (lightStoneTilemap.HasTile(new Vector3Int(x, y, 0))) // Light stone tile detected.
                {
                    if (!columnYValues.ContainsKey(x))
                        columnYValues[x] = new List<int>();

                    columnYValues[x].Add(y);
                }
            }
        }

        if (columnYValues.Count == 0)
        {
            Debug.LogWarning("No light stone tiles detected on the tilemap.");
            return;
        }

        GameObject grassParent = new GameObject("GrassTiles");
        grassParent.transform.parent = transform;

        foreach (var entry in columnYValues)
        {
            int x = entry.Key;
            List<int> yValues = entry.Value;
            yValues.Sort();

            for (int i = 1; i < yValues.Count; i++) 
            {
                int prevY = yValues[i - 1];
                int currentY = yValues[i];

                if (currentY - prevY > 1)
                {
                    int grassY = prevY + 1; 

                    // Use weighted spawn chance for selecting a grass variant.
                    float totalWeight = 0f;
                    for (int j = 0; j < grassVariants.Count; j++)
                    {
                        totalWeight += Mathf.Pow(spawnDecay, j);
                    }

                    float randomValue = Random.Range(0f, totalWeight);
                    GameObject selectedGrass = grassVariants[grassVariants.Count - 1]; // Default to the last element.

                    for (int j = 0; j < grassVariants.Count; j++)
                    {
                        float weight = Mathf.Pow(spawnDecay, j);
                        if (randomValue < weight)
                        {
                            selectedGrass = grassVariants[j];
                            break;
                        }
                        randomValue -= weight;
                    }

                    Vector3 grassPosition = new Vector3(x, grassY, -0.01f);
                    GameObject grass = Instantiate(selectedGrass, grassPosition, Quaternion.identity);
                    grass.transform.parent = grassParent.transform;
                    grass.name = $"Grass_x{x}_y{grassY}";
                }
            }
        }

        Debug.Log("Grass placement complete!");
    }

    void PlaceVinesOnRandomGrayStone()
    {
        List<KeyValuePair<(int, int), GameObject>> grayStoneTiles = new List<KeyValuePair<(int, int), GameObject>>();

        // Find all gray stone tiles by tile ID (0).
        foreach (var entry in tile_Grid)
        {
            TileProperties tileProperties = entry.Value.GetComponent<TileProperties>();
            if (tileProperties != null && tileProperties.tileID == 0)
            {
                grayStoneTiles.Add(entry);
            }
        }

        if (grayStoneTiles.Count == 0)
        {
            Debug.LogWarning("No gray stone tiles found.");
            return;
        }

        Debug.Log($"Number of gray stone tiles found: {grayStoneTiles.Count}");
        int numVines = Mathf.CeilToInt(vinesToLightStoneRatio * grayStoneTiles.Count);

        GameObject vinesParent = new GameObject("Vines");
        vinesParent.transform.parent = transform;

        for (int i = 0; i < numVines; i++)
        {
            KeyValuePair<(int, int), GameObject> randomTile = grayStoneTiles[Random.Range(0, grayStoneTiles.Count)];
            (int x, int y) = randomTile.Key;

            if (lightStoneTilemap.HasTile(new Vector3Int(x, y + 1, 0)))
            {
                continue;
            }

            if (prefab_Vines == null)
            {
                Debug.LogWarning("Vine prefab is not assigned in the inspector.");
                return;
            }

            int vineLength = Random.Range(minVineLength, maxVineLength + 1);

            for (int j = 1; j <= vineLength; j++)
            {
                int vineY = y + j;
                if (vineY < -mapHeight / 2 || vineY > mapHeight / 2)
                    break;

                if (lightStoneTilemap.HasTile(new Vector3Int(x, vineY, 0)))
                    break;

                GameObject vines = Instantiate(prefab_Vines, new Vector3(x, vineY, -0.01f), Quaternion.identity);
                vines.name = $"Vines_x{x}_y{vineY}";
                vines.transform.parent = vinesParent.transform;
                tile_Grid[(x, vineY)] = vines;
            }
        }
    }

    void PlaceCamerasOnBottom()
    {
        GameObject cameraParent = new GameObject("Cameras");
        cameraParent.transform.parent = transform;

        // Loop through columns (x-values)
        for (int x = -mapWidth / 2; x < mapWidth / 2; x++)
        {
            int? lastSolidTileY = null;

            // Loop from top to bottom to detect height jumps
            for (int y = mapHeight / 2; y >= -mapHeight / 2; y--)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (lightStoneTilemap.HasTile(tilePosition)) 
                {
                    if (lastSolidTileY != null && y < lastSolidTileY - 1)
                    {
                        if (Random.Range(0, chancetospawncamera) == 0)
                        {
                            Vector3 cameraPosition = new Vector3(x, lastSolidTileY.Value - 1, -0.01f);
                            GameObject selectedCameraPrefab = prefab_Cameras[Random.Range(0, prefab_Cameras.Count)];
                            
                            GameObject camera = Instantiate(selectedCameraPrefab, cameraPosition, Quaternion.identity);
                            camera.transform.parent = cameraParent.transform;
                            camera.name = $"Camera_x{x}_y{lastSolidTileY.Value - 1}";

                            tile_Grid[(x, lastSolidTileY.Value - 1)] = camera;
                        }
                    }
                    lastSolidTileY = y;
                }
            }
        }
        Debug.Log("Cameras placement complete!");
    }

    void CreateBarrier()
    {
        GameObject wallGroup = new GameObject("BoundaryWalls");
        wallGroup.transform.parent = transform;
        wallGroup.transform.localPosition = Vector3.zero;

        // Create walls along the left and right edges.
        for (int y = -mapHeight / 2 - 1; y <= mapHeight / 2; y++)
        {
            CreateWallTile(-mapWidth / 2 - 1, y, wallGroup);
            CreateWallTile(mapWidth / 2, y, wallGroup);
        }

        // Create walls along the top and bottom edges.
        for (int x = -mapWidth / 2 - 1; x <= mapWidth / 2; x++)
        {
            CreateWallTile(x, -mapHeight / 2 - 1, wallGroup);
            CreateWallTile(x, mapHeight / 2, wallGroup);
        }
    }

    void CreateWallTile(int x, int y, GameObject wallGroup)
    {
        GameObject wallTile = Instantiate(prefab_Wall_test, wallGroup.transform);
        wallTile.name = $"Wall_{x}_{y}";
        wallTile.transform.localPosition = new Vector3(x, y, 0);
        tile_Grid[(x, y)] = wallTile;
    }
}
