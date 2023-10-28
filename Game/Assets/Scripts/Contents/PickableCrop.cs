using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableCrop : MonoBehaviour
{
    [SerializeField]
    string id;
    public string Id {  get { return id; } }

    public void ClearCrop()
    {
        Destroy(gameObject);
    }
}
