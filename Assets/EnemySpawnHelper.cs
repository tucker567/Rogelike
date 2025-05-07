using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemySpawnHelper : MonoBehaviour
{
    public GameObject walkingEnemyPrefab;
    public int numberOfEnemies = 3;
    public float yOffset = 0.1f;

    public void SpawnGroundEnemies()
    {
        if (walkingEnemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned!");
            return;
        }

        PerlinNoisMap map = GetComponent<PerlinNoisMap>();
        if (map == null)
        {
            Debug.LogError("PerlinNoisMap not found on object!");
            return;
        }

        var tilemap = map.lightStoneTilemap;
        int spawned = 0;

        for (int x = -map.mapWidth / 2; x < map.mapWidth / 2 && spawned < numberOfEnemies; x++)
        {
            for (int y = -map.mapHeight / 2; y < map.mapHeight / 2 && spawned < numberOfEnemies; y++)
            {
                Vector3Int below = new Vector3Int(x, y - 1, 0);
                Vector3Int pos = new Vector3Int(x, y, 0);

                // If there’s stone below and open space here
                if (tilemap.HasTile(below) && !tilemap.HasTile(pos))
                {
                    Vector3 groundedPos = FindGroundedSpawn(new Vector3(x, y, -0.01f), tilemap);
                    GameObject enemy = Instantiate(walkingEnemyPrefab, groundedPos, Quaternion.identity);
                    enemy.name = $"WalkingEnemy_{x}_{y}";
                    spawned++;
                }
            }
        }

        Debug.Log($"Spawned {spawned} walking enemies.");
    }

    Vector3 FindGroundedSpawn(Vector3 original, Tilemap tilemap)
    {
        int maxDrop = 10;
        Vector3 checkPos = original;

        for (int i = 0; i < maxDrop; i++)
        {
            Vector3Int below = Vector3Int.FloorToInt(checkPos + Vector3.down);
            if (tilemap.HasTile(below))
            {
                return new Vector3(below.x, below.y + 1 + yOffset, original.z);
            }
            checkPos += Vector3.down;
        }

        return original;
    }
}
