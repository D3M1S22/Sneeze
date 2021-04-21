using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class playerController : MonoBehaviour
{
    private Rigidbody2D body;

    #region moving

    private float moveInput;
    private bool isFacingRight = true;
    [SerializeField] float speed;

    #endregion

    #region jumping

    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpforce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private bool isJumping;
    private bool canDoubleJump;
    [SerializeField] private float jumpTime;
    private float jumpTimeCounter;
    private bool jumpButtonPressed;

    #endregion

    #region wallJumping

    [SerializeField] private float xWallJumpForce;
    [SerializeField] private float yWallJumpForce;
    private bool isWallJumping;
    private bool wallJumpingPressed;
    [SerializeField] private float wallJumpTime;
    private float tempInput;

    #endregion

    #region wallSliding

    [SerializeField] private Transform frontCheck;
    [SerializeField] private float frontCheckRadius;
    private bool isTouchingFront;
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed;

    #endregion






    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //higher jump perform
        HigherJump();

        //WallSliding perform
        WallSlidingPerform();

        //wall jump perform
        WallJumpPerform();
    }

    void FixedUpdate()
    {
        //move
        if (!isWallJumping)
        {
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        }

        //isGrounded check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        //isTouchingFront check
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, frontCheckRadius, groundMask);

    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;

        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            Flip();
        }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (context.started && isWallSliding)
        {

            wallJumpingPressed = true;
        }

        if (context.canceled)
        {
            wallJumpingPressed = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isWallSliding)
        {
            isJumping = true;
            canDoubleJump = true;
            jumpTimeCounter = jumpTime;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        if (context.performed && !isGrounded && canDoubleJump && !isWallSliding)
        {
            canDoubleJump = false;
            body.velocity = Vector2.up * doubleJumpforce;
        }

        if (context.canceled)
        {
            isJumping = false;
        }
    }

    private void HigherJump()
    {
        if (isJumping && canDoubleJump)
        {
            if (jumpTimeCounter > 0)
            {
                body.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    private void WallSlidingPerform()
    {
        if (isTouchingFront && moveInput != 0 && !isGrounded && !isJumping)
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

    private void WallJumpPerform()
    {
        if (isWallSliding && wallJumpingPressed)
        {
            wallJumpingPressed = false;
            isWallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }

        if (isWallJumping)
        {
            body.velocity = new Vector2(xWallJumpForce * -tempInput, yWallJumpForce);
        }
    }

    private void SetWallJumpingToFalse()
    {
        canDoubleJump = true;
        isWallJumping = false;
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
