using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Claire : MonoBehaviour
{
    enum State
    {
        None,
        Move,
        Act
    }
    enum Location
    {
        Home,
        Counter
    }

    //Transforms
    public Transform homePos;
    public Transform counterPos;
    public Transform counterForwardPos;
    public Transform talkingForwardPos;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoHome;


    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    private State state = State.None;
    [SerializeField]
    private Location location = Location.Home;

    bool isTalking = false;

    [SerializeField] GameObject door_opend;
    [SerializeField] GameObject door_cloed;
    bool isDoorOpend = false;

    int hour;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();
    }

    private void Update()
    {
        if (agent == null) return;

        anim.SetFloat("speed", agent.velocity.magnitude);

        hour = Managers.Time.GetHour();

        if (state == State.None && hour == TimeToGoToWork)
        {
            //이동한다. 
            agent.destination = counterPos.position;
            state = State.Move;
            location = Location.Counter;
        }
        else if (state == State.None && hour == TimeToGoHome)
        {
            //이동한다. 
            agent.destination = homePos.position;
            state = State.Move;
            location = Location.Home;
        }
        //Move 상태이고 목적지에 도달했으면
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state == State.Move)
        {
            state = State.Act;
            StartCoroutine(WaitAndSetStateNone());

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Counter:
                    OnCounter();
                    break;
                default:
                    break;
            }
        }

        //플레이어가 대화를 걸었을 때 
        if (dialog.Talking == true && isTalking == false)
        {
            agent.isStopped = true;
            anim.SetTrigger("stop");
            isTalking = true;
        }
        //대화가 끝났을 때
        if (dialog.Talking == false && isTalking == true)
        {
            agent.isStopped = false;
            isTalking = false;
        }
    }

    void OnCounter()
    {
        transform.LookAt(counterForwardPos);
    }

    private IEnumerator WaitAndSetStateNone()
    {
        yield return new WaitForSeconds(Managers.Time.GetOneHourTime());

        state = State.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if( hour >= TimeToGoToWork && other.CompareTag("Door"))
        {
            door_opend.SetActive(true);
            door_cloed.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hour >= TimeToGoHome && other.gameObject.CompareTag("Door"))
        {
            door_opend.SetActive(false);
            door_cloed.SetActive(true);
        }
    }
}
