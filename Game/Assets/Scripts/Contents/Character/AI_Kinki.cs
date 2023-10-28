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

        //아무것도 안하고있고 TimeToGoRestaurant시이면  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoRestaurant)
        {
            //레스토랑으로 이동한다. 
            agent.destination = restaurantPos.position;
            Move();
            gpt.nowState = "햄버거 먹으러 식당에 가고있음";
            location = Location.Restaurant;
        }
        //움직이지 않고있고 TimeToGoHome1시이면  
        if (state == State.Act && Managers.Time.GetHour() == TimeToGoHome1)
        {
            StandUp();
            //집으로 이동한다. 
            agent.destination = homePos.position;
            Move();
            location = Location.Home;
        }
        //아무것도 안하고있고 TimeToGoForaWalk시이면  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoForaWalk)
        {
            //산책장소로 이동한다. 
            agent.destination = walkPos.position;
            gpt.nowState = "잠시 산책 나옴";
            Move();
            location = Location.OnAWalk;
        }
        //아무것도 안하고있고 TimeToGoHome2시이면  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoHome2)
        {
            //집으로 이동한다. 
            agent.destination = homePos.position;
            gpt.nowState = "산책을 끝내고 집에 가는 중";
            Move();
            location = Location.Home;
        }


        //Move 상태이고 목적지에 도달했으면
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

        //플레이어가 대화를 걸었을 때 
        if (dialog.Talking == true && isTalking == false)
        {
            agent.acceleration = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            
            isTalking = true;
        }
        //대화가 끝났을 때
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
