using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance; // Singleton instance

    [System.Serializable]
    public class PrefabEntry
    {
        public string name;
        public GameObject prefab;
    }

    public List<PrefabEntry> prefabList = new List<PrefabEntry>();
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Populate the dictionary for quick lookup
        foreach (var entry in prefabList)
        {
            if (entry.prefab != null && !prefabDictionary.ContainsKey(entry.name))
            {
                prefabDictionary.Add(entry.name, entry.prefab);
            }
        }
    }

    /// <summary>
    /// Instantiates a prefab by name at a given position and rotation.
    /// </summary>
    public GameObject SpawnPrefab(string prefabName, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.TryGetValue(prefabName, out GameObject prefab))
        {
            return Instantiate(prefab, position, rotation);
        }
        else
        {
            Debug.LogError($"Prefab '{prefabName}' not found in PrefabManager.");
            return null;
        }
    }
}
