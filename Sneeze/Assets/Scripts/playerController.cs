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
    [SerializeField] private Transform frontCheck;
    [SerializeField] private float frontCheckRadius;
    private bool isTouchingFront;

    #endregion

    #region jumping
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float limitFallSpeed;
    private bool isJumping;
    private bool canDoubleJump;
    private float jumpTimeCounter;
    private float jumpTime = 0.15f;

    #endregion

    #region wallSliding
    [SerializeField] private float wallSlidingSpeed;
    private bool isWallSliding;

    #endregion

    #region wallJumping
    private bool isWallJumping;
    [SerializeField] private float xWallForce;
    [SerializeField] private float yWallForce;
    [SerializeField] private float wallJumpTime;

    #endregion

    private float tempInput;

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

        playerActionControl.Player.Move.performed += _ => PerformMove();

        playerActionControl.Player.Jump.started += _ => StartJump();
        playerActionControl.Player.Jump.canceled += _ => EndJump();

        playerActionControl.Player.WallJump.started += _ => StartWallJump();
        playerActionControl.Player.WallJump.canceled += _ => EndWallJump();
    }

    void Update()
    {
        //control gravity
        jumpGravityControl();

        //wallSLide perform
        WallSliding();

        //wallJump perform
        WallJumpPerform();
    }

    void FixedUpdate()
    {
        //move
        if (!isWallJumping)
        {
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        }

        //ground check
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        //front check
        //isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, frontCheckRadius, groundMask);
    }

    private void PerformMove()
    {
        moveInput = playerActionControl.Player.Move.ReadValue<float>();

        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            Flip();
        }
    }
    private void StartJump()
    {
        if (isGrounded)
        {
            isJumping = true;
            canDoubleJump = true;
            body.velocity = Vector2.up * jumpForce;
        }

        if (!isGrounded && canDoubleJump)
        {
            body.velocity = Vector2.up * doubleJumpForce;
            canDoubleJump = false;
        }
    }
    private void EndJump()
    {
        isJumping = false;
    }
    private void StartWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }
    }
    private void WallJumpPerform()
    {
        if (isWallJumping)
        {
            canDoubleJump = true;
            body.velocity = new Vector2(xWallForce * -tempInput, yWallForce);
        }
    }
    private void EndWallJump()
    {
        isWallJumping = false;
    }
    private void SetWallJumpingToFalse()
    {
        isWallJumping = false;
    }
    private void WallSliding()
    {
        if (isTouchingFront && !isGrounded)
        {
            isWallSliding = true;
            canDoubleJump = false;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "wall")
        {
            if (isFacingRight)
            {
                tempInput = 1;
            }
            else
            {
                tempInput = -1;
            }

            isTouchingFront = true;
        }

        if (coll.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "wall")
        {
            isTouchingFront = false;
        }
        if (coll.gameObject.tag == "ground")
        {
            isGrounded = false;
        }
    }
    private void jumpGravityControl()
    {
        if (body.velocity.y < 0 && body.velocity.y > limitFallSpeed)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (body.velocity.y > 0 && !isJumping && !isWallJumping)
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
