using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void Save(SaveData data)
    {
        Debug.Log("저장완료");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.dataPath,"SaveG.bin");
        FileStream stream = File.Create(path);

        formatter.Serialize(stream,data);
        stream.Close();
    }

    public static SaveData Load()
    {

        try
        {
        Debug.Log("불러오기성공");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.dataPath, "SaveG.bin");
        FileStream stream = File.OpenRead(path);
        SaveData data = (SaveData)formatter.Deserialize(stream);
        stream.Close();
        return data;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return default;
        }



    }
}
