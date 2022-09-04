using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navAgent;

    private EnemyStates enemy_State;

    public float walk_Speed = 0.5f;
    public float run_Speed = 4f;

    public float chase_Distance = 7f;
    private float current_Chase_Distance;
    public float attack_Distance = 1.8f;
    public float chase_After_Attack_Distance = 2f;

    public float patrol_Radius_Min = 20f, patrol_Radius_Max = 60f;
    public float patrol_For_This_Time = 15f;
    private float patrol_Timer;
    private Animator enemy_Anim;
    public float wait_Before_Attack = 2f;
    private float attack_Timer;

    private Transform target;

    public GameObject attack_Point;

    private float health = 100f;
    [SerializeField]
    GameObject tigerObject;
    //private EnemyAudio enemy_Audio;

    void Awake()
    {
        enemy_Anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;

        //enemy_Audio = GetComponentInChildren<EnemyAudio>();

    }

    // Use this for initialization
    void Start()
    {

        enemy_State = EnemyStates.PATROL;

        patrol_Timer = patrol_For_This_Time;

        // when the enemy first gets to the player
        // attack right away
        attack_Timer = wait_Before_Attack;

        // memorize the value of chase distance
        // so that we can put it back
        current_Chase_Distance = chase_Distance;

    }

    // Update is called once per frame
    void Update()
    {

        if (enemy_State == EnemyStates.PATROL)
        {
            Patrol();
        }

        if (enemy_State == EnemyStates.CHASE)
        {
            Chase();
        }

        if (enemy_State == EnemyStates.ATTACK)
        {
            Attack();
        }
        Die();
    }

    void Patrol()
    {
        enemy_Anim.SetBool("Walk", true);
        // tell nav agent that he can move
        navAgent.isStopped = false;
        navAgent.speed = walk_Speed;

        // add to the patrol timer
        patrol_Timer += Time.deltaTime;

        if (patrol_Timer > patrol_For_This_Time)
        {
            SetNewRandomDestination();

            patrol_Timer = 0f;
        }

        if (navAgent.velocity.sqrMagnitude > 0)
        {
            enemy_Anim.SetBool("Walk", true);
        }
        else
        {
            enemy_Anim.SetBool("Walk", false);
        }

        // test the distance between the player and the enemy
        if (Vector3.Distance(transform.position, target.position) <= chase_Distance)
        {

            enemy_Anim.SetBool("Walk", false);

            enemy_State = EnemyStates.CHASE;

            // play spotted audio
            //enemy_Audio.Play_ScreamSound();

        }


    } // patrol

    void Chase()
    {

        // enable the agent to move again
        navAgent.isStopped = false;
        navAgent.speed = run_Speed;
        // set the player's position as the destination
        // because we are chasing(running towards) the player
        navAgent.SetDestination(target.position);

        if (navAgent.velocity.sqrMagnitude > 0)
        {

            enemy_Anim.SetBool("Run", true);

        }
        else
        {

            enemy_Anim.SetBool("Run", false);

        }

        // if the distance between enemy and player is less than attack distance
        if (Vector3.Distance(transform.position, target.position) <= attack_Distance)
        {

            // stop the animations
            enemy_Anim.SetBool("Run", false);
            enemy_Anim.SetBool("Walk", false);
            enemy_State = EnemyStates.ATTACK;

            // reset the chase distance to previous
            if (chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance;
            }

        }
        else if (Vector3.Distance(transform.position, target.position) > chase_Distance)
        {
            // player run away from enemy

            // stop running
            enemy_Anim.SetBool("Run", false);

            enemy_State = EnemyStates.PATROL;

            // reset the patrol timer so that the function
            // can calculate the new patrol destination right away
            patrol_Timer = patrol_For_This_Time;

            // reset the chase distance to previous
            if (chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance;
            }


        } // else

    } // chase

    void Attack()
    {

        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = true;
        attack_Timer += Time.deltaTime;

        if (attack_Timer > wait_Before_Attack)
        {

            StartCoroutine(AttackCoroutine());
        }



    } // attack

    IEnumerator AttackCoroutine()
    {
        enemy_Anim.SetTrigger("HeadAttack");

        attack_Timer = 0f;
        yield return new WaitForSeconds(2);
        enemy_Anim.SetTrigger("TailAttack");
        yield return new WaitForSeconds(3);
        enemy_Anim.SetTrigger("HeadAttack");
        if (Vector3.Distance(transform.position, target.position) > 1f)
        {
            // player run away from enemy

            enemy_Anim.SetBool("Run", false);

            enemy_State = EnemyStates.PATROL;

            patrol_Timer = patrol_For_This_Time;

            // reset the chase distance to previous
            if (chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance;
            }

        }
    }

    private void OnCollisionEnter(Collision _target)
    {
        if(_target.collider.gameObject.tag == "Player")
        {
            _target.gameObject.GetComponent<PlayerController>().ReduceHealth(20);
        }
    }

    void SetNewRandomDestination()
    {

        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

        Vector3 randDir = Random.insideUnitSphere * rand_Radius;
        randDir += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);

        navAgent.SetDestination(navHit.position);

    }

    void Turn_On_AttackPoint()
    {
        attack_Point.SetActive(true);
    }

    void Turn_Off_AttackPoint()
    {
        if (attack_Point.activeInHierarchy)
        {
            attack_Point.SetActive(false);
        }
    }

    public EnemyStates Enemy_State
    {
        get; set;
    }
    public void SetHealth(float _damage)
    {
        health -= _damage;
    }
    public void Die()
    {
        if (health <= 0)
        {
            StartCoroutine(DeathAnime());
        }
    }
    IEnumerator DeathAnime()
    {
        enemy_Anim.SetBool("Run", false);
        enemy_Anim.SetBool("Walk", false);
        enemy_Anim.SetTrigger("DeathAnime");
        yield return new WaitForSeconds(2f);
                
        
        gameObject.SetActive(false);
        gameObject.GetComponent<EnemyController>().enabled = false;
        
    }
}
