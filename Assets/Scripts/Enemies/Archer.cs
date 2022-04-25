using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    public GameObject bulletPrefab;

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

        if (frozen)
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
                    Shoot();
                    break;

                case EnemyState.READY_TO_ATTACK:
                    ReadyToShoot();
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

    private void Shoot()
    {
        if (attackCoolDown <= 0)
        {
            canAttack = true;
            attackCoolDown = 1f;
        }
        else
        {
            attackCoolDown -= Time.deltaTime;
        }
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(_Shoot());
        }
    }

    IEnumerator _Shoot()
    {
        GameObject bullet = PoolingManager.Instance.GetObject(PoolObjectType.ENEMY_ARROW);
        bullet.transform.position = transform.position;
        Vector3 bulletDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90f;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (bullet != null)
        {
            GetCurrentDirection(bulletDirection);
            SetAnimator();
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.25f);
            AudioManager.Instance.PlaySound(SoundName.SHOOT);
            
            bullet.SetActive(true);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bulletDirection * bulletForce, ForceMode2D.Impulse);
            canAttack = false;
            yield return new WaitForSeconds(0.3f);
            currentState = EnemyState.READY_TO_ATTACK;
        }
    } 
}
