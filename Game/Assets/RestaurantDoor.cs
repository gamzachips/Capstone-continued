using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RestaurantDoor : MonoBehaviour
{
    private bool isOpend = false;
    public bool Opend {  get { return isOpend; } set {  isOpend = value; } }

    private bool isOpening = true;

    public GameObject leftDoor;
    public GameObject rightDoor;

    public Vector3 leftDoorOpendPosition;
    public Vector3 rightDoorOpendPosition;
    public Vector3 leftDoorOpendRotation;
    public Vector3 rightDoorOpendRotation;

    public void OpenDoor()
    {
        if (isOpend == false)
        {
            leftDoor.transform.localPosition = leftDoorOpendPosition;
            leftDoor.transform.rotation = Quaternion.Euler(leftDoorOpendRotation);

            rightDoor.transform.localPosition = rightDoorOpendPosition;
            rightDoor.transform.rotation = Quaternion.Euler(rightDoorOpendRotation);

            isOpening = true;
            isOpend = true;
        }
        else
            isOpening = false;
    }

    public void CloseDoor()
    {
        if (isOpening == true)
            return;
        if(isOpend == true)
        {
            leftDoor.transform.localPosition = Vector3.zero;
            leftDoor.transform.rotation = Quaternion.Euler(Vector3.zero);

            rightDoor.transform.localPosition = Vector3.zero;
            rightDoor.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        isOpend = false;
    }
}
