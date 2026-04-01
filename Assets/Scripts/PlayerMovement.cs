using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

/// <summary>
/// PlayerMovement handles 2D platformer character movement including:
/// - Horizontal movement with configurable speed
/// - Multi-jump system (double jump)
/// - Variable jump height (hold jump for higher jumps)
/// - Enhanced gravity with faster falling
/// - Ground detection with physics overlap
/// - Automatic sprite flipping based on movement direction
/// </summary>
public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody2D rb;

        [Header("Movement")]
        public float moveSpeed = 5f;
        private float horizontalMovement;
        private bool isFacingRight = true;

        [Header("Dashing")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.1f;
        public float dashCooldown = 0.1f;
        bool isDashing;
        bool canDash = true;
        TrailRenderer trailRenderer;

    void Start()
    {
        jumpsRemaining = maxJumps;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;
    bool wasGrounded;

        [Header("Ground Check")]
        public Transform groundCheckPos;
        public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
        public LayerMask groundLayer;
        bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = -18f;
    public float fallMultiplier = 2f;

     [Header("Wall Check")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header ("Wall Movement")]

        public float wallSlideSpeed = 2f;
        bool isWallSliding;
        bool isWallJumping;
        float wallJumpDirection;
        float wallJumpTime = 0.5f;
        float wallJumpTimer;
        public Vector2 wallJumpPower = new Vector2(5f, 10f);

    void Update()
    {
        if(isDashing)
        {
            return;
        }
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
       

        if(!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
             Flip();
        }
        
    }

    public void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    /// <summary>
    /// Handles horizontal movement input from the Input System.
    /// </summary>
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        float dashDirection = isFacingRight ? 1 : -1;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true
;
    }

    /// <summary>
    /// Handles jump input with variable jump height and multi-jump support.
    /// </summary>
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;

            if(transform.localScale.x != wallJumpDirection)
            {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void GroundCheck()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
        
        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;  
        }
        
        wasGrounded = isGrounded;
    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);
    }

    /// <summary>
    /// Processes enhanced gravity with faster falling and terminal velocity.
    /// </summary>
    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if(!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;

        }

    }

    private void ProcessWallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = - transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        {
            isWallJumping = false;
        }
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Draws debug gizmos for ground and wall detection in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
