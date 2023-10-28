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
        //�̹���

        item = Managers.Data.GetItemData(itemId);

        item_image.sprite = Managers.Resource.GetSprite(itemId);
        Color color = item_image.color;
        color.a = 1f;
        item_image.color = color;

        //����
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

        // info ��ġ ����
        RectTransform infoUIPos = item_slotInfo.GetComponent<RectTransform>();
        Vector3 localPosition = inventoryUI.transform.InverseTransformPoint(infoPos.position);
        infoUIPos.localPosition = localPosition;
        item_slotInfo.SetActive(true);

        // info �ؽ�Ʈ ����
        inventoryUI.itemInfo_name.text = item.name;
        inventoryUI.itemInfo_price.text = item.sell_price + "GOLD";
        inventoryUI.itemInfo_energy.text = "������ +" + item.energy;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;

        if(Input.GetMouseButtonDown(1))
        {
            if(true)
            {
                // ��ư ��ġ ����
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

        //��ȣ�ۿ� ��ư������ ���콺�� ������������ ��ȣ�ۿ� ��ư�� ����
        if (inventoryUI.InteractBtn.GetComponent<UI_ItemInteractBtn>().MouseOn == false)
        {
            inventoryUI.InteractBtn.SetActive(false);
        }
    }

    public void EatItem()
    {
        //�Ա� ȿ�� 
        Managers.Sound.PlayEating();
        Managers.Energy.IncreaseEnergy(item.energy);
        RemoveItem(1);
    }

    public void RemoveItem(int count)
    {
        Inventory.instance.RemoveItem(item.id, count);
    }
    
}
