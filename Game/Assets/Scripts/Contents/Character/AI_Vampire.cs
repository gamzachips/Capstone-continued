using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*�����̾�
 *  18�� : ������ ���ͼ� �Ĵ����� ��
 *  24�� : �Ĵ翡�� ���� ���̳� ȣ�� ��å
 *  4�� : ��ġ�� �ɾ�����
 *  6�� : ���� ��
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

        //�ƹ��͵� ���ϰ��ְ� TimeToGoRestaurant���̸�  
        if (state == State.None && Managers.Time.GetHour() == TimeToGoRestaurant)
        {
            //����������� �̵��Ѵ�. 
            agent.destination = restaurantPos.position;
            Move();
            location = Location.Restaurant;
        }
        
        else if (state == State.None && Managers.Time.GetHour() == TimeToGoRiver)
        {
            //�� �Ǵ� ȣ���� �̵�
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

        //Move �����̰� �������� ����������
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

        //�÷��̾ ��ȭ�� �ɾ��� �� 
        if (dialog.Talking == true && isTalking == false)
        {
            agent.isStopped = true;
            anim.SetTrigger("stop");
            isTalking = true;
        }
        //��ȭ�� ������ ��
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
