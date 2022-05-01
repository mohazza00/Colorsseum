using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour
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
    public Inventory inventory;

    [Header("Movement")]
    public Vector2 movement;
    public float movementSpeed;
    public float dashSpeed;
    private float currentDashSpeed;
    public Vector2 playerForwardDir;
  
    [Header("Tutorial")]
    public TutorialManager tutorialManager;

    [Header("Input Checking")]
    private bool up;
    private bool down;
    private bool right;
    private bool left;
    private bool dash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        currentState = State.MOVEMENT;
    }

    private void Update()
    {
        if (!tutorialManager.checkingMovement)
            return;
     
        switch (currentState)
        {
            case State.MOVEMENT:

                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                animator.SetFloat("Horizontal", movement.x);
                animator.SetFloat("Vertical", movement.y);
                animator.SetFloat("Speed", movement.sqrMagnitude);

                if (movement.x == 1 || movement.x == -1 || movement.y == 1 || movement.y == -1)
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
                    }
                }
                break;

            case State.DASHING:
                
                float dashSpeedDropMultiplier = 5f;
                currentDashSpeed -= currentDashSpeed * dashSpeedDropMultiplier * Time.deltaTime;

                float minimumDashSpeed = 100f;
                if (currentDashSpeed < minimumDashSpeed)
                {
                    currentState = State.MOVEMENT;
                    dash = true;
                }
                break;
        }

        CheckInputs();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.MOVEMENT:
                rb.velocity = movement.normalized * movementSpeed * Time.deltaTime;
                break;

            case State.DASHING:
                rb.velocity = movement.normalized * currentDashSpeed * Time.deltaTime;
                break;
        }
    }

    public void CheckInputs()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            up = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            left = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            down = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            right = true;
        }

        if(tutorialManager.currentSlide == 4)
        {
            if (up && down && left && right && dash)
            {
                tutorialManager.FinishMovementCheck();
                ResetPlayerTransform();
            }
        }
    }

    public void ResetPlayerTransform()
    {
        transform.position = Vector3.zero;
        movement = Vector2.zero;
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetFloat("LastHorizontal", 0);
        animator.SetFloat("LastVertical", -1);
    }
}
