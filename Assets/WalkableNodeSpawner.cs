using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkableNodeSpawner : MonoBehaviour
{
    public GameObject walkablePrefab; // Assign your WalkableProxy prefab
    public float nodeYOffset = 0.1f;  // Slight Y offset to prevent z-fighting
    public Transform walkableParent; // Empty GameObject to group all nodes

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f); 
        SpawnNodes();
    }

void SpawnNodes()
{
    var perlinMap = GetComponent<PerlinNoisMap>();
    if (perlinMap == null)
    {
        Debug.LogError("PerlinNoisMap not found!");
        return;
    }

    var tilemap = perlinMap.lightStoneTilemap;
    int spawned = 0;

    for (int x = -perlinMap.mapWidth / 2; x < perlinMap.mapWidth / 2; x++)
    {
        for (int y = -perlinMap.mapHeight / 2; y < perlinMap.mapHeight / 2; y++)
        {
            Vector3Int tilePos = new Vector3Int(x, y, 0);
            Vector3Int belowPos = new Vector3Int(x, y - 1, 0);

            if (!tilemap.HasTile(tilePos) && tilemap.HasTile(belowPos))
            {
                Vector3 spawnPosition = new Vector3(x, y + nodeYOffset, -1f); // <- Z offset to avoid collision
                GameObject node = Instantiate(walkablePrefab, spawnPosition, Quaternion.identity, walkableParent);
                node.name = $"WalkableNode_{x}_{y}";
                spawned++;
            }
        }
    }

    Debug.Log($"Spawned {spawned} refined walkable nodes.");
}



}
