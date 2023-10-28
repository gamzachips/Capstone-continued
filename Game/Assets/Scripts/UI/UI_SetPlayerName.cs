using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SetPlayerName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    public void OnButtonClicked()
    {
        if(inputField != null)
        {
            Managers.Data.PlayerName = inputField.text;
            Managers.UI.LockCursor();
            gameObject.SetActive(false);
        }
    }
}
