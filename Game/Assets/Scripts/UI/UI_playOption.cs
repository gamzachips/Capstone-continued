using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_playOption : MonoBehaviour
{
    public void Option()
    {
        CanvasGroupOn(optiopGroup);
    }
    public CanvasGroup optiopGroup;

    public void QuitGame()
    {
        Debug.Log("QuitGame");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void QuitOption()
    {

    }

    public void CanvasGroupOn( CanvasGroup cg )
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;

    }
    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    }
