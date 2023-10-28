using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //저장및 불러오기 관련
    public void Saveplayer()
    {
        SaveData save = new SaveData();
        save.x = transform.position.x;
        save.y = transform.position.y;
        save.z = transform.position.z;

        SaveManager.Save(save);

    }

    public void LoadPlayer()
    {
        SaveData save = SaveManager.Load();
        transform.position = new Vector3(save.x, save.y, save.z);
    }


//피로도 관련







}
