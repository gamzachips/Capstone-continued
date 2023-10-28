using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory: MonoBehaviour
{


    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public delegate void OnSlotCountChange(int val); 
    public OnSlotCountChange onSlotCountChange;


    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;

    LinkedList<string> items = new LinkedList<string>();
    Dictionary</*id*/ string,/*count*/ int> itemCounts = new Dictionary<string, int>();

    
    private int slotCount;
    public int SlotCount
    {
        get => slotCount;
        set
        {
            slotCount = value;
            onSlotCountChange.Invoke(slotCount);
        }
    }

    void Start()
    {
        slotCount = 20;   

    }

    public void GetInventoryItems(out LinkedList<string> itemList, out Dictionary<string, int> itemCountDict)
    {
        itemList = items;
        itemCountDict = itemCounts;
    }

    public bool AddItem(string itemId, int itemCount)
    {
        if (items.Count < slotCount || itemCounts.ContainsKey(itemId))
        {
            
            if(itemCounts.ContainsKey(itemId))
            {
                itemCounts[itemId] += itemCount;
            }
            else
            {
                itemCounts.Add(itemId, itemCount);
                items.AddLast(itemId);
            }

            onChangeItem.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(string itemId, int count)
    {
        itemCounts[itemId] -= count;
        if (itemCounts[itemId] <= 0)
        {
            items.Remove(itemId);
        }

        onChangeItem.Invoke();
    }
}
