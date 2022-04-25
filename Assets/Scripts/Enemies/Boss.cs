using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    [Header("Settings")]
    public RuntimeAnimatorController[] animators;
    public Image healthBar;
    public float changeColorTimer;
    public GameObject shield;

    [Header("Variables")]
    public int currentPhase = 0;
    public LayerMask bulletsLayer;
    public bool rebounceBullet = false;
    public bool shieldIsActive = false;
    private BoxCollider2D shieldCollider;
    private float shieldActiveTimer = 4f;

    protected new void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        unit = GetComponent<Unit>();
        boxCollider = GetComponent<BoxCollider2D>();
        SelectRandomColor();
    }


    private new void Start()
    {
        gameManager = GameManager.Instance;
        StartCoroutine(PatrolRoutine());

        currentState = EnemyState.ENTERING_STAGE;
        target = FindObjectOfType<Player>().transform;

        attackCoolDown = attackMaxCoolDown;
        currentHealth = maxHealth;
        changeColorTimer = Random.Range(3f, 7f);
        animator.runtimeAnimatorController = animators[0];
    }

    protected override void Update()
    {
        base.Update();

        if (frozen)
            return;

        
        if (gameManager.gameState == GameState.SPANWING_WAVE || gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            if(currentPhase == 1)
                shieldCollider.isTrigger = true;

            EnterStage();
        }
        else
        {
            ChangeColor();

            if (currentPhase == 0)
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
                }
            }
            else if(currentPhase == 1)
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
                }
            }

            else if (currentPhase == 2)
            {
                switch (currentState)
                {
                    case EnemyState.CHASING_PLAYER:
                        ChasePlayer();
                        break;

                    case EnemyState.ATTACKING:
                        Debug.Log("Enemy forward " + forwardDirection);
                        Shoot();
                        break;

                    case EnemyState.READY_TO_ATTACK:
                        ReadyToShoot();
                        break;
                }                 
            }
        }
    }

    private void ChangeColor()
    {
        if (changeColorTimer <= 0f)
        {
            SelectRandomColor();
            changeColorTimer = Random.Range(3f, 7f);
        }
        else
        {
            changeColorTimer -= Time.deltaTime;
        }
    }


    private void SelectRandomColor()
    {
        int random = Random.Range(0, 4);

        if (random == 0)
            unit.ChangeColor(UnitColor.GREEN);
        if (random == 1)
            unit.ChangeColor(UnitColor.YELLOW);
        if (random == 2)
            unit.ChangeColor(UnitColor.BLUE);
        if (random == 3)
            unit.ChangeColor(UnitColor.RED);

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
            GameObject bullet = PoolingManager.Instance.GetObject(PoolObjectType.ENEMY_ARROW);
            bullet.transform.position = transform.position;
            Vector3 bulletDirection = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90f;
            Debug.Log(angle);
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            bullet.SetActive(true);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bulletDirection * bulletForce, ForceMode2D.Impulse);
            canAttack = false;
            currentState = EnemyState.READY_TO_ATTACK;
        }
    }

    public void BossLevelUp()
    {
        enemyLevel += 1;
        bulletForce += 2f;
        movementSpeed += 1;     
    }

    public void BossTakeDamage(int damageAmount)
    {
        if (gameManager.gameState == GameState.SPANWING_WAVE || gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
            return;

        currentHealth -= damageAmount;
        PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_EFFECT, transform.position, null);
        AudioManager.Instance.PlaySound(SoundName.HIT);

        UpdateHealthUI();

        if (currentHealth == 10)
        {
            CallMinions(1);
        }
        if (currentHealth == 5)
        {
            CallMinions(2);
        }
        if (currentHealth <= 0)
        {
            BossDie();
        }
    }

    private void BossDie()
    {
        if (gameManager.enemiesOnStage.Contains(gameObject))
        {
            gameManager.enemiesOnStage.Remove(gameObject);
        }
        gameManager.bossDefeated = true;
        healthBar.fillAmount = 0f;
        Destroy(gameObject);
    }

    public void CallMinions(int number)
    {
        if (gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
            return;

        StartCoroutine(StartNextPhaseRoutine(number));              
    }

    IEnumerator StartNextPhaseRoutine(int number)
    {
        foreach (GameObject obj in gameManager.bossMinionsOnStage)
        {
            if (!gameManager.enemiesOnStage.Contains(obj))
            {
                gameManager.enemiesOnStage.Add(obj);
            }
        }

        gameManager.bossMinionsOnStage.Clear();


        if (number == 1)
        {
            bulletForce = 12f;
            damageAmount = 1;
            movementSpeed = 4f;
            currentPhase = 1;
            attackRange = 7;
            gameManager.wavesSpawner.nextMinionWave = 0;
            gameManager.wavesSpawner.StartMinionsWave();
            animator.runtimeAnimatorController = animators[1];
            unit.sprite = renderer.GetComponent<SpriteRenderer>();
            shield.SetActive(true);
            shieldCollider = shield.GetComponent<BoxCollider2D>();
        }
        if (number == 2)
        {
           
            damageAmount = 1;
            bulletForce = 10f;
            movementSpeed = 4f;
            shieldCollider.isTrigger = true;
            shield.SetActive(false);
            attackRange = 7f;
            currentPhase = 2;
            gameManager.wavesSpawner.nextMinionWave = 1;
            gameManager.wavesSpawner.StartMinionsWave();
            animator.runtimeAnimatorController = animators[2];
            unit.sprite = renderer.GetComponent<SpriteRenderer>();
        }

        yield break;
    }

    public void PutDownShield()
    {
        shieldIsActive = true;

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
            GetCurrentDirection(target.position - transform.position);
            SetAnimator();
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

    public void UpdateHealthUI()
    {
        if (currentHealth == 15)
        {
            healthBar.fillAmount = 1f;
        }
        else if(currentHealth == 14)
        {
            healthBar.fillAmount = 0.84f;
        }
        else if (currentHealth == 13)
        {
            healthBar.fillAmount = 0.8f;
        }
        else if (currentHealth == 12)
        {
            healthBar.fillAmount = 0.73f;
        }
        else if (currentHealth == 11)
        {
            healthBar.fillAmount = 0.65f;
        }
        else if (currentHealth == 10)
        {
            healthBar.fillAmount = 0.62f;
        }
        else if (currentHealth == 9)
        {
            healthBar.fillAmount = 0.57f;
        }
        else if (currentHealth == 8)
        {
            healthBar.fillAmount = 0.52f;
        }
        else if (currentHealth == 7)
        {
            healthBar.fillAmount = 0.47f;
        }
        else if (currentHealth == 6)
        {
            healthBar.fillAmount = 0.42f;
        }
        else if (currentHealth == 5)
        {
            healthBar.fillAmount = 0.38f;
        }
        else if (currentHealth == 4)
        {
            healthBar.fillAmount = 0.32f;
        }
        else if (currentHealth == 3)
        {
            healthBar.fillAmount = 0.25f;
        }
        else if (currentHealth == 2)
        {
            healthBar.fillAmount = 0.2f;
        }
        else if (currentHealth == 1)
        {
            healthBar.fillAmount = 0.15f;
        }
        else if (currentHealth == 0)
        {
            healthBar.fillAmount = 0f;
        }
    }
}
