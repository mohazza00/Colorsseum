using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private enum State
    {
        MOVEMENT,
        DASHING,
    }

    private State currentState;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer sprite;
    public Transform attackPoint;

    [Header("Dependencies")]
    public Unit unit;
    public Inventory inventory;
    public DamageDetector damageDetector;
    public Shooting shooting;
    public GameManager gameManager;

    [Header("Movement")]
    public Vector2 movement;
    public float movementSpeed;
    public float dashSpeed;
    public Vector2 playerForwardDir;
    public bool canDoubleDash;
    private float currentDashSpeed;
    private bool doubleDashed;

    public bool gameOver;
    public bool isBuyingItems;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        unit = GetComponent<Unit>();
        inventory = GetComponent<Inventory>();
        damageDetector = GetComponent<DamageDetector>();
        sprite = GetComponent<SpriteRenderer>();
        shooting = GetComponent<Shooting>();

        unit.sprite = sprite;
    }

    private void Start()
    {
        currentState = State.MOVEMENT;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (gameManager.gameState == GameState.SPANWING_WAVE || gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            movement = Vector3.zero;
            animator.SetFloat("Speed", 0);
            rb.velocity = Vector2.zero;
            currentState = State.MOVEMENT;
            return;
        }         

        if(gameOver)
        {
            movement = Vector3.zero;
            animator.SetFloat("Speed", 0);
            if (Input.GetKeyDown(KeyCode.R))
            {
                gameManager.RestartGame();
                SceneManager.LoadScene(5);
                gameOver = false;               
            }
        }
        else
        {
            switch (currentState)
            {
                case State.MOVEMENT:
                    movement.x = Input.GetAxisRaw("Horizontal");
                    movement.y = Input.GetAxisRaw("Vertical");

                    animator.SetFloat("Horizontal", movement.x);
                    animator.SetFloat("Vertical", movement.y);
                    animator.SetFloat("Speed", movement.sqrMagnitude);

                    if(movement.x == 1 || movement.x == -1 || movement.y == 1 || movement.y == -1)
                    {
                        animator.SetFloat("LastHorizontal", movement.x);
                        animator.SetFloat("LastVertical", movement.y);
                        playerForwardDir = movement;

                    }


                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        if (movement.sqrMagnitude > 0)
                        {
                            AudioManager.Instance.PlaySound(SoundName.DASH);

                            currentDashSpeed = dashSpeed;
                            currentState = State.DASHING;
                            damageDetector.invincible = true;
                        }                    
                    }
                    break;

                case State.DASHING:
                    if(!doubleDashed)
                    {
                        if (canDoubleDash)
                        {
                            if (Input.GetKeyDown(KeyCode.L))
                            {
                                AudioManager.Instance.PlaySound(SoundName.DASH);
                                currentDashSpeed = dashSpeed * 2;
                                doubleDashed = true;
                            }
                        }
                    }
                   
                    float dashSpeedDropMultiplier = 5f;
                    currentDashSpeed -= currentDashSpeed * dashSpeedDropMultiplier * Time.deltaTime;

                    float minimumDashSpeed = 100f;
                    if (currentDashSpeed < minimumDashSpeed)
                    {
                        currentState = State.MOVEMENT;
                        damageDetector.invincible = false;
                        doubleDashed = false;
                    }
                    break;
            }
        }     
       
    }

    private void FixedUpdate()
    {
        switch(currentState)
        {
            case State.MOVEMENT:
                rb.velocity = movement.normalized * movementSpeed * Time.deltaTime;
                break;

            case State.DASHING:
                rb.velocity = playerForwardDir.normalized * currentDashSpeed * Time.deltaTime;
                break;
        }
    }
}
