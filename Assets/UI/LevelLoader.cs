// Assets/Scripts/LevelLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static string TargetScene;   // Static, globally remembered
    private static bool isLoading = false;

    public static void Load(string sceneName)
    {
        if (isLoading) return;  // prevent double loading
        isLoading = true;

        TargetScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); // << jump to the loading screen
    }
}
