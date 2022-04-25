using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Enemy
{
    [Header("Shield")]
    public GameObject shield;
    public bool rebounceBullet = false;
    public bool shieldIsActive = false;
    private BoxCollider2D shieldCollider;
    private float shieldActiveTimer = 4f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        shieldCollider = shield.GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (frozen && currentState != EnemyState.KNOCKED_BACK)
            return;
        
        if (gameManager.gameState == GameState.SPANWING_WAVE || gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            shieldCollider.isTrigger = true;
            EnterStage();
        }
        else
        {
            shieldCollider.isTrigger = false;

            switch (currentState)
            {
                case EnemyState.CHASING_PLAYER:
                    ChasePlayer();
                    break;

                case EnemyState.ATTACKING:
                    movement = Vector3.zero;
                    PutDownShield();
                    break;

                case EnemyState.READY_TO_ATTACK:
                    shieldIsActive = false;
                    TakeOffShield();
                    break;

                case EnemyState.KNOCKED_BACK:
                    if (knockback)
                    {
                        KnockbackTimer();
                    }
                    else
                    {
                        Stun();
                    }
                    break;
            }
        }
    }

    public void PutDownShield()
    {
        if (rebounceBullet)
        {
            currentState = EnemyState.READY_TO_ATTACK;
            shieldCollider.isTrigger = true;
            rebounceBullet = false;
            shieldActiveTimer = 4f;
            return;
        }
       
        if (shieldActiveTimer <= 0f)
        {
            currentState = EnemyState.READY_TO_ATTACK;
            shieldCollider.isTrigger = true;
            rebounceBullet = false;
            shieldActiveTimer = 5f;
        }
        else
        {
            shieldActiveTimer -= Time.deltaTime;
            if(!shieldIsActive)
            {
                GetCurrentDirection(target.position - transform.position);
                SetAnimator();
                shieldIsActive = true;
            }         
        }              
    }

    public void TakeOffShield()
    {
        if (startPatrolling)
        {
            patrolRandomPeriod = Random.Range(1.5f, 3f);
            startPatrolling = false;
        }

        if (patrolRandomPeriod <= 0f)
        {
            currentState = EnemyState.CHASING_PLAYER;
            startPatrolling = true;

        }
        else
        {
            patrolRandomPeriod -= Time.deltaTime;
            Patrol();
        }       
    }
}
