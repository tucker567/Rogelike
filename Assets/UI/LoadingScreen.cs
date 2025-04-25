using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI tipText;       // Drag the TipText object here
    public float tipChangeInterval = 4f;  // Seconds between tips
    public float minLoadTime = 2f;       // Minimum time to show loading screen
    public TextMeshProUGUI percentText; // Drag the TextMeshPro object here


    [TextArea(2, 5)]
    public List<string> tips = new List<string>
    {

    };

    private void Start()
    {
        StartCoroutine(LoadAsync());
        StartCoroutine(CycleTips());
    }

IEnumerator LoadAsync()
{
    yield return null;

    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LevelLoader.TargetScene);
    asyncLoad.allowSceneActivation = false;

    float elapsedTime = 0f;

    while (elapsedTime < minLoadTime)
    {
        elapsedTime += Time.unscaledDeltaTime;

        // ✅ Calculate percent based on elapsed min loading time
        float progress = Mathf.Clamp01(elapsedTime / minLoadTime);
        percentText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";

        yield return null;
    }

    // ✅ After min load time is reached, allow activation
    percentText.text = "Loading... 100%";

    // optional extra polish wait
    yield return new WaitForSecondsRealtime(0.2f);

    asyncLoad.allowSceneActivation = true;
}



IEnumerator CycleTips()
{
    List<string> remainingTips = new List<string>(tips); // Make a working copy

    while (true)
    {
        if (remainingTips.Count == 0)
        {
            // Refill once all have been shown
            remainingTips = new List<string>(tips);
        }

        int randomIndex = Random.Range(0, remainingTips.Count);
        tipText.text = remainingTips[randomIndex];
        remainingTips.RemoveAt(randomIndex); // Remove shown tip

        yield return new WaitForSecondsRealtime(tipChangeInterval);
    }
}


}
