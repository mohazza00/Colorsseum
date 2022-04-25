using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    SWORDMAN,
    ARCHER,
    SHIELD,
    TUTORIAL,
    BOSS,
}

public enum EnemyState
{
    ENTERING_STAGE,
    CHASING_PLAYER,
    READY_TO_ATTACK,
    ATTACKING,
    KNOCKED_BACK,
}

public enum SpawningGate
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject renderer;
    public GameObject outline;
    public Unit unit;
    public SpriteRenderer sprite;
    public BoxCollider2D boxCollider;

    [Header("Movement")]
    public float movementSpeed;
    public Vector3 startingPosition;
    public Vector3 currentDirection;
    public Vector3 forwardDirection;
    public Vector3 movement;
    public float attackRange = 3f;
    public LayerMask waterLayer;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    [Header("Attack")]
    public int damageAmount;
    public bool canAttack;
    public float attackMaxCoolDown = 1f;
    public float attackCoolDown;
    public int enemyLevel = 1;
    public LayerMask playerLayer;
    public Vector2 hitbox;
    public Transform attackPoint;
    public float bulletForce;

    [Header("Knockback")]
    public bool knockback;
    public float knockbackTime;
    public float knockbackCounter;
    private bool checkForWater;
    private bool fellInWater;

    [Header("Stun")]
    public float stunTime;
    private float stunCounter;

    [Header("Other Variables")]
    protected EnemyState currentState;
    protected bool frozen;
    protected Transform target;
    protected GameManager gameManager;
    protected bool startPatrolling;
    protected float patrolRandomPeriod;
    private SpawningGate spawningGate;
    private bool moveToStartingPoint;
    private bool reachedStartingPoint;
    private bool animationIsPlaying;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        unit = GetComponent<Unit>();
        boxCollider = GetComponent<BoxCollider2D>();
        SelectRandomUnitColor();
    }

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;

        StartCoroutine(PatrolRoutine());
        currentState = EnemyState.ENTERING_STAGE;
        target = FindObjectOfType<Player>().transform;
        currentHealth = maxHealth;

        attackCoolDown = attackMaxCoolDown;
        gameObject.layer = LayerMask.NameToLayer("FlyingEnemy"); //can pass through arena edges
    }

    protected virtual void Update()
    {
        if (gameManager.freezeEnemies)
        {
            frozen = true;
        }
        else
        {
            frozen = false;
        }

        if (frozen)
        {
            animator.speed = 0f;
            movement = Vector3.zero;
        }
        else
        {
            animator.speed = 1f;
            frozen = false;
        }

        if(checkForWater)
        {
            Collider2D water = Physics2D.OverlapBox(renderer.GetComponent<BoxCollider2D>().bounds.center, renderer.GetComponent<BoxCollider2D>().bounds.size, 0f, waterLayer);
            if (water != null)
            {
                fellInWater = true;
                checkForWater = false;
            }
        }
        
    }

    public void InitializeEnemy(Vector3 _startingPosition, SpawningGate _spawningGate)
    {
        startingPosition = _startingPosition;
        spawningGate = _spawningGate;
    }

    protected void SelectRandomUnitColor()
    {
        int random = Random.Range(0, 4);

        if (random == 0) unit.unitColor = UnitColor.GREEN;
        else if (random == 1) unit.unitColor = UnitColor.YELLOW;
        else if(random == 2) unit.unitColor = UnitColor.BLUE;
        else if(random == 3) unit.unitColor = UnitColor.RED;
    }

    protected void EnterStage()
    {
        if (!moveToStartingPoint)
        {

            if (spawningGate == SpawningGate.UP)
            {
                transform.position += new Vector3(0f, -1f).normalized * movementSpeed * Time.deltaTime;
                movement = new Vector3(0f, -1f);
            }

            if (spawningGate == SpawningGate.DOWN)
            {
                transform.position += new Vector3(0f, 1f).normalized * movementSpeed * Time.deltaTime;
                movement = new Vector3(0f, 1f);
            }

            if (spawningGate == SpawningGate.LEFT)
            {
                transform.position += new Vector3(1f, 0f).normalized * movementSpeed * Time.deltaTime;
                movement = new Vector3(1f, 0f);
            }

            if (spawningGate == SpawningGate.RIGHT)
            {
                transform.position += new Vector3(-1f, 0f).normalized * movementSpeed * Time.deltaTime;
                movement = new Vector3(-1f, 0f);
            }
            GetCurrentDirection(movement);
            SetAnimator();
            animator.SetFloat("Speed", movement.sqrMagnitude);

        }
        else
        {
            if (!reachedStartingPoint)
            {
                transform.position += (startingPosition - transform.position).normalized * movementSpeed * Time.deltaTime;
                GetCurrentDirection(startingPosition - transform.position);
                SetAnimator();
                animator.SetFloat("Speed", movement.sqrMagnitude);
            }
            else
            {
                boxCollider.isTrigger = false;
                if (gameManager.gameState == GameState.SPANWING_WAVE)
                {
                    if (!gameManager.enemiesOnStage.Contains(gameObject))
                    {
                        gameManager.enemiesOnStage.Add(gameObject);
                    }
                }

                if (gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
                {
                    if(enemyType != EnemyType.BOSS)
                    {
                        if (!gameManager.enemiesOnStage.Contains(gameObject))
                        {
                            if (!gameManager.bossMinionsOnStage.Contains(gameObject))
                            {
                                gameManager.bossMinionsOnStage.Add(gameObject);
                            }
                        }

                    }
                    
                }

                if(enemyType == EnemyType.BOSS)
                {
                    gameManager.bossHealthBar.SetActive(true);
                    Boss boss = GetComponent<Boss>();
                    
                    if(!gameManager.bossEntered)
                    {
                        AudioManager.Instance.StopSound(SoundName.BGM_1);
                        AudioManager.Instance.PlaySound(SoundName.BGM_3);
                        gameManager.bossEntered = true;
                    }
                    boss.healthBar = GameObject.FindGameObjectWithTag("BossHp").GetComponent<Image>();
                    boss.UpdateHealthUI();

                }
                currentState = EnemyState.CHASING_PLAYER;
                gameObject.layer = LayerMask.NameToLayer("Enemy");


            }

            reachedStartingPoint = Vector2.SqrMagnitude(startingPosition - transform.position) < 0.1f;       

        }

    }

    protected void SetDirection()
    {
        currentDirection = GetRandomDirection();
    }

    protected Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    protected void Patrol()
    {
        transform.position += currentDirection * movementSpeed * Time.deltaTime;
        GetCurrentDirection(currentDirection);

        SetAnimator();
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    protected IEnumerator PatrolRoutine()
    {
        while (true)
        {
            float waitingTime = Random.Range(1f, 2f);
            SetDirection();
            yield return new WaitForSeconds(waitingTime);
        }
    }

    protected void Attack()
    {
        if (Vector3.Distance(transform.position, target.position) > attackRange + 1)
        {
            if(!animationIsPlaying)
            {
                currentState = EnemyState.CHASING_PLAYER;
                canAttack = false;
            }
           
        }

        if (attackCoolDown <= 0)
        {
            canAttack = true;
            attackCoolDown = attackMaxCoolDown;
            animationIsPlaying = false;
        }
        else
        {
            attackCoolDown -= Time.deltaTime;
        }
        if (canAttack)
        {
            if (attackPoint != null)
            {
                animationIsPlaying = true;
                canAttack = false;
                StartCoroutine(_DealDamage());
            }
           
        }
    }

    IEnumerator  _DealDamage()
    {

        AudioManager.Instance.PlaySound(SoundName.SLASH);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, hitbox, 0f, playerLayer);

        foreach (Collider2D player in hits)
        {
            player.transform.GetComponent<DamageDetector>().TakeDamage(damageAmount);
            //transform.localScale = new Vector3(1f, 1f, 1f);
            currentState = EnemyState.CHASING_PLAYER;
            animationIsPlaying = false;

        }
    }  

    public void ChasePlayer()
    {
        if(Vector3.Distance(transform.position, target.position) > attackRange)
        {
            Vector3 dir = target.position - transform.position;
            //Debug.Log(dir);
            GetCurrentDirection(dir);
            SetAnimator();
            animator.SetFloat("Speed", movement.sqrMagnitude);
            transform.position += dir.normalized * movementSpeed * Time.deltaTime;
        }
        else
        {
            currentState = EnemyState.ATTACKING;
            animator.SetFloat("Speed", 0f);
        }
    }

    public void SetAnimator()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        if (movement.x == 1 || movement.x == -1 || movement.y == 1 || movement.y == -1)
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);

            forwardDirection = movement;
        }
    }

    public void GetCurrentDirection(Vector3 dir)
    {
        if(dir.x < 0.5 && dir.x > -0.5f && dir.y < 0.5 && dir.y > -0.5f)
        {
            movement = Vector3.zero;
        }
        if (dir.x < 0.5f && dir.x > -0.5f && dir.y < -0.5f)
        {
            movement = new Vector3(0f, -1f);
        }

        if (dir.x < 0.5f && dir.x > -0.5f && dir.y > 0.5f)
        {
            movement = new Vector3(0f, 1f);
        }

        if (dir.x < -0.5f && dir.y < 0.5f && dir.y > -0.5f)
        {
            movement = new Vector3(-1f, 0f);
        }

        if (dir.x > 0.5f && dir.y < 0.5f && dir.y > -0.5f)
        {
            movement = new Vector3(1f, 0f);
        }

        if (dir.x > 0.5f && dir.y > 0.5f)
        {
            movement = new Vector3(1f, 1f);
        }
        if (dir.x < -0.5f && dir.y < -0.5f)
        {
            movement = new Vector3(-1f, -1f);
        }
            
        if (dir.x > 0.5f && dir.y < -0.5f)
        {
            movement = new Vector3(1f, -1f);
        }
        if (dir.x < -0.5f && dir.y > 0.5f)
        {
            movement = new Vector3(-1f, 1f);
        }      
    }

    public void ReadyToAttack()
    {
        
        if (startPatrolling)
        {
            patrolRandomPeriod = Random.Range(0.2f, 2f);
            startPatrolling = false;
        }

        if(patrolRandomPeriod <= 0f)
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

    public void ReadyToShoot()
    {
        if (startPatrolling)
        {
            patrolRandomPeriod = Random.Range(0.2f, 2f);
            startPatrolling = false;
        }

        if (patrolRandomPeriod <= 0f)
        {
            currentState = EnemyState.ATTACKING;
            startPatrolling = true;

        }
        else
        {
            patrolRandomPeriod -= Time.deltaTime;
            Patrol();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Stage"))
        {
            currentState = EnemyState.CHASING_PLAYER;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (moveToStartingPoint)
            return;

        if(collision.gameObject.layer == LayerMask.NameToLayer("Gate"))
        {
            boxCollider.isTrigger = true;
            moveToStartingPoint = true;
        }
    }


    public void LevelUp()
    {
        enemyLevel += 1;
        maxHealth += 1f;
        currentHealth += 1f;
        damageAmount += 1;
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        bulletForce += 2f;
        if(unit.enemyType == EnemyType.SWORDMAN)
        {
            movementSpeed += 1;
        }
    }

    

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_EFFECT, transform.position, null);
        AudioManager.Instance.PlaySound(SoundName.HIT);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(gameManager.enemiesOnStage.Contains(gameObject))
        {
            gameManager.enemiesOnStage.Remove(gameObject);
        }

        if (!gameManager.bossMinionsOnStage.Contains(gameObject))
        {
            gameManager.bossMinionsOnStage.Remove(gameObject);
        }

        Destroy(gameObject);
    }

    public void AnimationFinished()
    {
        animationIsPlaying = false;
    }

    public void KnockBack(Vector3 player, float knockbackForce)
    {
        gameObject.layer = LayerMask.NameToLayer("FlyingEnemy");
        checkForWater = true;
        knockback = true;
        rb.velocity = Vector2.zero;
        knockbackCounter = knockbackTime;
        currentState = EnemyState.KNOCKED_BACK;
        Vector2 dir = transform.position - player;
        dir = dir.normalized * knockbackForce;
        rb.AddForce(dir, ForceMode2D.Impulse);
        movement = Vector3.zero;
        animator.SetFloat("Speed", 0f);
        SetAnimator();
    }

    public void KnockbackTimer()
    {
        if (!knockback)
            return;

        if(knockbackCounter > 0)
        {
            knockbackCounter -= Time.deltaTime;
        }
        else
        {
            knockback = false;
            knockbackCounter = knockbackTime;
            stunTime = Random.Range(0.5f, 1f);
            stunCounter = stunTime;

            if (fellInWater)
            {
                rb.velocity = rb.velocity * 0.5f;
                StartCoroutine(FallIntoWater());
            }
            else
            {
                rb.velocity = Vector2.zero;
                gameObject.layer = LayerMask.NameToLayer("Enemy");

            }
        }
    }

    private IEnumerator FallIntoWater()
    {
        animator.SetTrigger("Fall");
        yield return new WaitForSeconds(0.3f);
        PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_EFFECT, transform.position, null);
        AudioManager.Instance.PlaySound(SoundName.WATER_BLOOP);
        Die();
        
    }

    public void Stun()
    {
        if(stunCounter > 0)
        {
            stunCounter -= Time.deltaTime;
        }
        else
        {
            currentState = EnemyState.CHASING_PLAYER;
        }
    }
}
