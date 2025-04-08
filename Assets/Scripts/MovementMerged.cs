using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem Dust;

    // Movement parameters
    private float moveSpeed => PlayerStatsEffects.Instance.finalMoveSpeed; 
    private float jumpHeight => PlayerStatsEffects.Instance.finalJumpHeight;

    private float horizontal;
    private bool isFacingRight = true;

    // Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    // Wall movement parameters
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;
    
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    private float wallJumpingPowerX => PlayerStatsEffects.Instance.finalWallJumpingPower.x; // Horizontal power
    private float wallJumpingPowerY => PlayerStatsEffects.Instance.finalWallJumpingPower.y; // Vertical power
    // Wall jump limit
    private int wallJumpCount = 0;
    private float maxWallJumps => PlayerStatsEffects.Instance.finalMaxWallJumps; // Maximum number of wall jumps

    private void Update()
    {
        // Get horizontal input
        horizontal = Input.GetAxisRaw("Horizontal");

        // Reset wall jump count if player touches the ground
        if (IsGrounded())
        {
            wallJumpCount = 0;
        }

        // Jumping: if jump pressed and on the ground, then jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Jumping!");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            CreateDust();
        }

        // Handle wall sliding and wall jumping
        if (wallCheck != null)
        {
            WallSlide();
            WallJump();
        }

        // Flip the character based on horizontal input
        DirectionFlip();

        // Apply gravity scale
        rb.gravityScale = PlayerStatsEffects.Instance.finnalGravityScale; // Use the final gravity scale from PlayerStatsEffects
    }

    private void FixedUpdate()
    {
        if (PlayerStatsEffects.Instance == null)
        {
            Debug.LogError("PlayerStatsEffects.Instance is null. Ensure it is properly initialized in the scene."); // Added error log
            return; // Prevent further execution
        }

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return wallCheck != null && Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = Mathf.Sign(transform.position.x - wallCheck.position.x);
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f && wallJumpCount < maxWallJumps)
        {
            isWallJumping = true;
            float jumpX = wallJumpingPowerX; // Use the x component for horizontal velocity
            float jumpY = wallJumpingPowerY; // Use the y component for vertical velocity
            rb.linearVelocity = new Vector2(jumpX * wallJumpingDirection, jumpY); // Corrected from linearVelocity to velocity
            wallJumpingCounter = 0f;
            wallJumpCount++; // Increment the wall jump count

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
            CreateDust();
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void DirectionFlip()
    {
        if (horizontal < 0 && isFacingRight)
        {
            Flip();
        }
        else if (horizontal > 0 && !isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    void CreateDust()
    {
        Dust.Play();
    }
}
