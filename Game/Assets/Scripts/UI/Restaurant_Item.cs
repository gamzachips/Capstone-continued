using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Restaurant_Item : MonoBehaviour
{
    Food food;
    
    public TextMeshProUGUI description_name;
    public TextMeshProUGUI description_price;
    public TextMeshProUGUI description_energy;
    public Image description_image;

    bool clicked = false;
    float clickTime = 0.0f;


    [SerializeField]
    TextMeshProUGUI guideTextUI;

    public void Init(Food item)
    {
        food = item;
        description_name.SetText(food.name);
        description_price.SetText("{0}GOLD", food.purchase_price);
        description_energy.SetText("에너지 +{0}", food.energy);
        description_image.sprite = Managers.Resource.GetSprite(item.id);

    }
    public void Update()
    {
        clickTime += Time.unscaledDeltaTime;
        Debug.Log(clickTime);
    }

    public void OnButtonClicked()
    {
        //더블클릭 판단 
        if (clicked == false)
        {
            clicked = true;
            clickTime = 0.0f;
        }
        else if (clickTime > 0.3f)
        {
            clicked = false;
            clickTime = 0.0f;
        }
        else
        {
            Purchase();
            clicked = false;
            clickTime = 0.0f;
        }
    }

    private void Purchase()
    {
        if(Managers.Gold.SubGold(food.purchase_price))
        {
            //구매 시도
            if(Inventory.instance.AddItem(food.id, 1))
            { //구매 가능

                guideTextUI.SetText("구매 완료!");
                guideTextUI.color = Color.green;
                StartCoroutine(WaitAndRemoveGuideText());
            }
            else
            {   //구매 불가능 (슬롯 부족)

                guideTextUI.SetText("인벤토리 공간이 부족합니다!");
                guideTextUI.color = Color.red;
                StartCoroutine(WaitAndRemoveGuideText());
                //차감된 골드 다시 복구
                Managers.Gold.AddGold(food.purchase_price);
            }
        }
        else
        {
            //구매 불가능 (돈 부족) 
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
}
