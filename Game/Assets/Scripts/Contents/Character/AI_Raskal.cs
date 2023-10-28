using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;

public class AI_Raskal : MonoBehaviour
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
        Work,
        FrontHome
    }

    //Transforms
    public Transform homePos;
    public Transform restaurantPos;
    public Transform restaurantForwardPos;
    public Transform frontHomePos;
    public Transform frontHomeForwardPos;
    public Transform[] workPoses;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoToRestaruant;
    public int TimeToGoToFrontHome;
    public int TimeToGoHome;


    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    State state = State.None;
    [SerializeField]
    Location location = Location.Home;

    [SerializeField] GameObject beer;
    [SerializeField] GameObject food;

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

        gpt = gameObject.transform.Find("ToActivate").GetComponentInChildren<ChatGPT>();

    }

    private void Update()
    {
        if (agent == null) return;

        anim.SetFloat("speed", agent.velocity.magnitude);

        if (state == State.None && Managers.Time.GetHour() == TimeToGoToWork)
        {
            //이동한다. 
            agent.destination = workPoses[nowIndex].position;
            gpt.nowState = "going to work(weed in the field)";
            MoveToWork();
        }

        else if (state != State.Move && finishedAct && Managers.Time.GetHour() == TimeToGoToRestaruant && location != Location.Restaurant )
        {
            agent.destination = restaurantPos.position;
            Move();
            location = Location.Restaurant;
            gpt.nowState = "going to the restaurant for dinner";
        }
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoToFrontHome)
        {
            //이동한다. 
            beer.SetActive(false);
            food.SetActive(false);
            agent.destination = frontHomePos.position;
            anim.SetTrigger("stop");
            Move();
            location = Location.FrontHome;
            gpt.nowState = "going back to home";

        }
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoHome)
        {
            //이동한다. 
            agent.destination = homePos.position;
            anim.SetTrigger("stop");
            Move();
            location = Location.Home;
            gpt.nowState = "going home to sleep";

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
            if(location != Location.Work)
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
                case Location.FrontHome:
                    OnFrontHome();
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
        anim.SetTrigger("sitrest");
        gpt.nowState = "having dinner with beer in the restaurant";
        beer.SetActive(true);
        food.SetActive(true);
    }

    void OnFrontHome()
    {
        transform.LookAt(frontHomeForwardPos.position);
        anim.SetTrigger("sit");
        gpt.nowState = "listening to music";
    }

}
