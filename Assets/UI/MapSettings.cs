using UnityEngine;

public class MapSettings : MonoBehaviour
{
    public static MapSettings Instance;

    public float magnitude = 10f;
    public float frequency = 1f;
    public float noiseThreshold = 0.5f;
    public int seed = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <- this is critical
        }
        else
        {
            Destroy(gameObject); // prevent duplicate singletons
        }
    }

    void Start()
    {
        Debug.Log("MapSettings is alive and persisting to next scene!");
    }

}
