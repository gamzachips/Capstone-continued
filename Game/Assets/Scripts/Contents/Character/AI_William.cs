using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using OpenAI;

public class AI_William : MonoBehaviour
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
        Work
    }

    //Transforms
    public Transform homePos;
    public Transform benchPos;
    public Transform[] workPoses;

    //Times
    public int TimeToGoToWork;
    public int TimeToGoBackHome;

    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    State state = State.None;
    Location location = Location.Home;

    bool finishedAct = false;
    bool isTalking = false;
    int nowIndex = 0;

    float agentAccel;

    ChatGPT gpt;

private void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();

        agentAccel = agent.acceleration;

        gpt = gameObject.transform.Find("ToActivate").GetComponentInChildren<ChatGPT>();
    }

    private void Update()
    {
        if (agent == null) return;

        //아무것도 안하고있고 TimeToGoToWork시이면 
        if (state == State.None && Managers.Time.GetHour() == TimeToGoToWork)
        {
            //이동한다. 
            agent.destination = workPoses[nowIndex].position;
            gpt.nowState = "옥수수 농장에 일하러 가는 중";
            MoveToWork();
        }

        //일이 끝났고 TimeToGoBackHome시 이후면
        if (state == State.Act && finishedAct && Managers.Time.GetHour() >= TimeToGoBackHome)
        {
            agent.destination = homePos.position;
            gpt.nowState = "일을 끝내고 집에 가는 중";
            state = State.None;
            anim.SetTrigger("walk");
        }

        //Move 상태이고 목적지에 도달했으면
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state==State.Move)
        {
            state = State.Act;
            anim.SetTrigger("stop");

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Work:
                    DoWork();
                    gpt.nowState = "옥수수 농장에서 일 하는 중(잡초 뽑기)";
                    break;

            }
        }

        //Act 상태이고 행동이 끝났으면 
        if(state == State.Act && finishedAct == true)
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

        //플레이어가 대화를 걸었을 때 
        if(dialog.Talking == true && isTalking == false)
        {
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            isTalking = true;
            StopAllCoroutines();

        }
        //대화가 끝났을 때
        if (dialog.Talking == false && isTalking == true)
        {
            isTalking = false;
            agent.isStopped = false;
            agent.acceleration = agentAccel;
            if (state == State.Move)
                anim.SetTrigger("walk");
            else if (state == State.Act && location == Location.Work)
            {
                finishedAct = false;
                DoWork();
            }

        }
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
}
