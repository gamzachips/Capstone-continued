using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InvenSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    UI_Inventory inventoryUI;

    ItemBase item;
    int itemCount = 0;
    public TextMeshProUGUI item_count;
    public Image item_image;

    bool mouseOn = false;
    public bool MouseOn { get { return mouseOn; } }
    
    GameObject item_slotInfo;

    [SerializeField]
    Transform infoPos;

    [SerializeField]
    Transform interactBtnPos;

    private void Start()
    {
        inventoryUI = GameObject.Find("Inventory").GetComponent<UI_Inventory>();
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
        if(item == null)
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
        inventoryUI.itemInfo_price.text = item.sell_price + "GOLD";
        inventoryUI.itemInfo_energy.text = "에너지 +" + item.energy;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;

        if(Input.GetMouseButtonDown(1))
        {
            if(true)
            {
                // 버튼 위치 조정
                RectTransform uiPos = inventoryUI.InteractBtn.GetComponent<RectTransform>();
                Vector3 localPosition = inventoryUI.transform.InverseTransformPoint(interactBtnPos.position);
                uiPos.localPosition = localPosition;
                inventoryUI.InteractBtn.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;
        mouseOn = false;

        item_slotInfo.SetActive(false);
        StartCoroutine(CloseInteractBtn());
       
    }

    private IEnumerator CloseInteractBtn()
    {
        yield return new WaitForSeconds(0.2f);

        //상호작용 버튼에서도 마우스가 떼어져있으면 상호작용 버튼을 닫음
        if (inventoryUI.InteractBtn.GetComponent<UI_ItemInteractBtn>().MouseOn == false)
        {
            inventoryUI.InteractBtn.SetActive(false);
        }
    }

    public void EatItem()
    {
        //먹기 효과 
        Managers.Sound.PlayEating();
        Managers.Energy.IncreaseEnergy(item.energy);
        RemoveItem(1);
    }

    public void RemoveItem(int count)
    {
        Inventory.instance.RemoveItem(item.id, count);
    }
    
}
