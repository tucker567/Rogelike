using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    /* ---------- Inspector‑assigned data ---------- */

    [Header("Characters")]
    public List<CharacterDefinition> characters;     // drag all CharacterDefinition assets here

    [Header("UI")]
    public Image portraitDisplay;                    // portrait Image
    public TMP_Text nameText;                        // TextMeshPro for name
    public TMP_Text descriptionText;                 // TextMeshPro for description

    /* ---------- Private state ---------- */

    int currentIndex = 0;                            // which character is selected

    /* ---------- Unity messages ---------- */

    void Start()
    {
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("CharacterSelectManager: no CharacterDefinition assets assigned!");
            return;
        }

        ShowCharacterInfo(0);                        // display first character on load
    }

    /* ---------- Public button callbacks ---------- */

    public void OnNextCharacter()                    // wired to Next button
    {
        currentIndex = (currentIndex + 1) % characters.Count;
        ShowCharacterInfo(currentIndex);
    }

    public void OnPreviousCharacter()               // wired to Previous button
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = characters.Count - 1;
        ShowCharacterInfo(currentIndex);
    }

    public void SelectCharacterByIndex(int index)   // wired to each portrait button
    {
        if (index < 0 || index >= characters.Count) return;
        currentIndex = index;
        ShowCharacterInfo(currentIndex);
    }

    public void OnPlayButton()                      // wired to Play button
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);
        SceneManager.LoadScene("Map 1");         // replace with your map scene name
    }

    /* ---------- Helpers ---------- */

    void ShowCharacterInfo(int index)
    {
        CharacterDefinition d = characters[index];
        portraitDisplay.sprite   = d.portrait;
        nameText.text            = d.characterName;
        descriptionText.text     = d.description;
    }

    /* ---------- Optional: keyboard hot‑keys ---------- */

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnPreviousCharacter();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnNextCharacter();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            OnPlayButton();
    }
}
