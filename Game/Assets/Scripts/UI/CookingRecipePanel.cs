using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CookingRecipePanel : MonoBehaviour
{
    [SerializeField]
    UI_CookingSlot food;

    [SerializeField]
    UI_CookingSlot[] ingredients;

    [SerializeField]
    string foodId;
    [SerializeField]
    string[] ingredientIds;

    [SerializeField]
    Image checkImage;

    private void Start()
    {
        food.Set(foodId);

        int i = 0;
        for(;i<ingredientIds.Length;i++)
        {
            ingredients[i].Set(ingredientIds[i]);
        }
        for(; i <3;i++)
        {
            ingredients[i].SetNull();
        }

        UpdatePanel();
        Inventory.instance.onChangeItem += UpdatePanel;
    }

    public void UpdatePanel()
    {
        LinkedList<string> itemList;
        Dictionary<string, int> itemCountDict;
        Inventory.instance.GetInventoryItems(out itemList, out itemCountDict);

        checkImage.gameObject.SetActive(true);
        food.ClearDark();
        int i = 0;
        for (; i < ingredientIds.Length; i++)
        {
            //아이템이 없으면
            if(itemList.Find(ingredientIds[i]) == null)
            {
                ingredients[i].AddDark();
                checkImage.gameObject.SetActive(false);
                food.AddDark();
            }
            else //있으면
            {
                ingredients[i].ClearDark();
            }
        }
    }

    public void Cook()
    {
        LinkedList<string> itemList;
        Dictionary<string, int> itemCountDict;
        Inventory.instance.GetInventoryItems(out itemList, out itemCountDict);

        for (int i = 0; i < ingredientIds.Length; i++)
        {
            //아이템이 없으면 요리 불가
            if (itemList.Find(ingredientIds[i]) == null)
                return;
        }

        for(int i = 0; i < ingredientIds.Length; i++)
        {
            Inventory.instance.RemoveItem(ingredientIds[i],1);
        }

        Inventory.instance.AddItem(foodId, 1);
        
    }
}
