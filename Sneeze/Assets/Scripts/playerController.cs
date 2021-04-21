using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class playerController : MonoBehaviour
{
    private Rigidbody2D body;
    private playerActionControl playerActionControl;

    #region moving
    private float moveInput;
    private bool isFacingRight = true;
    [SerializeField] float speed;

    #endregion

    #region Checking

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    #endregion

    #region jumping
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float limitFallSpeed;
    private bool isJumping;
    private float jumpTimeCounter;
    private float jumpTime = 0.15f;

    #endregion


    void Awake()
    {
        playerActionControl = new playerActionControl();
    }

    private void OnEnable()
    {
        playerActionControl.Enable();
    }

    private void OnDisable()
    {
        playerActionControl.Disable();
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        playerActionControl.Player.Move.started += _ => StartMove();
        playerActionControl.Player.Move.performed += _ => PerformMove();
        playerActionControl.Player.Move.canceled += _ => EndMove();

        playerActionControl.Player.Jump.started += _ => StartJump();
        playerActionControl.Player.Jump.canceled += _ => EndJump();
    }

    void Update()
    {
        //perform jump
        PerformJump();

        //control gravity
        jumpGravityControl();
    }

    void FixedUpdate()
    {
        //move
        body.velocity = new Vector2(moveInput * speed, body.velocity.y);

        //ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }
    private void StartMove()
    {

    }
    private void PerformMove()
    {
        moveInput = playerActionControl.Player.Move.ReadValue<float>();

        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            Flip();
        }
    }
    private void EndMove()
    {

    }
    private void StartJump()
    {
        if (isGrounded)
        {
            isJumping = true;
            body.velocity = Vector2.up * jumpForce;
        }
    }
    private void PerformJump()
    {

    }
    private void EndJump()
    {
        isJumping = false;
    }
    private void jumpGravityControl()
    {
        if (body.velocity.y < 0 && body.velocity.y > limitFallSpeed)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (body.velocity.y > 0 && !isJumping)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    private void Flip()
    {
        if (isFacingRight)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        isFacingRight = !isFacingRight;
    }

}
