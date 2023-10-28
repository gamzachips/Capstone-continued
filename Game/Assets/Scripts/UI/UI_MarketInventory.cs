using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_MarketInventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject player;

    //slot
    UI_MarketInvenSlot[] slots;
    public Transform slotHolder;

    UI_MarketInvenSlot selectedSlot;
    public UI_MarketInvenSlot SelectedSlot { get { return selectedSlot; } set { selectedSlot = value; } }

    [SerializeField]
    GameObject slotInfo;
    public GameObject SlotInfo { get { return slotInfo; } }

    public TextMeshProUGUI itemInfo_name;
    public TextMeshProUGUI itemInfo_price;
    public TextMeshProUGUI itemInfo_energy;

    //For selling
    [SerializeField]
    UI_MarketInvenSlot checkSlot;
    public UI_MarketInvenSlot CheckSlot { get { return checkSlot; } }
    int checkCount = 1;
    public int CheckCount
    {
        get { return checkCount; }
        set
        {
            checkCount = value;
            checkCountUI.SetText("{0}", checkCount);
            if (checkSlot != null)
                checkGoldUI.SetText("{0}", checkCount * checkSlot.Item.sell_price);
            else
                checkGoldUI.SetText("0");
        }
    }

    [SerializeField]
    TextMeshProUGUI checkCountUI;

    [SerializeField]
    TextMeshProUGUI checkGoldUI;

    [SerializeField]
    TextMeshProUGUI guideTextUI;

    private void Start()
    {
        slots = slotHolder.GetComponentsInChildren<UI_MarketInvenSlot>(); // content���� slot ���� ����ü� �ִ� ��
        Inventory.instance.onChangeItem += InventoryChanged;
        Inventory.instance.onChangeItem.Invoke();
    }

    private void InventoryChanged()
    {
        //�κ��丮 ������ �����´�
        LinkedList<string> itemList;
        Dictionary<string, int> itemCountDict;
        Inventory.instance.GetInventoryItems(out itemList, out itemCountDict);

        int slotNum = 0;
        foreach (string itemId in itemList)
        {
            //�� ������ �κ��丮 ����Ʈ�� ����
            slots[slotNum].Set(itemId, itemCountDict[itemId]);
            slotNum++;
        }
        for (int slot = slotNum; slot < slots.Length; slot++)
        {
            slots[slot].SetNull();
        }
    }

    public void IncreaseCheckCount()
    {
        if(checkSlot.ItemCount > checkCount)
        {
            CheckCount = CheckCount +1;
        }
    }
    public void IncreaseMaxCheckCount()
    {
        CheckCount = checkSlot.ItemCount;
    }
    public void DecreaseCheckCount()
    {
        if(checkCount > 1)
        {
            CheckCount = CheckCount - 1;
        }
    }
    public void DecreaseMinCheckCount()
    {
        CheckCount = 1;
    }



    public void SellItem()
    {
        //�÷��̾� ��� �߰�
        Managers.Gold.AddGold(checkSlot.Item.sell_price * CheckCount);

        guideTextUI.SetText("�Ǹ� �Ϸ�!");
        guideTextUI.color = Color.green;
        StartCoroutine(WaitAndRemoveGuideText());

        //�κ��丮���� ������ ����
        Inventory.instance.RemoveItem(checkSlot.Item.id, checkCount);

        CheckCount = 0;
        checkSlot.SetNull();
    }

     IEnumerator WaitAndRemoveGuideText()
    {
        yield return new WaitForSecondsRealtime(1f);
        guideTextUI.SetText("");
    }
}
