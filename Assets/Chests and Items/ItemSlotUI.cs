using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    public void SetItem(Item item, int count)
    {
        icon.sprite = item.icon;
        countText.text = count.ToString();
    }
}
