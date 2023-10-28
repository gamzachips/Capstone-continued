using OpenAI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Gift;

public class GiftInvenUI : MonoBehaviour
{
    Gift npcGift;
    public Gift NpcGift { get {  return npcGift; } set { npcGift = value; } }

    [SerializeField] GameObject withGift;
    [SerializeField] TextMeshProUGUI giftNameText;

    //slot
    GiftSlot[] slots;
    public Transform slotHolder;

    string selectedGift;
    public string SelectedGift {  get { return selectedGift; } set {  selectedGift = value; } }

    [SerializeField]
    GameObject slotInfo;
    public GameObject SlotInfo { get { return slotInfo; } }

    public TextMeshProUGUI itemInfo_name;
    public TextMeshProUGUI itemInfo_price;
    public TextMeshProUGUI itemInfo_energy;

    public void Start()
    {
        slots = slotHolder.GetComponentsInChildren<GiftSlot>();
        Inventory.instance.onChangeItem += InventoryChanged;
        Inventory.instance.onChangeItem.Invoke();
    }
    private void InventoryChanged()
    {
        //인벤토리 정보를 가져온다
        LinkedList<string> itemList;
        Dictionary<string, int> itemCountDict;
        Inventory.instance.GetInventoryItems(out itemList, out itemCountDict);

        int slotNum = 0;
        foreach (string itemId in itemList)
        {
            //각 슬롯을 인벤토리 리스트로 갱신
            slots[slotNum].Set(itemId, itemCountDict[itemId]);
            slotNum++;
        }
        for (int slot = slotNum; slot < slots.Length; slot++)
        {
            slots[slot].SetNull();
        }
    }

    public void OnClikedGiftBtn()
    {
        if (npcGift == null) return;

        Inventory.instance.RemoveItem(selectedGift, 1);

        int giftGrade = (int)NpcGift.GetGiftGrade(selectedGift);
        //선물 선호도 전달
        npcGift.gameObject.GetComponent<ChatGPT>().UpdateGiftPrompt(selectedGift, giftGrade);

        //선물 텍스트 
        giftNameText.SetText(Managers.Data.GetItemData(SelectedGift).name);
        withGift.SetActive(true);

        //창 닫기
        gameObject.SetActive(false);
    }
}
