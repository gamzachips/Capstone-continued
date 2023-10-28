using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeFields : MonoBehaviour
{
    [SerializeField]
    GameObject[] fields;
    [SerializeField]
    GameObject[] buttons;
    [SerializeField]
    int[] golds;

    int fieldLevel = 0;

    [SerializeField]
    GameObject[] cropButtons;

    private void Start()
    {
        for(int i = 1;i < buttons.Length; i++)
        {
            buttons[i].GetComponent<ButtonAction>().DeactiveButton();
        }
    }
    public void UpgradeTo1()
    {
        if(Managers.Gold.SubGold(golds[0]))
        {
            UpgradeButton(0);
            cropButtons[0].SetActive(true);
            cropButtons[1].SetActive(true);
            cropButtons[2].SetActive(true);
        }
    }
    public void UpgradeTo2()
    {
        if(Managers.Gold.SubGold(golds[1]))
        {
            UpgradeButton(1);
            cropButtons[3].SetActive(true);
            cropButtons[4].SetActive(true);
            cropButtons[5].SetActive(true);
        }
    }
    public void UpgradeTo3()
    {
        if(Managers.Gold.SubGold(golds[2]))
        {
            UpgradeButton(2);
            cropButtons[6].SetActive(true);
            cropButtons[7].SetActive(true);
            cropButtons[8].SetActive(true);
        }
    }
    public void UpgradeTo4()
    {
        if(Managers.Gold.SubGold(golds[3]))
        {
            UpgradeButton(3);
            cropButtons[9].SetActive(true);
            cropButtons[10].SetActive(true);
        }
    }

    private void UpgradeButton(int now)
    {
        fields[now].SetActive(true);
        buttons[now].GetComponent<ButtonAction>().DeactiveButton();
        buttons[now].GetComponentInChildren<TextMeshProUGUI>().text = "구매완료";

        if (now < buttons.Length - 1)
            buttons[now + 1].GetComponent<ButtonAction>().ActiveButton();
    }

}
