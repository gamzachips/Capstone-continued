using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class AI_Jack : MonoBehaviour
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
        Chop,
        Pan,
        Counter
    }

    //Transforms
    public Transform homePos;
    public Transform counterPos;
    public Transform counterForwardPos;
    public Transform chopPos;
    public Transform chopForwardPos;
    public Transform panPos;
    public Transform panForwardPos;
    public Transform fryingpanStovePos;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoHome;

    //Items
    public GameObject knife;
    public GameObject turnner;
    public GameObject fryingpan;
    public GameObject fryingpanPos;
    public GameObject foods;
    public GameObject cutFoods;
    public GameObject cutFoods2;

    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    private State state = State.None;
    [SerializeField]
    private Location location = Location.Home;

    bool isTalking = false;

    bool finishedAct = true;
    bool isDoorOpend = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();
        knife.SetActive(false);
        turnner.SetActive(false);
        foods.SetActive(false);
        cutFoods.SetActive(false);
        cutFoods2.SetActive(false);
    }

    private void Update()
    {
        if (agent == null) return;

        if (state == State.None && Managers.Time.GetHour() == TimeToGoToWork)
        {
            //이동한다. 
            agent.destination = counterPos.position;
            Move();
            location = Location.Counter;
        }
        else if(state == State.None && TimeToGoToWork < Managers.Time.GetHour() && Managers.Time.GetHour() < TimeToGoHome )
        {
            if (finishedAct)
            {
                finishedAct = false;
                StopAllCoroutines();
                int rand = Random.Range(0, 3);
                Move();
                switch (rand)
                {
                    case 0:
                        if (location == Location.Counter)
                        {
                            anim.SetTrigger("stop");
                            break;
                        }
                        agent.destination = counterPos.position;
                        location = Location.Counter;
                        break;
                    case 1:
                        if (location == Location.Chop)
                        {
                            anim.SetTrigger("stop");
                            break;
                        }
                        agent.destination = chopPos.position;
                        location = Location.Chop;
                        break;
                    case 2:
                        if (location == Location.Pan)
                        {
                            anim.SetTrigger("stop");
                            break;
                        }
                        agent.destination = panPos.position;
                        location = Location.Pan;
                        break;
                    default:
                        break;
                }
                
            }
        }
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoHome)
        {
            //이동한다. 
            agent.destination = homePos.position;
            Move();
            location = Location.Home;
        }


        //Move 상태이고 목적지에 도달했으면
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state == State.Move)
        {
            state = State.Act;
            StopAllCoroutines();

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Counter:
                    StartCoroutine(FinishCounterActCoroutine());
                    break;
                case Location.Pan:
                    StartCoroutine(PanActCoroutine());
                    break;
                case Location.Chop:
                    StartCoroutine(ChopActCoroutine());
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
            if (state == State.Move)
                anim.SetTrigger("walk");
        }
    }
    void Move()
    {
        state = State.Move;
        anim.SetTrigger("walk");
    }

    private IEnumerator PanActCoroutine()
    {
        transform.LookAt(panForwardPos);
        anim.SetTrigger("stop");
        yield return new WaitForSeconds(1f);

        anim.SetTrigger("pan");
        transform.LookAt(panForwardPos);
        yield return new WaitForSeconds(15f);
        finishedAct = true;
        state = State.None;
    }

    private IEnumerator ChopActCoroutine()
    {
        transform.LookAt(chopForwardPos);
        anim.SetTrigger("stop");
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("chop");
        transform.LookAt(chopForwardPos);
        yield return new WaitForSeconds(15f);
        finishedAct = true;
        state = State.None;
    }

    private IEnumerator FinishCounterActCoroutine()
    {
        transform.LookAt(counterForwardPos);
        anim.SetTrigger("stop");
        yield return new WaitForSeconds(1f);
        transform.LookAt(counterForwardPos);
        yield return new WaitForSeconds(20f);
        finishedAct = true;
        state = State.None;
    }
    public void GrabFryingPan()
    {
        fryingpan.transform.parent = fryingpanPos.transform;
        fryingpan.transform.position = fryingpanPos.transform.position;
        fryingpan.transform.rotation = fryingpanPos.transform.rotation;
    }

    public void LayDownFryPan()
    {
        fryingpan.transform.parent = fryingpanStovePos;
        fryingpan.transform.position = fryingpanStovePos.position;
        fryingpan.transform.rotation = fryingpanStovePos.rotation;
    }
    public void StartChop()
    {
        foods.SetActive(true);
        knife.SetActive(true);
        StartCoroutine(Chopping());
    }

    private IEnumerator Chopping()
    {
        yield return new WaitForSeconds(1.5f);
        foods.SetActive(false);
        cutFoods.SetActive(true);
        yield return new WaitForSeconds(3f);
        cutFoods2.SetActive(true);
    }
    public void EndChop()
    {
        knife.SetActive(false);
        cutFoods.SetActive(false);
        cutFoods2.SetActive(false);
    }
    public void StartFrying()
    {
        turnner.SetActive(true);
        GrabFryingPan();
    }
    public void EndFrying()
    {
        turnner.SetActive(false);
        LayDownFryPan();
    }


    //Collision - Door
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            other.gameObject.GetComponent<RestaurantDoor>().OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            other.gameObject.GetComponent<RestaurantDoor>().CloseDoor();
        }
    }
}
