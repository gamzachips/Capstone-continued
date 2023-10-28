using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        Debug.Log("NewGame");
        SceneLoad.LoadSceneHandle("Main",0);
     //   SceneManager.LoadScene("Main");
    }

    public void ContinueGame()
    {
      Debug.Log("ContinueGame");
      SceneLoad.LoadSceneHandle("Main", 1);

    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
