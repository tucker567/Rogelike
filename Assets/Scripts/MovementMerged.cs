using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement parameters
    [SerializeField] private float speed = 8f;           // Horizontal movement speed
    [SerializeField] private float jumpPower = 16f;        // Jump vertical power
    private float horizontal;
    private bool isFacingRight = true;

    // Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    // Wall movement parameters (optional, requires wall references)
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;
    
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private void Update()
    {
        // Get horizontal input
        horizontal = Input.GetAxisRaw("Horizontal");

        // Jumping: if jump pressed and on the ground, then jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
        // Cut jump short if jump button is released while moving upward
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Handle wall sliding and wall jumping if wall references are set
        if (wallCheck != null)
        {
            WallSlide();
            WallJump();
        }

        // Flip the character based on horizontal input
        DirectionFlip();
    }

    private void FixedUpdate()
    {
        // Only allow horizontal movement normally if not wall jumping
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    // Check if the player is on the ground
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // Check if the player is touching a wall
    private bool IsWalled()
    {
        return wallCheck != null && Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    // Wall sliding functionality
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

    // Wall jumping functionality (flipping removed)
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            // Determine jump direction based on the relative positions of the player and the wallCheck
            wallJumpingDirection = Mathf.Sign(transform.position.x - wallCheck.position.x);
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            // Removed flipping logic during wall jump; character's facing direction remains unchanged

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // Automatically flip the character based on horizontal input
    private void DirectionFlip()
    {
        // Flip if the input direction is opposite to the current facing direction
        if (horizontal < 0 && isFacingRight)
        {
            Flip();
        }
        else if (horizontal > 0 && !isFacingRight)
        {
            Flip();
        }
    }

    // Flip the character’s facing direction by modifying the parent transform's local scale.
    // This method ensures all child objects flip along with the parent.
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
