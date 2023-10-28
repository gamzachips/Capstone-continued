using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeField : MonoBehaviour
{
    public GameObject grownTreePrefab;
    private GameObject currentTree;
    public float genertateTime = 3f;

    private bool isGrown ;

    public bool IsGrown
    { 
        get { return isGrown; }
        set { isGrown = value; }
    }

    private void Start()
    {
        currentTree = transform.Find("Tree").gameObject;
        isGrown = true;
    }



    public IEnumerator GrowTreeAfterDelay()
    { //나무 다시 자란 상태로
        yield return new WaitForSeconds(genertateTime);

        Transform treeTransform = currentTree.transform;

        //나무에 있는 열매들을 나무로부터 분리
        treeTransform.DetachChildren();
        
        //새로운 나무 생성
        GameObject newTree = Instantiate(grownTreePrefab, treeTransform.position, treeTransform.rotation, transform);

        //기존 나무 삭제
        Destroy(currentTree);

        currentTree = newTree;

        isGrown = true;
    }

}
