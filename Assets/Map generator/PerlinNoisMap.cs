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

    // Tilemap for the light stone RuleTile.
    public Tilemap lightStoneTilemap;

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
        CreatTileGroups();
        GenerateMap();
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

    void CreatTileGroups()
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
    public float height = 0.1f; // Default height.
}
