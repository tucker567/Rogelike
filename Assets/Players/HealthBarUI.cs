using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Very light wrapper around a Slider + label.
/// Attach it to the HealthBar game-object in the Canvas.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider  slider;     // drag the Slider here
    [SerializeField] private TMP_Text healthText; // drag the TextMeshProUGUI here

    /// <summary>Call once at start or whenever max HP changes.</summary>
    public void SetMax(int max)
    {
        slider.maxValue = max;
        slider.value    = max;
        healthText.text = $"{max}/{max}";
    }

    /// <summary>Call every time current HP changes.</summary>
    public void SetValue(int value)
    {
        slider.value    = value;
        healthText.text = $"{value}/{slider.maxValue}";
    }
}

