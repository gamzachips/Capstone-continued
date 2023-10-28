using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant_Order : MonoBehaviour
{
    [SerializeField]
    GameObject[] canvases;

    [SerializeField]
    List<GameObject> dishButtons;

    GameObject player;

    bool playerIn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadMenu();

            playerIn = true;
            player = other.gameObject;
            Managers.UI.EnableInteractText();
            Managers.UI.SetInteractText("[E] 주문하기");
        }
    }

    private void Update()
    {
        if (playerIn && Input.GetKey(KeyCode.E))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            canvases[0].SetActive(true);
            Managers.Time.StopTime();
            player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
            player.GetComponent<CharacterController>().enabled = false;
            Managers.UI.SetInteractText("[ESC] 창 닫기");
        }

        if (playerIn && Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            foreach (GameObject go in canvases)
            {
                go.SetActive(false);
            }
            Managers.Time.RunTime();
            player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
            player.GetComponent<CharacterController>().enabled = true;
            Managers.UI.SetInteractText("[E] 주문하기");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (playerIn && other.CompareTag("Player"))
        {
            playerIn = false;
            Managers.UI.SetInteractText("");
            Managers.UI.DisableInteractText();
        }
    }

    private void LoadMenu()
    {
        var foodData = Managers.Data.FoodList;

        int i = 0;
        foreach (var food in foodData)
        {
            if (dishButtons.Count <= i) continue;
            dishButtons[i].GetComponent<Restaurant_Item>().Init(food);
            i++;
        }
    }
}
