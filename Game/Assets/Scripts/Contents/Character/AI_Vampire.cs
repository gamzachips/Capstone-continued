using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*뱀파이어
 *  18시 : 집에서 나와서 식당으로 감
 *  24시 : 식당에서 나와 강이나 호수 산책
 *  4시 : 벤치에 앉아있음
 *  6시 : 집에 들어감
 */

public class AI_Vampire : MonoBehaviour
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
        River
    }

    //Transforms
    public Transform homePos;
    public Transform benchForwardPos;
    public Transform riverPos;
    public Transform riverPos2;
    public Transform restaurantPos;
    public Transform restaurantForwardPos;
    public Transform WineGlassTablePos;

    //Times
    public int TimeToGoRestaurant;
    public int TimeToGoHome;
    public int TimeToGoRiver;

    //Items
    public GameObject WineGlass;
    public GameObject WineGlassPos;
    public GameObject WineBottle;
    public GameObject Food;

    Animator anim;
    NavMeshAgent agent;
    NPCDialog dialog;

    [SerializeField]
    State state = State.None;
    [SerializeField]
    Location location = Location.Home;

    bool isTalking = false;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dialog = GetComponentInChildren<NPCDialog>();
        anim.SetTrigger("walk");

        WineGlass.SetActive(false);
        Food.SetActive(false);
        WineBottle.SetActive(false);
    }


    private void Update()
    {
        if (agent == null) return;

        //아무것도 안하고있고 TimeToGoRestaurant시이면  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoRestaurant)
        {
            //레스토랑으로 이동한다. 
            agent.destination = restaurantPos.position;
            Move();
            location = Location.Restaurant;
        }
        
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoRiver)
        {
            //강 또는 호수로 이동
            StandUp();
            int rand = Random.Range(0, 2);
            if (rand == 0)
                agent.destination = riverPos.position;
            else
                agent.destination = riverPos2.position;
            Move();
            location = Location.River;

        }

        else if (state == State.None && Managers.Time.GetHour() == TimeToGoHome)
        {
            agent.destination = homePos.position;
            Move();
            location = Location.Home;
        }

        //Move 상태이고 목적지에 도달했으면
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && state == State.Move)
        {
            agent.velocity = Vector3.zero;
            state = State.Act;

            anim.SetTrigger("stop");

            switch (location)
            {
                case Location.Home:
                    break;
                case Location.Restaurant:
                    {
                        SitAndDrink();
                        break;
                    }
                case Location.River:
                    break;
                default:
                    break;
            }
            StartCoroutine(WaitAndSetStateNone());
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
                SitAndDrink();
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
    
    void SitAndDrink()
    {
        transform.LookAt(restaurantForwardPos.position);
        anim.SetTrigger("drink");
        WineGlass.SetActive(true);
        Food.SetActive(true);
        WineBottle.SetActive(true);
        transform.LookAt(restaurantForwardPos.position);

    }

    void StandUp()
    {
        anim.SetTrigger("stop");
        state = State.None;
        WineGlass.SetActive(false);
        Food.SetActive(false);
        WineBottle.SetActive(false);
    }

    public void GrabWineGlass()
    {
        WineGlass.transform.parent = WineGlassPos.transform;
    }

    public void LayDownWineGlass()
    {
        WineGlass.transform.parent = WineGlassTablePos;
        WineGlass.transform.position = WineGlassTablePos.position;
        WineGlass.transform.rotation = WineGlassTablePos.rotation;

    }

}
