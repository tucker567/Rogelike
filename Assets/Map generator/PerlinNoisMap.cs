using System.Collections.Generic;
using UnityEngine;


public class PerlinNoisMap : MonoBehaviour
{
    // Dictionary to store prefabs, can add more prefabs if needed
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;


    public GameObject prefab_Dark_stone;
    public GameObject prefab_Light_stone;
    public GameObject prefab_Grass;
    public GameObject prefab_Wall_test;

    // Map size
    public int mapWidth = 160;
    public int mapHeight = 90;


    // Noise parameters
    public Dictionary<(int, int), GameObject> tile_Grid = new Dictionary<(int, int), GameObject>();


    // Recommended to use a value between 4.0 and 2.0
    public float magnitude = 10.0f;
    public float frequency = 1.0f; // Add frequency variable


    // Seed for randomization
    public int seed = -1; // -1 means it will generate a random seed


    int x_offset = 0; // <-, +>
    int y_offset = 0; // v-, +^


    // Erosion parameters
    public int numberOfDroplets = 1000; // Number of water droplets
    public float initialWaterAmount = 0.1f; // Initial water amount per droplet
    public float evaporationRate = 0.99f; // Rate at which water evaporates
    public float sedimentCapacity = 0.1f; // Maximum sediment a droplet can carry
    public float erosionStrength = 0.01f; // Strength of the erosion effect


    void Start()
    {
        Debug.Log("Starting map generation...");
        // If no seed is provided, generate a random one
        if (seed == -1)
            seed = UnityEngine.Random.Range(0, 10000); // Generates a random seed for variety


        x_offset = seed;
        y_offset = seed;


        Debug.Log("Using Seed: " + seed); // Log seed for debugging


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

        // Create walls along the left and right edges
        for (int y = -mapHeight; y < mapHeight; y++)
        {
            CreateWallTile(-mapWidth, y, wallGroup);
            CreateWallTile(mapWidth - 1, y, wallGroup);
        }

        // Create walls along the top and bottom edges
        for (int x = -mapWidth; x < mapWidth; x++)
        {
            CreateWallTile(x, -mapHeight, wallGroup);
            CreateWallTile(x, mapHeight - 1, wallGroup);
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
        tileset.Add(0, prefab_Dark_stone);
        tileset.Add(1, prefab_Light_stone);
    }


    void CreatTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tileGroup = new GameObject(prefab_pair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = new Vector3(0, 0, 0);
            tileGroups.Add(prefab_pair.Key, tileGroup);
        }
    }


    void GenerateMap()
    {
        for (int x = -mapWidth; x < mapWidth; x++)
        {
            for (int y = -mapHeight; y < mapHeight; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y);
                if (!tile_Grid.ContainsKey((x, y)) || tile_Grid[(x, y)] == null)
                {
                    CreateTile(tile_id, x, y);
                }
            }
        }


        SimulateErosion(); // Apply erosion after map generation
    }


    int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            (x + x_offset) / (magnitude * frequency), // Apply frequency to the Perlin noise calculation
            (y + y_offset) / (magnitude * frequency)
        );


        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);
        float scale_perlin = Mathf.Floor(clamp_perlin * tileset.Count);


        if (scale_perlin == 2)
        {
            scale_perlin = 1;
        }


        return Mathf.FloorToInt(scale_perlin);
    }


    void CreateTile(int tile_id, int x, int y)
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tileGroups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
        // Add the tile to the tile_Grid dictionary


        tile.name = string.Format("Tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);


        tile_Grid[(x, y)] = tile;
    }
    
    void check_tile(int x, int y)
    {
        foreach (var tileEntry in tile_Grid)
        {
            GameObject tile = tileEntry.Value;
            if (tile != null && tile.name.StartsWith("Tile_x") && tile.name.Contains("_y"))
            {
                // Check if the tile is an instance of the Lightstone prefab
                if (tile.CompareTag("Lightstone"))
                {
                    // Perform your logic here
                    Debug.Log($"Lightstone tile found at ({tileEntry.Key.Item1}, {tileEntry.Key.Item2})");

                }
            }
        }
    }

    void SimulateErosion()
    {
        for (int i = 0; i < numberOfDroplets; i++)
        {
            // Initialize droplet at a random position
            Vector2 startPos = new Vector2(Random.Range(-mapWidth, mapWidth), Random.Range(-mapHeight, mapHeight));
            Droplet droplet = new Droplet(startPos, initialWaterAmount);


            // Simulate droplet movement
            while (droplet.waterAmount > 0.01f)
            {
                // Get the current tile at the droplet's position
                (int x, int y) = ((int)droplet.position.x, (int)droplet.position.y);
                if (tile_Grid.ContainsKey((x, y)))
                {
                    GameObject tile = tile_Grid[(x, y)];
                    TileProperties properties = tile.GetComponent<TileProperties>();
                    if (properties != null)
                    {
                        // Erode the tile based on the droplet's sediment capacity and water amount
                        properties.height -= erosionStrength * droplet.sedimentAmount;
                        droplet.sedimentAmount *= 0.9f; // Sediment carried decreases over time
                    }
                }


                // Move the droplet to a neighboring tile with the steepest descent
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
                droplet.waterAmount *= evaporationRate; // Water evaporates over time
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
    public float height = 0.0f; // Default height
}


