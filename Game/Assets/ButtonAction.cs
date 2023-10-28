using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour
{
    public void DeactiveButton()
    {
        Button button = gameObject.GetComponent<Button>();
        button.interactable = false;
        
        Image image = gameObject.GetComponent<Image>();
        Color newColor = new Color(125f, 125f, 125f);
        image.color = newColor;
    }

    public void ActiveButton()
    {
        Button button = gameObject.GetComponent <Button>();
        button.interactable = true;

        Image image = gameObject.GetComponent<Image>();
        Color newColor = Color.white;
        image.color = newColor;
    }
}
