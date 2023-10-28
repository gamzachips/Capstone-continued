using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{

    public Sprite GetSprite(string id)
    {
        return Resources.Load<Sprite>("JsonData/Sprites/" + id);
    }

}

