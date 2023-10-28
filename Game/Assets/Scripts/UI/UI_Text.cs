using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Text : UI_Base
{
    public void SetObjectName(string name)
    {
        gameObject.name = name;
    }
    public void SetText(string text)
    {
        gameObject.GetComponent<TextMeshPro>().text = text;
    }

}
