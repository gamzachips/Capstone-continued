using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MarketInvenSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    UI_MarketInventory inventoryUI;
    UI_Market marketUI;

    [SerializeField]
    bool forMarket = false;

    ItemBase item;
    public ItemBase Item { get {  return item; } }
    int itemCount = 0;
    public int ItemCount { get { return itemCount; } }

    public TextMeshProUGUI item_count;
    public Image item_image;

    bool mouseOn = false;
    public bool MouseOn { get { return mouseOn; } }

    GameObject item_slotInfo;

    [SerializeField]
    Transform infoPos;


    private void Start()
    {
        inventoryUI = GameObject.Find("Market_InventoryUI").GetComponent<UI_MarketInventory>();
        marketUI = GameObject.Find("MarketUI").GetComponent<UI_Market>();
        item_slotInfo = inventoryUI.SlotInfo;

    }

    public void Set(string itemId, int count)
    {
        //이미지

        item = Managers.Data.GetItemData(itemId);

        item_image.sprite = Managers.Resource.GetSprite(itemId);
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

        mouseOn = true;
        inventoryUI.SelectedSlot = this;

        // info 위치 조정
        RectTransform infoUIPos = item_slotInfo.GetComponent<RectTransform>();
        Vector3 localPosition = inventoryUI.transform.InverseTransformPoint(infoPos.position);
        infoUIPos.localPosition = localPosition;
        item_slotInfo.SetActive(true);

        // info 텍스트 변경
        inventoryUI.itemInfo_name.text = item.name;
        if(forMarket)
            inventoryUI.itemInfo_price.text = item.purchase_price + "GOLD";
        else
            inventoryUI.itemInfo_price.text = item.sell_price + "GOLD";

        inventoryUI.itemInfo_energy.text = "에너지 +" + item.energy;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;
        if (!forMarket)
        {
            inventoryUI.CheckSlot.Set(item.id, itemCount);
            inventoryUI.CheckCount = 1;
        }    
        else
        {
            marketUI.CheckSlot.Set(item.id, itemCount);
            marketUI.CheckCount = 1;
        }

        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;
        mouseOn = false;

        item_slotInfo.SetActive(false);

    }

    public void RemoveItem(int count)
    {
        Inventory.instance.RemoveItem(item.id, count);
    }

}
