using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreeShaker : MonoBehaviour
{
    private GameObject currentCrop = null; //���� Ʈ���ſ� ���� ����


    void Update() //�� �����Ӹ���
    {
        //���� Ʈ���ſ� ���� ���� �ڶ� ���¿��� EŰ ���� ��
        if (currentCrop != null && currentCrop.GetComponent<TreeField>().IsGrown)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                //�ؽ�Ʈ ��Ȱ��ȭ
                Managers.UI.DisableInteractText();

                //���� ����
                StartCoroutine(ShakeTree());

            }
        }
    }

    private void OnTriggerEnter(Collider other) //Ʈ���� ���� ��
    {

        if (other.CompareTag("TreeTrigger") && currentCrop == null)
        {
            //���� Ʈ���ſ� ���� ������ ������ ���� ���� ������ ����
            currentCrop = other.gameObject.transform.parent.gameObject;

            if (currentCrop.GetComponent<TreeField>().IsGrown)
            {
                //�ؽ�Ʈ Ȱ��ȭ
                Managers.UI.SetInteractText("����[E]");
                Managers.UI.EnableInteractText();
            }
        }
    }

    private void OnTriggerExit(Collider other) //Ʈ���ſ��� �������� ��
    {
        if (other.CompareTag("TreeTrigger"))
        {
            //�ؽ�Ʈ ��Ȱ��ȭ
            Managers.UI.DisableInteractText();

            //���� ���� ���� null�� �ʱ�ȭ
            currentCrop = null;
        }
    }

    private IEnumerator ShakeTree()
    {
        TreeField treefield = currentCrop.GetComponentInChildren<TreeField>();

        //���� ����

        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("shake_tree");
        Managers.Energy.DecreaseEnergy(1f);

        yield return new WaitForSeconds(1);

        Rigidbody[] rigidbodys = treefield.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody r in rigidbodys)
        {
            r.useGravity = true;
        }

        BoxCollider[] childrenBox = treefield.GetComponentsInChildren<BoxCollider>();
         
        foreach (BoxCollider box in childrenBox)
        {
            box.enabled = true;
        }

        StartCoroutine(treefield.GrowTreeAfterDelay());
        treefield.IsGrown = false;
        
    }
}
