using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class AI_Cheif : MonoBehaviour
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
        Restaurant,
        Market,
        Park,
        Work
    }

    //Transforms
    public Transform homePos;
    public Transform restaurantPos;
    public Transform restaurantForwardPos;
    public Transform marketPos;
    public Transform marketForwardPos;
    public Transform parkPos;
    public Transform[] workPoses;
    public Transform BeerGlassTablePos;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoMarketOrPark;
    public int TimeToGoToRestaruant;
    public int TimeToGoHome;

    //Items
    public GameObject BeerGlass;
    public GameObject BeerGlassPos;
    

    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    State state = State.None;
    [SerializeField]
    Location location = Location.Home;

    int type = 1;
    bool randValueSelected = false;
    bool isTalking = false;

    bool finishedAct = true;
    int nowIndex = 0;

    ChatGPT gpt;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();
        anim.SetTrigger("walk");

        BeerGlass.SetActive(false);
        gpt = gameObject.transform.Find("ToActivate").GetComponentInChildren<ChatGPT>();

    }

    private void Update()
    {
        if (agent == null) return;

        if(randValueSelected == false && Managers.Time.GetHour() == 0)
        {
            type = Random.Range(1, 3);
            randValueSelected = true;
        }

        if (state == State.None && Managers.Time.GetHour() == TimeToGoToWork)
        {
            //이동한다. 
            agent.destination = workPoses[nowIndex].position;
            gpt.nowState = "going to work(weed in the field)";
            MoveToWork();
        }

        else if (state != State.Move && finishedAct && Managers.Time.GetHour() == TimeToGoMarketOrPark)
        {
            if(type == 1)
            {
                agent.destination = marketPos.position;
                Move();
                location = Location.Market;
                gpt.nowState = "going to the market";
            }
            else if (type == 2)
            {
                agent.destination = parkPos.position;
                Move();
                location = Location.Park;
                gpt.nowState = "going to the park.";
            }
        }
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoToRestaruant)
        {
            agent.destination = restaurantPos.position;
            Move();
            location = Location.Restaurant;
            gpt.nowState = "going to the restaurant.";

        }
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoHome)
        {
            //이동한다. 
            agent.destination = homePos.position;
            anim.SetTrigger("stop");
            BeerGlass.SetActive(false);
            Move();
            location = Location.Home;
            gpt.nowState = "going to home to sleep";

        }
        //Act 상태이고 행동이 끝났으면 
        else if (location == Location.Work && state == State.Act && finishedAct == true)
        {
            //목적지를 랜덤하게 선택해 이동한다. 
            int idx;
            while (true)
            {
                idx = Random.Range(0, workPoses.Length);
                if (idx != nowIndex) break;
            }
            nowIndex = idx;

            agent.destination = workPoses[nowIndex].position;

            //Move 상태로 바꾸고 위치를 지정한다
            MoveToWork();
        }


        //Move 상태이고 목적지에 도달했으면
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state == State.Move)
        {
            state = State.Act;
            if (location != Location.Work)
                StartCoroutine(WaitAndSetStateNone());
            anim.SetTrigger("stop");

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Restaurant:
                    OnRestaurant();
                    break;
                case Location.Work:
                    DoWork();
                    break;
                case Location.Market:
                    break;
                case Location.Park:
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
            if (state != State.Move && location == Location.Restaurant)
                OnRestaurant();
        }
    }
    void Move()
    {
        state = State.Move;
        anim.SetTrigger("walk");
    }

    private IEnumerator WaitAndSetStateNone()
    {
        yield return new WaitForSeconds(Managers.Time.GetOneHourTime());

        state = State.None;
    }

    void MoveToWork()
    {
        state = State.Move;
        location = Location.Work;
        anim.SetTrigger("walk");
        finishedAct = false;
        StopAllCoroutines();
    }

    void DoWork()
    {
        gpt.nowState = "working(weeding) in the field next to the house";
        StartCoroutine(PlayWorkAimCoroutine());
    }

    private IEnumerator PlayWorkAimCoroutine()
    {
        anim.SetTrigger("pull_plant");
        //15.5초 후 애니메이션을 멈춘다
        yield return new WaitForSeconds(15.5f);
        finishedAct = true;
        anim.SetTrigger("stop");
    }

    void OnRestaurant()
    {
        transform.LookAt(restaurantForwardPos.position);
        anim.SetTrigger("drink");
        BeerGlass.SetActive(true);
        gpt.nowState = "drink beear in the restaurant";
    }


    public void GrabBeerGlass()
    {
        BeerGlass.transform.parent = BeerGlassPos.transform;
        BeerGlass.transform.position = BeerGlassPos.transform.position;
        BeerGlass.transform.rotation = BeerGlassPos.transform.rotation;
    }

    public void LayDownBeerGlass()
    {
        BeerGlass.transform.parent = BeerGlassTablePos;
        BeerGlass.transform.position = BeerGlassTablePos.position;
        BeerGlass.transform.rotation = BeerGlassTablePos.rotation;
    }
}
