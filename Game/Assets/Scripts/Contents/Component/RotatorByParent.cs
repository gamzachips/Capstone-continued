using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorByParent : MonoBehaviour
{
    float speed = 30f;
    
    
    void Update ()
    {
        Transform parent = transform.parent;
        transform.Rotate(transform.InverseTransformDirection(parent.up), speed * Time.deltaTime);
        
    }
}
