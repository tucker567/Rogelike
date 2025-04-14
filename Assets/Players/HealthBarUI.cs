using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;

    public void SetMax(int max)
    {
        slider.maxValue = max;
        slider.value    = max;
        healthText.text = $"{max}/{max}";
    }

    public void SetValue(int value)
    {
        slider.value = value;
        healthText.text = $"{value}/{slider.maxValue}";
    }
}
