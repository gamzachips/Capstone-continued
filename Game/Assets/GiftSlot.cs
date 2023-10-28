using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GiftSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    GiftInvenUI giftInvenUI;

    string itemId;

    ItemBase item;
    public ItemBase Item { get { return item; } }
    int itemCount = 0;
    public int ItemCount { get { return itemCount; } }

    public TextMeshProUGUI item_count;
    public Image item_image;

    GameObject item_slotInfo;

    [SerializeField]
    Transform infoPos;

    private void Start()
    {
        giftInvenUI = GameObject.Find("GiftInventoryUI").GetComponent<GiftInvenUI>();
    }

    public void OnClicked()
    {
        giftInvenUI.SelectedGift = itemId;
    }

    public void Set(string id, int count)
    {
        itemId = id;
        //이미지

        item = Managers.Data.GetItemData(id);

        item_image.sprite = Managers.Resource.GetSprite(id);
        Color color = item_image.color;
        color.a = 1f;
        item_image.color = color;

        //개수
        itemCount = count;
        item_count.SetText(itemCount.ToString());
    }

    public void SetNull()
    {
        item = null;
        Color color = item_image.color;
        color.a = 0f;
        item_image.color = color;

        item_count.SetText("");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        // info 위치 조정
        RectTransform infoUIPos = item_slotInfo.GetComponent<RectTransform>();
        Vector3 localPosition = giftInvenUI.transform.InverseTransformPoint(infoPos.position);
        infoUIPos.localPosition = localPosition;
        item_slotInfo.SetActive(true);

        // info 텍스트 변경
        giftInvenUI.itemInfo_name.text = item.name;
        giftInvenUI.itemInfo_price.text = item.sell_price + "GOLD";

        giftInvenUI.itemInfo_energy.text = "에너지 +" + item.energy;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        item_slotInfo.SetActive(false);

    }
}
