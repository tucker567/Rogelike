using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text countText;

    [Header("Tooltip")]
    public GameObject tooltipTextObject;  // Assign in Inspector
    private TMP_Text tooltipText;
    private Item currentItem;

    private bool allowTooltip = false; // prevents tooltip on spawn

    public void SetItem(Item item, int count)
    {
        icon.sprite = item.icon;
        countText.text = count.ToString();
        currentItem = item;

        if (tooltipText == null && tooltipTextObject != null)
        {
            tooltipText = tooltipTextObject.GetComponent<TMP_Text>();
        }

        if (tooltipText != null)
        {
            tooltipText.text = item.description;
        }

        tooltipTextObject.SetActive(false); // Make sure it's hidden initially
        allowTooltip = false; // Prevent showing immediately

        // Wait one frame before allowing tooltips
        Invoke(nameof(EnableTooltip), 0.05f);
    }

    private void EnableTooltip()
    {
        allowTooltip = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (allowTooltip && tooltipTextObject != null)
        {
            tooltipTextObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipTextObject != null)
        {
            tooltipTextObject.SetActive(false);
        }
    }
}
