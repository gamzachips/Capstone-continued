using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Farmer : MonoBehaviour
{
    private GameObject currentCropField = null; //현재 트리거에 잡힌 작물 영역

    private bool isWaitingForSelecting = false;


    void Update() //매 프레임마다
    {
        //현재 트리거에 잡힌 작물 있고 작물이 자라는 중이 아니라면
        if (currentCropField != null && currentCropField.GetComponent<CropField>().State != CropField.FieldState.Growing)
        {
            //E키 누르면
            if (Input.GetKeyDown(KeyCode.E))
            {
                  GameObject crop = currentCropField;
                  //텍스트 비활성화
                  Managers.UI.DisableFarmingUI();

                  if (crop != null)
                  {
                        //플레이어 상태 변경
                        GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;

                        //자란 상태면 작물 수확
                        if (currentCropField.GetComponent<CropField>().State == CropField.FieldState.Grown)
                        {
                            Animator anim = GetComponent<Animator>();
                            anim.SetTrigger("pull_plant");
                            StartCoroutine(HarvestCrop());
                        }
                        //작물이 없으면 심기
                        else if (currentCropField.GetComponent<CropField>().Crop == null)
                        {
                            //UI보여주기 
                            Managers.UI.DisableInteractText();
                            Managers.UI.EnableFarmingUI();

                            //작물 고르고 심기
                            StartCoroutine(WaitforSelecting());
                            
                    }
                  }
            }
        }
    }
    private void OnTriggerStay(Collider other) //트리거 잡힐 때
    {
        if (other.CompareTag("CropTrigger") && currentCropField == null)
        {
            //현재 트리거에 잡힌 필드 저장
            currentCropField = other.transform.parent.gameObject;

            //작물이 자란 상태면
            if (currentCropField.GetComponent<CropField>().State == CropField.FieldState.Grown)
            {
                Managers.UI.SetInteractText("수확하기[E]");
                Managers.UI.EnableInteractText();
            }
            //작물이 없으면
            else if (currentCropField.GetComponent<CropField>().Crop == null)
            {
                Managers.UI.SetInteractText("심기[E]");
                Managers.UI.EnableInteractText();
            }
        }
    }


    private void OnTriggerExit(Collider other) //트리거에서 빠져나올 때
    {
        if (other.CompareTag("CropTrigger"))
        {
            //텍스트 비활성화
            Managers.UI.DisableInteractText();

            //현재 잡힌 작물 null로 초기화
            currentCropField = null;
        }
    }


    private IEnumerator PlantCrop(string cropId)
    {
        yield return new WaitForSeconds(1);
        currentCropField.GetComponent<CropField>().Plant(cropId);

        GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
    }

    private IEnumerator HarvestCrop() //작물 수확
    {
        //작물이 수확 가능한 상태일 때
        if(currentCropField != null && currentCropField.GetComponent<CropField>().State == CropField.FieldState.Grown)
        {
            GameObject crop = currentCropField.GetComponent<CropField>().Crop;
            yield return new WaitForSeconds(1f);
            Managers.Energy.DecreaseEnergy(2);

            if (crop != null)
            {
                FarmCrop farmCrop = crop.GetComponent<FarmCrop>();
                int cropNum = farmCrop.CropNum;
                //인벤토리에 아이템 추가
                Inventory.instance.AddItem(farmCrop.CropId, cropNum);

                //텍스트 띄우기
                Managers.UI.HarvestText(Managers.Data.GetCropData(farmCrop.CropId).name, cropNum);
                StartCoroutine(WaitAndClearHarvestText());

                //기존 작물 삭제
                GameObject.Destroy(crop);
                currentCropField.GetComponent<CropField>().State = CropField.FieldState.Empty;

                //현재 작물 null 초기화
                currentCropField = null;
            }

            GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;

        }
    }

    private IEnumerator WaitforSelecting()
    {
        isWaitingForSelecting = true;

        while(isWaitingForSelecting)
        {
            //입력 있을 때까지 대기
            yield return null;

           if (Input.GetKeyDown(KeyCode.Escape)) //ESC누르면 종료
                isWaitingForSelecting = false;
        }
        
        Managers.UI.DisableFarmingUI();
    }

    IEnumerator WaitAndClearHarvestText()
    {
        yield return new WaitForSeconds(0.5f);
        Managers.UI.ClearHarvestText();
    }

    public void SelectCrop(string cropId)
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("pull_plant");
        StartCoroutine(PlantCrop(cropId));
        isWaitingForSelecting = false;
        Managers.Energy.DecreaseEnergy(3); 
    }
}
