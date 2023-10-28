using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Market : MonoBehaviour
{
    //MarketInven
    LinkedList<string> items = new LinkedList<string>();
    Dictionary</*id*/ string,/*count*/ int> itemCounts = new Dictionary<string, int>();

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;

    [SerializeField]
    int defaultItemCount = 5;

    public GameObject marketPanel;
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
        slots = slotHolder.GetComponentsInChildren<UI_MarketInvenSlot>();
        LoadItemList();
        UpdateMarketSlots();
        onChangeItem += UpdateMarketSlots;
    }

    bool updated = false;
    private void Update()
    {
        if(Managers.Time.GetHour() == 0 && updated == false)
        {
            updated = true;
            LoadItemList();
            UpdateMarketSlots();
        }
        if (Managers.Time.GetHour() == 1)
            updated = false;
    }

    private void LoadItemList() //매일 자정 갱신
    {
        items.Clear();
        itemCounts.Clear();
        var groceryData = Managers.Data.GroceryList;
        int i = 0;
        foreach (var grocery in groceryData)
        {
            if (slots.Length <= i) break;
            i++;
            items.AddLast(grocery.id);
            itemCounts.Add(grocery.id, defaultItemCount);
        }
    }

    private void UpdateMarketSlots() //구매 때마다 갱신
    {
        int i = 0;
        foreach(var item  in items) 
        {
            if(slots.Length <= i) break;
            slots[i].Set(item, itemCounts[item]);
            i++;
        }
        for (; i < slots.Length; i++)
            slots[i].SetNull();
    }

    public void IncreaseCheckCount()
    {
        if (checkSlot.ItemCount > checkCount)
        {
            CheckCount = CheckCount + 1;
        }
    }
    public void IncreaseMaxCheckCount()
    {
        CheckCount = checkSlot.ItemCount;
    }
    public void DecreaseCheckCount()
    {
        if (checkCount > 1)
        {
            CheckCount = CheckCount - 1;
        }
    }
    public void DecreaseMinCheckCount()
    {
        CheckCount = 1;
    }

    public void PurchaseItem()
    {

        //플레이어 골드 차감
        if (Managers.Gold.SubGold(checkSlot.Item.sell_price * CheckCount))
        {

            //인벤토리에 아이템 추가
            if (Inventory.instance.AddItem(checkSlot.Item.id, checkCount))
            {
                //추가 성공

                //상점인벤에서 아이템 삭제
                RemoveItem(checkSlot.Item.id, CheckCount);

                guideTextUI.SetText("구매 완료!");
                guideTextUI.color = Color.green;
                StartCoroutine(WaitAndRemoveGuideText());
                CheckCount = 0;
                checkSlot.SetNull();
            }
            else
            {
                //아이템 슬롯 부족
                guideTextUI.SetText("인벤토리 공간이 부족합니다!");
                guideTextUI.color = Color.red;
                StartCoroutine(WaitAndRemoveGuideText());
                //차감된 골드 다시 복구
                Managers.Gold.AddGold(checkSlot.Item.sell_price * CheckCount);
            }
        }
        else //구매 실패
        {
            guideTextUI.SetText("골드가 부족합니다!");
            guideTextUI.color = Color.red;
            StartCoroutine(WaitAndRemoveGuideText());
        }

    }
    IEnumerator WaitAndRemoveGuideText()
    {
        yield return new WaitForSecondsRealtime(1f);
        guideTextUI.SetText("");
    }

    private void RemoveItem(string itemId, int count)
    {
        itemCounts[itemId] = itemCounts[itemId]- count;
        if (itemCounts[itemId] <= 0)
        {
            items.Remove(itemId);
        }

        onChangeItem.Invoke();
    }
}
