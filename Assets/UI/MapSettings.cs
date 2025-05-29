using UnityEngine;

public class MapSettings : MonoBehaviour
{
    public static MapSettings Instance;

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
