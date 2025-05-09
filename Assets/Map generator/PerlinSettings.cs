using UnityEngine;

public class PerlinSettings : MonoBehaviour
{
    public static PerlinSettings Instance { get; private set; }
    public int seed = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep settings across scenes.
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists.
        }
    }
}
