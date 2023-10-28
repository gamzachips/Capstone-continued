using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Likeability : MonoBehaviour
{
    [SerializeField]
    string npcName;

    [SerializeField]
    float like = 0;

    [SerializeField]
    TextMeshProUGUI npcNameText;

    [SerializeField]
    TextMeshProUGUI likeText;

    public float Like {  get { return like; } 
        set 
        { 
            like = value;
            SetGrade();
        } 
    }

    [SerializeField]
    int grade = 0;
    public int Grade { get {  return grade; }}

    private void Start()
    {
        npcNameText.SetText(npcName);
    }

    private void Update()
    {
        likeText.SetText(((int)like).ToString());
    }

    public void Increase(float amount)
    {
        like += amount;
        SetGrade();
    }

    public void ChangeWithItem(string itemId, int giftGrade)
    {
        float price = Managers.Data.GetItemData(itemId).sell_price;
        float likeAmount = Mathf.Sqrt(price/10);
        if (giftGrade == 0) likeAmount -= 5;
        else if (giftGrade == 2) likeAmount *= 3;
        like += likeAmount;
    }

    private void SetGrade()
    {
        if (like >= 2)
        {
            grade = 1;
        }
        if (like >= 10)
        {
            grade = 2;
        }
        if (like >= 30)
        {
            grade = 3;
        }
        if (like >= 50)
        {
            grade = 4;
        }
        if(like < -10)
        {
            grade = 5;
        }
    }
}
