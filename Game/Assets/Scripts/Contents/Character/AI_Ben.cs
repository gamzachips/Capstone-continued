using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Ben : MonoBehaviour
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
        Restaurant1,
        Restaurant2,
        Work,
        Park
    }

    //Transforms
    public Transform homePos;
    public Transform restaurant1Pos;
    public Transform restaurant1ForwardPos;
    public Transform restaurant2Pos;
    public Transform restaurant2ForwardPos;
    public Transform[] workPoses;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoToRestaruant;
    public int TimeToGoHome;


    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    State state = State.None;
    [SerializeField]
    Location location = Location.Home;

    [SerializeField] GameObject beer;
    [SerializeField] GameObject food1;
    [SerializeField] GameObject food2;

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

        else if (state != State.Move && finishedAct && Managers.Time.GetHour() == TimeToGoToRestaruant)
        {
            int rand = Random.Range(0, 2);
            if(rand == 0)
            {
                agent.destination = restaurant1Pos.position;
                location = Location.Restaurant1;
            }
            else
            {
                agent.destination = restaurant2Pos.position;
                location = Location.Restaurant2;
            }
            
            Move();
            gpt.nowState = "going to the restaurant for dinner";
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
            if (location != Location.Work)
                StartCoroutine(WaitAndSetStateNone());
            anim.SetTrigger("stop");

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Restaurant1:
                    OnRestaurant1();
                    break;
                case Location.Restaurant2:
                    OnRestaurant2();
                    break;
                case Location.Work:
                    DoWork();
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
            if (state != State.Move && location == Location.Restaurant1)
                OnRestaurant1();
            if(state != State.Move && location == Location.Restaurant2)
                OnRestaurant2();
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

    void OnRestaurant1()
    {
        transform.LookAt(restaurant1ForwardPos.position);
        anim.SetTrigger("drink");
        gpt.nowState = "drinking and talking with the cheif in the restaurant";
        beer.SetActive(true);
        food1.SetActive(true);
    }

    void OnRestaurant2()
    {
        transform.LookAt(restaurant2ForwardPos.position);
        anim.SetTrigger("eat");
        gpt.nowState = "having dinner in the restaurant";
        food2.SetActive(true);
    }

}
