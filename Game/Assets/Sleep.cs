using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    bool playerInTrigger = false;
    GameObject player;
    [SerializeField] GameObject beforeSleepUI;
    [SerializeField] GameObject sleepUI;
    [SerializeField] TextMeshProUGUI sleepTimeText;
    [SerializeField] TextMeshProUGUI todaySleepTimeText;

    int timeForSleep = 0;
    int todayTimeSleep = 0;
    int sleepTimeBefore = 0;
    int wakeupTime = 0;
    bool isSleeping = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player= other.gameObject;
            Managers.UI.SetInteractText("[E] 잠자기");
            Managers.UI.EnableInteractText();
            playerInTrigger = true;
        }
    }

    private void Update()
    {
        if(playerInTrigger && Input.GetKey(KeyCode.E))
        {

            if(todayTimeSleep < 10)
            {
                beforeSleepUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Managers.Time.StopTime();
                player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
                player.GetComponent<CharacterController>().enabled = false;
                sleepTimeBefore = Managers.Time.GetHour();
                todaySleepTimeText.SetText(todayTimeSleep + "시간)");
                sleepTimeText.SetText("1");
            }
            else
            {
                //더 잘 수 없음. 
            }
        }
        if(Managers.Time.GetHour() == 0)
        {
            if (isSleeping)
                todayTimeSleep = wakeupTime;
            else
                todayTimeSleep = 0;
        }

        if(isSleeping)
        {
            if(Managers.Time.GetHour() == wakeupTime)
            {
                Time.timeScale = 1f;
                sleepUI.SetActive(false);
                Managers.Energy.IncreaseEnergy(timeForSleep * 10);
                isSleeping = false;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Managers.Time.RunTime();
                player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
                player.GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    public void IncreaseSleepTime()
    {
        timeForSleep++;
        if (todayTimeSleep + timeForSleep > 10)
            timeForSleep = 10 - todayTimeSleep;

        wakeupTime = sleepTimeBefore + timeForSleep;
        if (wakeupTime >= 24)
            wakeupTime -= 24;
        sleepTimeText.SetText(timeForSleep.ToString());
    }

    public void DecreaseSleepTime()
    {
        timeForSleep--;
        if (timeForSleep <= 1)
            timeForSleep = 1;

        wakeupTime = sleepTimeBefore + timeForSleep;
            if(wakeupTime >= 24)
                wakeupTime -= 24;
        sleepTimeText.SetText(timeForSleep.ToString());
    }

    public void DoSleep()
    {
        beforeSleepUI.SetActive(false);
        sleepUI.SetActive(true);
        Time.timeScale = 50f;
        isSleeping = true;
        todayTimeSleep += timeForSleep;
    }
}
