using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeInterior : MonoBehaviour
{
    [SerializeField]
    GameObject[] interiors;

    [SerializeField]
    GameObject[] buttons;

    int interiorLevel = 0;

    [SerializeField]
    GameObject Kitchen;

    [SerializeField]
    int[] golds;

    private void Start()
    {
        for (int i = 1; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<ButtonAction>().DeactiveButton();
        }
        Kitchen.SetActive(false);

    }
    public void UpgradeTo1()
    {
        if (Managers.Gold.SubGold(golds[0]))
        {
            UpgradeButton(0);
        }
    }
    public void UpgradeTo2()
    {
        if (Managers.Gold.SubGold(golds[1]))
        {
            UpgradeButton(1);
            Kitchen.SetActive(true);
        }     
    }
    public void UpgradeTo3()
    {
        if (Managers.Gold.SubGold(golds[2]))
        {
            UpgradeButton(2);
        } 
    }
    public void UpgradeTo4()
    {
        if (Managers.Gold.SubGold(golds[3]))
        {
            UpgradeButton(3);
        }  
    }

    private void UpgradeButton(int now)
    {
        interiors[now].SetActive(true);
        buttons[now].GetComponent<ButtonAction>().DeactiveButton();
        buttons[now].GetComponentInChildren<TextMeshProUGUI>().text = "구매완료";

        if (now < buttons.Length)
            buttons[now + 1].GetComponent<ButtonAction>().ActiveButton();
    }


}
