using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoisMap : MonoBehaviour
{
    // Dictionaries to store prefabs and their groups.
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;

    public GameObject prefab_light_stone; // The wall of the map, where player can't go
    public RuleTile ruletile; // Using RuleTile for Dark stone
    public GameObject prefab_Wall_test;
    public GameObject prefab_Player;
    public GameObject prefab_Vines;
    public GameObject prefab_Portal;
    public List<GameObject> prefab_Cameras = new List<GameObject>();
    public int chancetospawncamera = 10;
    public List<GameObject> grassVariants = new List<GameObject>(); // List of grass prefabs

    // Tilemap for the light stone RuleTile.
    public Tilemap lightStoneTilemap;
    // % of vines to light stone tiles
    public float vinesToLightStoneRatio = 0.1f;
    // min vine length
    public int minVineLength = 1;
    // max vine length
    public int maxVineLength = 6;

    // Map dimensions (the map will be exactly mapWidth x mapHeight).
    public int mapWidth = 160;
    public int mapHeight = 90;

    // Noise parameters
    public Dictionary<(int, int), GameObject> tile_Grid = new Dictionary<(int, int), GameObject>();
    public float magnitude = 10.0f;
    public float frequency = 1.0f;
    
    // Adjust this threshold so that lower Perlin values produce dark stone.
    // If you don't see any dark stone, try increasing it (e.g., to 0.6 or 0.7).
    public float noiseThreshold = 0.5f;

    // Seed for randomization (-1 generates a random seed).
    public int seed = -1;
    int x_offset = 0;
    int y_offset = 0;

    // Erosion parameters
    public int numberOfDroplets = 1000;
    public float initialWaterAmount = 0.1f;
    public float evaporationRate = 0.99f;
    public float sedimentCapacity = 0.1f;
    public float erosionStrength = 0.01f;
    
    // Public variable to adjust weighted spawn chance decay for grass variants.
    // Lower values give a steeper drop-off in spawn chance.
    public float spawnDecay = 0.5f; 

    void Start()
    {
        Debug.Log("Starting map generation...");

        // Generate a random seed if not provided.
        if (seed == -1)
            seed = UnityEngine.Random.Range(0, 10000);

        x_offset = seed;
        y_offset = seed;

        Debug.Log("Using Seed: " + seed);

        CreateTileset();
        CreateTileGroups();
        GenerateMap();
        PlaceGrassOnSurface();
        PlaceCamerasOnBottom();
        PlaceVinesOnRandomGrayStone();
        CreateBarrier();

        Debug.Log("Map generation complete.");
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

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_light_stone); // Using prefab for Light stone
        // Note: The RuleTile for light stone is handled separately.
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
        // Loop over x and y from -mapWidth/2 to mapWidth/2 and -mapHeight/2 to mapHeight/2.
        for (int x = -mapWidth / 2; x < mapWidth / 2; x++)
        {
            for (int y = -mapHeight / 2; y < mapHeight / 2; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                if (!tile_Grid.ContainsKey((x, y)) || tile_Grid[(x, y)] == null)
                {
                    CreateTile(tile_id, x, y);
                }
            }
        }

        SimulateErosion(); // Apply erosion after map generation.
        PlacePortal(); // Place the portal after erosion.
        
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

        // Place the portal at the selected position
        GameObject portal = Instantiate(prefab_Portal, new Vector3(selectedPosition.x, selectedPosition.y, + 0.1f), Quaternion.identity);
        Debug.Log($"Portal spawned at {selectedPosition}");

        SummonPlayer(selectedPosition);
    }
    else
    {
        Debug.LogWarning("No valid portal positions found!");
    }
}

void SummonPlayer(Vector3Int chosenPosition)
{
    GameObject player = Instantiate(prefab_Player, transform);
    player.transform.position = new Vector3(chosenPosition.x, chosenPosition.y + 0.2f, 0);

    SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
    playerRenderer.sortingLayerName = "Player"; // Set to the layer you defined
    playerRenderer.sortingOrder = 1; // A higher number to ensure it's drawn on top
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
        public int tileID; // Add tileID property.
        public float height = 0.1f; // Default height.
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

        // Step 1: Find all gray stone tiles by tile ID.
        foreach (var entry in tile_Grid)
        {
            TileProperties tileProperties = entry.Value.GetComponent<TileProperties>();

            if (tileProperties != null && tileProperties.tileID == 0) // Assuming 0 is gray stone.
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

        // Create a parent object for vines (for better organization).
        GameObject vinesParent = new GameObject("Vines");
        vinesParent.transform.parent = transform;

        for (int i = 0; i < numVines; i++)
        {
            KeyValuePair<(int, int), GameObject> randomTile = grayStoneTiles[Random.Range(0, grayStoneTiles.Count)];
            (int x, int y) = randomTile.Key;

            // Step 2: Ensure no light stone tile is present above.
            if (lightStoneTilemap.HasTile(new Vector3Int(x, y + 1, 0)))
            {
                continue;
            }

            // Step 3: Instantiate vines above the selected gray stone tile.
            if (prefab_Vines == null)
            {
                Debug.LogWarning("Vine prefab is not assigned in the inspector.");
                return;
            }

            // Determine the random length for the vine.
            int vineLength = Random.Range(minVineLength, maxVineLength + 1);

            for (int j = 1; j <= vineLength; j++)
            {
                int vineY = y + j;

                // Ensure the vine does not grow outside the map boundaries.
                if (vineY < -mapHeight / 2 || vineY > mapHeight / 2)
                {
                    break;
                }

                // Ensure no light stone tile is present at the current position.
                if (lightStoneTilemap.HasTile(new Vector3Int(x, vineY, 0)))
                {
                    break;
                }

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
        int? lastSolidTileY = null; // Track the last solid tile’s y-coordinate

        // Loop from top to bottom to detect height jumps
        for (int y = mapHeight / 2; y >= -mapHeight / 2; y--)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 0);

            if (lightStoneTilemap.HasTile(tilePosition)) 
            {
                // If there was a previous solid tile and the gap between them is more than 1, it's a height jump
                if (lastSolidTileY != null && y < lastSolidTileY - 1)
                {
                    // Check if we should spawn a camera based on the chance
                    if (Random.Range(0, chancetospawncamera) == 0)
                    {
                        Vector3 cameraPosition = new Vector3(x, lastSolidTileY.Value - 1, -0.01f);
                        
                        // Select a random camera prefab from the list
                        GameObject selectedCameraPrefab = prefab_Cameras[Random.Range(0, prefab_Cameras.Count)];
                        
                        GameObject camera = Instantiate(selectedCameraPrefab, cameraPosition, Quaternion.identity);
                        camera.transform.parent = cameraParent.transform;
                        camera.name = $"Camera_x{x}_y{lastSolidTileY.Value - 1}";

                        // Store camera in tile Grid
                        tile_Grid[(x, lastSolidTileY.Value - 1)] = camera;
                    }
                }

                lastSolidTileY = y; // Update last solid tile position
            }
        }
    }
    Debug.Log("Cameras placement complete!");
}

}
