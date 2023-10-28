using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmCrop : MonoBehaviour
{
    string cropId;
    public string CropId { get { return cropId; } set { cropId = value; } }

    [SerializeField]
    int cropNum_min = 1;
    [SerializeField]
    int cropNum_max = 1;
    public int CropNum { get { return Random.Range(cropNum_min, cropNum_max + 1); }  }
}
