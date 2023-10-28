using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AI_Chicken : MonoBehaviour
{

    enum State
    {
        None,
        Act
    }

    Animator anim;
    NavMeshAgent agent;

    State state = State.None;

    [SerializeField]
    int timeToMakeEgg;

    [SerializeField]
    GameObject egg;

    bool eggMaden = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (agent == null) return;

        if (Managers.Time.GetHour() == timeToMakeEgg && !eggMaden) //달걀 생성
        {
            GameObject newEgg = Instantiate(egg, transform);
            eggMaden = true;
            newEgg.transform.parent = null;
        }
        if (Managers.Time.GetHour() == 0)
            eggMaden = false;

        if (state == State.None)
        {
            int randValue = Random.Range(0, 5);
            switch(randValue)
            {
                case 0: //걷기(5초)
                    Walk();
                    break;
                case 1: //달리기(3초)
                    Run();
                    break;
                case 2: //먹기
                    Eat();
                    break;
                case 3: //두리번거리기
                    LookAround();
                    break;
                case 4: //가만히 있기
                    StartCoroutine(WaitAndSetStateNone(5f, null));
                    break;
            }
            state = State.Act;
        }
    }

    private void Walk()
    {
        Move();
        agent.speed = 2f;
        agent.acceleration = 0.2f;
        anim.SetBool("Walk", true);
        StartCoroutine(WaitAndSetStateNone(5f, "Walk"));
    }
    private void Run()
    {
        Move();
        agent.speed = 4f;
        agent.acceleration = 0.4f;
        anim.SetBool("Run", true);
        StartCoroutine(WaitAndSetStateNone(2f, "Run"));
    }
    private void Eat()
    {
        anim.SetBool("Eat", true);
        StartCoroutine(WaitAndSetStateNone(3f, "Eat"));
    }
    private void LookAround()
    {
        anim.SetBool("Turn Head", true);
        StartCoroutine(WaitAndSetStateNone(3f, "Turn Head"));
    }
    private IEnumerator WaitAndSetStateNone(float time, string animation)
    {
        yield return new WaitForSeconds(time);
        state = State.None;
        if(animation != null)
            anim.SetBool(animation, false);
        agent.acceleration = 0f;
        agent.speed = 0f;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    private void Move()
    {
        float rot = Random.Range(-90, 90);
        Vector3 moveDirection = new Vector3(Mathf.Cos(rot * Mathf.Deg2Rad), 0f, Mathf.Sin(rot* Mathf.Deg2Rad));
        Vector3 target = transform.position + moveDirection * 2;
        agent.isStopped = false;

        agent.destination = target;
    }

}
