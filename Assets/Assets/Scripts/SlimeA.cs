using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeA : MonoBehaviour
{
    private GameManager _GameManager;
    private Animator anim;
    public int HP;

    private bool isDie;

    public enemyState state;

    public const float patrolWaitTime = 5f;

    private int rand;

    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;


    // Start is called before the first frame update
    void Start()
    {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();


        ChangeState(state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Died()
    {
        isDie = true;
        yield return new WaitForSeconds(2.3f);
        Destroy(this.gameObject);
    }

    #region MEUS METODOS
    void GetHit(int amount)
    {
        if(isDie == true) { return; }

        HP -= amount;

        if (HP > 0)
        {
            ChangeState(enemyState.FURY);
            anim.SetTrigger("GetHit");
        }
        else
        {
            anim.SetTrigger("Die");
            StartCoroutine("Died");
        }

    }

    void StateManager()
    {
        switch(state)
        {

            case enemyState.FOLLOW:

                break;

            case enemyState.FURY:

                destination = _GameManager.player.position;
                agent.destination = destination;

                break;

            case enemyState.PATROL:

                break;
        }
    }


    void ChangeState(enemyState newState)
    {
        StopAllCoroutines();
        state = newState;

        print(newState);

        switch (state)
        {
            case enemyState.IDLE:

                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                
                StartCoroutine("IDLE");

                break;

            case enemyState.ALERT:

                break;

            case enemyState.PATROL:

                agent.stoppingDistance = 0;
                idWayPoint = Random.Range(0, _GameManager.slimeWayPoints.Length);
                destination = _GameManager.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;

                StartCoroutine("PATROL");

                break;

            case enemyState.FURY:

                destination = transform.position;
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                agent.destination = destination;

                break;
        }

    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
        StayStill(50);
    }

    IEnumerator PATROL ()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        StayStill(30);
    }

    int Rand()
    {
        rand = Random.Range(0, 100);
        return rand;
    }

    void StayStill(int yes)
    {
        if (Rand() <= yes)
        {
            ChangeState(enemyState.IDLE);
        }
        else
        {
            ChangeState(enemyState.PATROL);
        }
    }
    #endregion
}
