using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordman : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (frozen && currentState != EnemyState.KNOCKED_BACK)
            return;

        if (gameManager.gameState == GameState.SPANWING_WAVE || gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            EnterStage();
        }
        else
        {
            switch (currentState)
            {

                case EnemyState.CHASING_PLAYER:
                    ChasePlayer();
                    break;

                case EnemyState.ATTACKING:
                    movement = Vector3.zero;
                    Attack();
                    break;

                case EnemyState.READY_TO_ATTACK:
                    ReadyToAttack();
                    break;

                case EnemyState.KNOCKED_BACK:
                    if(knockback)
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
}
