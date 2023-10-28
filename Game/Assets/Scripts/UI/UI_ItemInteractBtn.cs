using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemInteractBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UI_Inventory inventoryUI;

    bool mouseOn = false;
    public bool MouseOn { get { return mouseOn; } }

    private void Start()
    {
        inventoryUI = GameObject.Find("Inventory").GetComponent<UI_Inventory>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOn = false;

        if(inventoryUI.SelectedSlot.MouseOn == false)
        {
            inventoryUI.InteractBtn.SetActive(false);
        }
    }

    public void OnClickedEatBtn()
    {
        //먹기
        inventoryUI.SelectedSlot.EatItem();
        inventoryUI.InteractBtn.SetActive(false);
    }

    public void OnClickedRemoveBtn()
    {
        int count = 1; // 개수 TODO 
        inventoryUI.SelectedSlot.RemoveItem(count);
        inventoryUI.InteractBtn.SetActive(false);
    }

}
