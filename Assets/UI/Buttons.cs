using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void gotoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    void packatles()
    {
        Debug.Log("Packatles");
    }

    public void LoadGameScene()
    {
        LevelLoader.Load("Map 1"); // or whatever your actual scene name is
    }
}
