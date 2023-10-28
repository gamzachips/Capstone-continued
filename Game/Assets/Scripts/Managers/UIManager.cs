using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class UIManager
{
    GameObject canvas;
    GameObject interactText;
    GameObject famringUI;
    GameObject dayText;
    GameObject timeText;
    GameObject goldUI;
    GameObject player;
    TextMeshProUGUI harvestText;

    public void Start()
    {
        {
            canvas = GameObject.Find("MainCanvas");
            player = GameObject.Find("Player");
        }
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UI_InteractionText");
            interactText = GameObject.Instantiate(prefab, canvas.transform);
            interactText.SetActive(false);
        }
        {
            famringUI = GameObject.Find("UI_Farming");
            famringUI.SetActive(false);
        }
        {
            dayText = GameObject.Find("DayText");
            dayText.SetActive(true);
        }

        {
            timeText = GameObject.Find("TimeText");
            timeText.SetActive(true);
        }
        {
            goldUI = GameObject.Find("GoldText");
            goldUI.SetActive(true);
        }

        {
            harvestText = GameObject.Find("UI_HarvestText").GetComponent<TextMeshProUGUI>();
        }
    }

    public void Update()
    {
        timeText.GetComponent<TextMeshProUGUI>().SetText(Managers.Time.GetHour().ToString() + "½Ã");
        goldUI.GetComponent<TextMeshProUGUI>().SetText("{0}G", Managers.Gold.GetGold());
        dayText.GetComponent<TextMeshProUGUI>().SetText(Managers.Time.GetDay());
    }

    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }
    public void DisableCanvas()
    {
        canvas.SetActive(false);

    }
    public void EnableInteractText()
    {
        interactText.SetActive(true);
    }
    public void DisableInteractText()
    {
        interactText.SetActive(false);
    }

    public void SetInteractText(string text)
    {
        interactText.GetComponent<TextMeshProUGUI>().text = text;
    }
    public void EnableFarmingUI()
    {
        famringUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Managers.Time.StopTime();
        player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
        player.GetComponent<CharacterController>().enabled = false;
    }
    public void DisableFarmingUI()
    {
        famringUI.SetActive(false); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Managers.Time.RunTime();
        player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
        player.GetComponent<CharacterController>().enabled = true;
    }

    public void HarvestText(string name, int count)
    {
        harvestText.SetText(name + " +{0}", count);
    }

    public void ClearHarvestText()
    {

        harvestText.SetText("");
    }

    public void ReleaseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
        player.GetComponent<CharacterController>().enabled = false;
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
