using OpenAI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject dialogCamera;
    [SerializeField] private GameObject toActivate;
    [SerializeField] private CharacterType npcType;
    [SerializeField] private GameObject giftInvenUI;
    [SerializeField] private GameObject npcGift;

    private Transform npc;
    private Transform player;
    public Transform Player { get { return player; } }

    private bool isTalking = false;
    public bool Talking { get { return isTalking; } set { isTalking = value; } }
    private bool isFacing = false;

    private bool giftselecting = false;

    private void Start()
    {
        npc = gameObject.transform.parent;
    }

    void Update()
    {
        // ESC Escape
        if (isTalking && Input.GetKeyDown(KeyCode.Escape))
        {
            Recover();
            npc.Find("ToActivate").GetComponentInChildren<ChatGPT>().ResetDialogs();
            Managers.Time.RunTime();
            giftInvenUI.SetActive(false);
        }

        if(isTalking && !isFacing)
        {
            Vector3 directionToPlayer = player.position - npc.position;
            npc.rotation = Quaternion.Lerp(npc.transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime);

            Vector3 directionToNPC = npc.position - player.position;
            player.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(directionToNPC), Time.deltaTime);
        }

        if (isTalking && Input.GetKeyDown(KeyCode.Tab))
        {
            //선물하기 
            giftselecting = !giftselecting;
            if(giftselecting)
            {
                giftInvenUI.SetActive(true);
                giftInvenUI.GetComponent<GiftInvenUI>().NpcGift = npcGift.GetComponent<Gift>();
            }
            else
                giftInvenUI.SetActive(false);   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Managers.UI.SetInteractText("대화하기[E]");
            Managers.UI.EnableInteractText();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E) && isTalking == false)
        {
            Managers.UI.DisableInteractText();
            Talking = true;

            npc.Find("ToActivate").GetComponentInChildren<ChatGPT>().UpdateLikeability();

            player = other.transform;

            StartCoroutine(FaceEachOtherAfterDelay());
            // disable player input
            player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Interact;
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            npc.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;

            toActivate.SetActive(true);
            Managers.UI.DisableCanvas();

            //Camera
            mainCamera.SetActive(false);
            dialogCamera.SetActive(true);

            // display cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            toActivate.GetComponentInChildren<ChatGPT>().NPCType = npcType;


        }

      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Managers.UI.DisableInteractText();

        }
    }


    public void Recover()
    {
        player.GetComponent<PlayerController>().State = PlayerController.PlayerState.Idle;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
        npc.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;


        toActivate.SetActive(false);
        Managers.UI.EnableCanvas();
        Managers.Time.RunTime();

        Talking = false;
        isFacing = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Camera
        mainCamera.SetActive(true);
        dialogCamera.SetActive(false);
    }

    private IEnumerator FaceEachOtherAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        npc.GetComponent<Animator>().SetTrigger("stop");
        isFacing = true;
        Managers.Time.StopTime();
    }
}
