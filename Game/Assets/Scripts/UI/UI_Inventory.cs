using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    Inventory inven;

    public GameObject inventoryPanel;
    bool activateInventory = false;
    public GameObject player;

    //slot
    UI_InvenSlot[] slots;
    public Transform slotHolder;

    UI_InvenSlot selectedSlot;
    public UI_InvenSlot SelectedSlot { get { return selectedSlot; } set { selectedSlot = value; } }

    [SerializeField]
    GameObject slotInfo;
    public GameObject SlotInfo {  get { return slotInfo; } }

    [SerializeField]
    GameObject interactBtn;
    public GameObject InteractBtn { get {  return interactBtn; } }


    public TextMeshProUGUI itemInfo_name;
    public TextMeshProUGUI itemInfo_price;
    public TextMeshProUGUI itemInfo_energy;

    private void Start()
    {
        inven = Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<UI_InvenSlot>(); 
        inven.onSlotCountChange += SlotChange;
        inventoryPanel.SetActive(activateInventory);
        inven.onChangeItem += InventoryChanged;

        slotInfo.SetActive(false);
    }

    private void InventoryChanged()
    {
        //인벤토리 정보를 가져온다
        LinkedList<string> itemList;
        Dictionary<string, int> itemCountDict;
        inven.GetInventoryItems(out itemList, out itemCountDict);

        int slotNum = 0;
        foreach(string itemId in itemList)
        {
            //각 슬롯을 인벤토리 리스트로 갱신
            slots[slotNum].Set(itemId, itemCountDict[itemId]);
            slotNum++;
        }
        for(int slot = slotNum; slot < slots.Length; slot++)
        {
            slots[slot].SetNull();
        }
    }

    private void SlotChange(int val)
    {
        for (int i = 0; i < slots.Length; i++) {
            if (i < inven.SlotCount)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activateInventory = !activateInventory;
            inventoryPanel.SetActive(activateInventory);

            if(activateInventory)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
                player.GetComponent<CharacterController>().enabled = false;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
                player.GetComponent<CharacterController>().enabled = true;

                slotInfo.SetActive(false);
            }
        }
    }

  
}
