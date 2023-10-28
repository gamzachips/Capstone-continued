using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CropSelectBtn : MonoBehaviour
{
    [SerializeField]
    string cropId;
    public string CropId { get { return cropId; } }

    [SerializeField]
    GameObject player;

    public void SelectCrop()
    {
        player.GetComponent<Farmer>().SelectCrop(cropId);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
