using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CropPicker : MonoBehaviour
{
    private GameObject currentCrop = null; //���� Ʈ���ſ� ���� �۹�

    void Update() //�� �����Ӹ���
    {
        //���� Ʈ���ſ� ���� �۹� �ִ� ���¿��� EŰ ���� ��
        if (currentCrop != null && Input.GetKeyDown(KeyCode.E))
        {
            //�ؽ�Ʈ ��Ȱ��ȭ
            Managers.UI.DisableInteractText();

            //�۹� ��Ȯ
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("pick_fruit");

            StartCoroutine(HarvestCrop());

        }
    }

    private void OnTriggerEnter(Collider other) //Ʈ���� ���� ��
    {

        if (other.CompareTag("PickableCropTrigger") && currentCrop == null)
        {
            //���� Ʈ���ſ� ���� �۹��� ������ ���� ���� �۹��� ����
            currentCrop = other.gameObject.transform.parent.gameObject;

            //�ؽ�Ʈ Ȱ��ȭ
            Managers.UI.SetInteractText("�ݱ�[E]");
            Managers.UI.EnableInteractText();
        }
    }

    private void OnTriggerExit(Collider other) //Ʈ���ſ��� �������� ��
    {
        if (other.CompareTag("PickableCropTrigger"))
        {
            //�ؽ�Ʈ ��Ȱ��ȭ
            Managers.UI.DisableInteractText();

            //���� ���� �۹� null�� �ʱ�ȭ
            currentCrop = null;
        }
    }

    private IEnumerator HarvestCrop() //�۹� ��Ȯ
    {
        if (currentCrop != null)
        {
            GameObject crop = currentCrop;
            GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;

            yield return new WaitForSeconds(1f);

            GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;

            //�۹� ��Ȱ��ȭ
            crop.GetComponent<PickableCrop>().ClearCrop();

            Inventory.instance.AddItem(crop.GetComponent<PickableCrop>().Id,1);

            Managers.Energy.DecreaseEnergy(0.2f);
            //���� �۹� null �ʱ�ȭ
            currentCrop = null;
        }
    }
}
