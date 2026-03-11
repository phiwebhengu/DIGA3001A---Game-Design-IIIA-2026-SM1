using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 5f;
    // FIXED: Removed 'float' from before LayerMask
    public LayerMask groundLayer;

    [Header("Detection")]
    public float groundCheckDistance = 1.1f;
    public float obstacleCheckDistance = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground Check
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (player == null) return;

        // Determine direction to player (1 for right, -1 for left)
        float direction = math.sign(player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 << player.gameObject.layer);
        // Horizontal Movement
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

            // 1. Jump if there is a wall in front
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), obstacleCheckDistance, groundLayer);

            // 2. Jump if there is a gap ahead
            Vector3 gapCheckPos = transform.position + new Vector3(direction * 0.5f, 0, 0);
            RaycastHit2D gapAhead = Physics2D.Raycast(gapCheckPos, Vector2.down, groundCheckDistance, groundLayer);

            // 3. Jump if player is above and there is a platform to catch
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundLayer);

            if (groundInFront.collider != null || gapAhead.collider == null || (isPlayerAbove && platformAbove.collider != null))
            {
                shouldJump = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            // Apply upward impulse for the jump
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
