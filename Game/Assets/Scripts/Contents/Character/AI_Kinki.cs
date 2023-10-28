using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using OpenAI;

public class AI_Kinki : MonoBehaviour
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
        OnAWalk
    }

    //Transforms
    public Transform homePos;
    public Transform restaurantPos;
    public Transform restaurantLookAtPos;
    public Transform walkPos;

    //Times
    public int TimeToGoRestaurant;
 
    public int TimeToGoHome1;
    public int TimeToGoForaWalk;
    public int TimeToGoHome2;

    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    State state = State.None;
    [SerializeField]
    Location location = Location.Home;

    bool isTalking = false;

    public GameObject hamburger;
    public GameObject coke;
    public GameObject fries;

    float agentAccel;

    ChatGPT gpt;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();

        hamburger.SetActive(false);
        coke.SetActive(false);
        fries.SetActive(false);

        agentAccel = agent.acceleration;
        gpt = gameObject.transform.Find("ToActivate").GetComponentInChildren<ChatGPT>();

    }

    private void Update()
    {
        if (agent == null) return;
        anim.SetFloat("speed", agent.velocity.magnitude);

        //�ƹ��͵� ���ϰ��ְ� TimeToGoRestaurant���̸�  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoRestaurant)
        {
            //����������� �̵��Ѵ�. 
            agent.destination = restaurantPos.position;
            Move();
            gpt.nowState = "�ܹ��� ������ �Ĵ翡 ��������";
            location = Location.Restaurant;
        }
        //�������� �ʰ��ְ� TimeToGoHome1���̸�  
        if (state == State.Act && Managers.Time.GetHour() == TimeToGoHome1)
        {
            StandUp();
            //������ �̵��Ѵ�. 
            agent.destination = homePos.position;
            Move();
            location = Location.Home;
        }
        //�ƹ��͵� ���ϰ��ְ� TimeToGoForaWalk���̸�  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoForaWalk)
        {
            //��å��ҷ� �̵��Ѵ�. 
            agent.destination = walkPos.position;
            gpt.nowState = "��� ��å ����";
            Move();
            location = Location.OnAWalk;
        }
        //�ƹ��͵� ���ϰ��ְ� TimeToGoHome2���̸�  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoHome2)
        {
            //������ �̵��Ѵ�. 
            agent.destination = homePos.position;
            gpt.nowState = "��å�� ������ ���� ���� ��";
            Move();
            location = Location.Home;
        }


        //Move �����̰� �������� ����������
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state == State.Move)
        {
            state = State.None;
            anim.SetTrigger("stop");

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Restaurant:
                    gpt.nowState = "eating hamburger in the restaurant";
                    SitAndEatBurger();
                    break;
                case Location.OnAWalk:
                    break;
                default:
                    break;
            }
        }

        //�÷��̾ ��ȭ�� �ɾ��� �� 
        if (dialog.Talking == true && isTalking == false)
        {
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            
            isTalking = true;
        }
        //��ȭ�� ������ ��
        if (dialog.Talking == false && isTalking == true)
        {
            agent.acceleration = agentAccel;
            agent.isStopped = false;
            
            isTalking = false;
            if (state != State.Move && location == Location.Restaurant)
                SitAndEatBurger();
        }
    }
    void Move()
    {
        state = State.Move;
    }

    void SitAndEatBurger()
    {
        transform.LookAt(restaurantLookAtPos.position);
        state = State.Act;
        anim.SetTrigger("sit");
        hamburger.SetActive(true);
        coke.SetActive(true);
        fries.SetActive(true);
    }

    void StandUp()
    {
        anim.SetTrigger("stop");
        state = State.None;
        hamburger.SetActive(false);
        coke.SetActive(false);
        fries.SetActive(false);
    }

}
