using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreeShaker : MonoBehaviour
{
    private GameObject currentCrop = null; //현재 트리거에 잡힌 나무


    void Update() //매 프레임마다
    {
        //현재 트리거에 잡힌 나무 자란 상태에서 E키 누를 때
        if (currentCrop != null && currentCrop.GetComponent<TreeField>().IsGrown)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                //텍스트 비활성화
                Managers.UI.DisableInteractText();

                //나무 흔들기
                StartCoroutine(ShakeTree());

            }
        }
    }

    private void OnTriggerEnter(Collider other) //트리거 잡힐 때
    {

        if (other.CompareTag("TreeTrigger") && currentCrop == null)
        {
            //현재 트리거에 잡힌 나무가 없으면 지금 잡힌 나무를 저장
            currentCrop = other.gameObject.transform.parent.gameObject;

            if (currentCrop.GetComponent<TreeField>().IsGrown)
            {
                //텍스트 활성화
                Managers.UI.SetInteractText("흔들기[E]");
                Managers.UI.EnableInteractText();
            }
        }
    }

    private void OnTriggerExit(Collider other) //트리거에서 빠져나올 때
    {
        if (other.CompareTag("TreeTrigger"))
        {
            //텍스트 비활성화
            Managers.UI.DisableInteractText();

            //현재 잡힌 나무 null로 초기화
            currentCrop = null;
        }
    }

    private IEnumerator ShakeTree()
    {
        TreeField treefield = currentCrop.GetComponentInChildren<TreeField>();

        //나무 흔들기

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
