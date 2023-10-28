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

        //�ƹ��͵� ���ϰ��ְ� TimeToGoToWork���̸� 
        if (state == State.None && Managers.Time.GetHour() == TimeToGoToWork)
        {
            //�̵��Ѵ�. 
            agent.destination = workPoses[nowIndex].position;
            gpt.nowState = "������ ���忡 ���Ϸ� ���� ��";
            MoveToWork();
        }

        //���� ������ TimeToGoBackHome�� ���ĸ�
        if (state == State.Act && finishedAct && Managers.Time.GetHour() >= TimeToGoBackHome)
        {
            agent.destination = homePos.position;
            gpt.nowState = "���� ������ ���� ���� ��";
            state = State.None;
            anim.SetTrigger("walk");
        }

        //Move �����̰� �������� ����������
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
                    gpt.nowState = "������ ���忡�� �� �ϴ� ��(���� �̱�)";
                    break;

            }
        }

        //Act �����̰� �ൿ�� �������� 
        if(state == State.Act && finishedAct == true)
        {
            //�������� �����ϰ� ������ �̵��Ѵ�. 
            int idx;
            while (true)
            {
                idx = Random.Range(0, workPoses.Length);
                if (idx != nowIndex) break;
            }
            nowIndex = idx;
            
            agent.destination = workPoses[nowIndex].position;

            //Move ���·� �ٲٰ� ��ġ�� �����Ѵ�
            MoveToWork();
        }

        //�÷��̾ ��ȭ�� �ɾ��� �� 
        if(dialog.Talking == true && isTalking == false)
        {
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            isTalking = true;
            StopAllCoroutines();

        }
        //��ȭ�� ������ ��
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
        //15.5�� �� �ִϸ��̼��� �����
        yield return new WaitForSeconds(15.5f);
        finishedAct = true;
        anim.SetTrigger("stop");
    }
}
