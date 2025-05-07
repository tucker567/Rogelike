using UnityEngine;

public class PerlinSettings : MonoBehaviour
{
    public static PerlinSettings Instance { get; private set; }

    public float magnitude = 10.0f;
    public float frequency = 1.0f;
    public float noiseThreshold = 0.5f;
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
