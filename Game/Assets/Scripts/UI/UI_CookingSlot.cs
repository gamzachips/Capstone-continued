using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CookingSlot : MonoBehaviour
{
    [SerializeField]
    string itemId;

    [SerializeField]
    Image image;

    [SerializeField]
    TextMeshProUGUI itemName;

    [SerializeField]
    Image cover;

    public void Set(string id)
    {
        itemId = id;
        image.sprite = Managers.Resource.GetSprite(itemId);
        itemName.SetText(Managers.Data.GetItemData(id).name);
        Color color = image.color;
        color.a = 1f;
        image.color = color;
    }

    public void SetNull()
    {
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        itemName.SetText("");
        AddDark();
    }

    public void AddDark()
    {
        cover.gameObject.SetActive(true);
    }

    public void ClearDark()
    {
        cover.gameObject.SetActive(false);
    }
}
