using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MarketTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject canvas;
    bool playerIn = false;

    GameObject player;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerIn = true;
            player = collider.gameObject;
            Managers.UI.EnableInteractText();
            Managers.UI.SetInteractText("[E] 상점 이용하기");
        }
    }

    private void Update()
    {
        if (playerIn && Input.GetKey(KeyCode.E))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            canvas.SetActive(true);
            Managers.Time.StopTime();
            player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
            player.GetComponent<CharacterController>().enabled = false;
            Managers.UI.SetInteractText("[ESC] 창 닫기");
        }

        if (playerIn && Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            canvas.SetActive(false);
            Managers.Time.RunTime();
            player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
            player.GetComponent<CharacterController>().enabled = true;
            Managers.UI.SetInteractText("[E] 상점 이용하기");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (playerIn && collider.CompareTag("Player"))
        {
            playerIn = false;
            Managers.UI.SetInteractText("");
            Managers.UI.DisableInteractText();
        }
    }
}
