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
    public Image leftWeaponImage;                  // Image for left weapon
    public TMP_Text leftWeaponDescription;           // TextMeshPro for left weapon description
    public Image rightWeaponImage;                 // Image for right weapon
    public TMP_Text rightWeaponDescription;          // TextMeshPro for right weapon description

    [Header("MapSettingsInputs")]
    public TMP_InputField magnitudeInput;
    public TMP_InputField frequencyInput;
    public TMP_InputField noiseThresholdInput;
    public TMP_InputField seedInput;

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

    public void OnPlayButton()
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);

        if (MapSettings.Instance != null)
        {
            if (!string.IsNullOrWhiteSpace(magnitudeInput.text))
                float.TryParse(magnitudeInput.text, out MapSettings.Instance.magnitude);

            if (!string.IsNullOrWhiteSpace(frequencyInput.text))
                float.TryParse(frequencyInput.text, out MapSettings.Instance.frequency);

            if (!string.IsNullOrWhiteSpace(noiseThresholdInput.text))
                float.TryParse(noiseThresholdInput.text, out MapSettings.Instance.noiseThreshold);

            if (!string.IsNullOrWhiteSpace(seedInput.text))
                int.TryParse(seedInput.text, out MapSettings.Instance.seed);

            Debug.Log($"Updated MapSettings: mag={MapSettings.Instance.magnitude}, freq={MapSettings.Instance.frequency}, threshold={MapSettings.Instance.noiseThreshold}, seed={MapSettings.Instance.seed}");
        }

        LevelLoader.Load("Map 1");
    }




    /* ---------- Helpers ---------- */

    void ShowCharacterInfo(int index)
    {
        CharacterDefinition d = characters[index];
        portraitDisplay.sprite   = d.portrait;
        nameText.text            = d.characterName;
        descriptionText.text     = d.description;
        leftWeaponImage.sprite   = d.leftWeaponSprite;
        leftWeaponDescription.text = d.leftWeaponDescription;
        rightWeaponImage.sprite  = d.rightWeaponSprite;
        rightWeaponDescription.text = d.rightWeaponDescription;
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
