using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager
{
    private List<Food> foodList = new List<Food>();
    public List<Food> FoodList { get { return foodList; } }

    private List<Food> cookFoodList = new List<Food>();
    public List<Food> CookFoodList { get { return cookFoodList; } }

    private List<Crop> cropList = new List<Crop>();
    public List<Crop> CropList { get { return cropList; } }

    private List<Grocery> groceryList = new List<Grocery>();
    public List<Grocery> GroceryList { get { return groceryList; } }

    string playerName;
    public string PlayerName {  get { return playerName; } set {  playerName = value; } }

    public void Start()
    {
        //FoodData Load
        {
            var json = ReadFile("JsonData/item_food").text;
            FoodData foodData = JsonUtility.FromJson<FoodData>(json);
            foreach(var food in foodData.info) { foodList.Add(food); }
        }
        //CookFoodData Load
        {
            var json = ReadFile("JsonData/item_cookFood").text;
            CookFoodData cookFoodData = JsonUtility.FromJson<CookFoodData>(json);
            foreach (var cookFood in cookFoodData.info) { cookFoodList.Add(cookFood); }
        }
        //GroceryData Load
        {
            var json = ReadFile("JsonData/item_grocery").text;
            GroceryData groceryData = JsonUtility.FromJson<GroceryData>(json);
            foreach (var grocery in groceryData.info) { groceryList.Add(grocery); }
        }


        //CropData Load
        {
            var json = ReadFile("JsonData/item_crop").text;
            CropData cropData = JsonUtility.FromJson<CropData>(json);
            foreach (var crop in cropData.info) { cropList.Add(crop); }
        }
    }

    private TextAsset ReadFile(string path)
    {
        var json = Resources.Load<TextAsset>(path);
        return json;
    }


    public Food GetFoodData(string id)
    {
        foreach(var food in foodList)
        {
            if (food.id == id)
                return food;
        }
        return null;
    }
    public Food GetCookFoodData(string id)
    {
        foreach (var food in cookFoodList)
        {
            if (food.id == id)
                return food;
        }
        return null;
    }

    public Crop GetCropData(string id)
    {
        foreach (var crop in cropList)
        {
            if (crop.id == id)
                return crop;
        }
        return null;
    }

    public Grocery GetGroceryData(string id)
    {
        foreach (var grocery in groceryList)
        {
            if (grocery.id == id)
                return grocery;
        }
        return null;
    }


    public ItemBase GetItemData(string id)
    {
        {
            ItemBase item = GetFoodData(id);
            if (item != null) return item;
        }

        {
            ItemBase item = GetCookFoodData(id);
            if (item != null) return item;
        }

        {
            ItemBase item = GetCropData(id);
            if (item != null) return item;
        }

        {
            ItemBase item = GetGroceryData(id);
            if (item != null) return item;
        }
        return null;
    }

}
