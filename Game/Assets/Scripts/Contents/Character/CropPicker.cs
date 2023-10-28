using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CropPicker : MonoBehaviour
{
    private GameObject currentCrop = null; //현재 트리거에 잡힌 작물

    void Update() //매 프레임마다
    {
        //현재 트리거에 잡힌 작물 있는 상태에서 E키 누를 때
        if (currentCrop != null && Input.GetKeyDown(KeyCode.E))
        {
            //텍스트 비활성화
            Managers.UI.DisableInteractText();

            //작물 수확
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("pick_fruit");

            StartCoroutine(HarvestCrop());

        }
    }

    private void OnTriggerEnter(Collider other) //트리거 잡힐 때
    {

        if (other.CompareTag("PickableCropTrigger") && currentCrop == null)
        {
            //현재 트리거에 잡힌 작물이 없으면 지금 잡힌 작물을 저장
            currentCrop = other.gameObject.transform.parent.gameObject;

            //텍스트 활성화
            Managers.UI.SetInteractText("줍기[E]");
            Managers.UI.EnableInteractText();
        }
    }

    private void OnTriggerExit(Collider other) //트리거에서 빠져나올 때
    {
        if (other.CompareTag("PickableCropTrigger"))
        {
            //텍스트 비활성화
            Managers.UI.DisableInteractText();

            //현재 잡힌 작물 null로 초기화
            currentCrop = null;
        }
    }

    private IEnumerator HarvestCrop() //작물 수확
    {
        if (currentCrop != null)
        {
            GameObject crop = currentCrop;
            GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;

            yield return new WaitForSeconds(1f);

            GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;

            //작물 비활성화
            crop.GetComponent<PickableCrop>().ClearCrop();

            Inventory.instance.AddItem(crop.GetComponent<PickableCrop>().Id,1);

            Managers.Energy.DecreaseEnergy(0.2f);
            //현재 작물 null 초기화
            currentCrop = null;
        }
    }
}
