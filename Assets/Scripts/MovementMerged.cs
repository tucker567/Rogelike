using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        WallSliding
    }
    private MovementState state;

    private Animator anim;

    public ParticleSystem Dust;

    // Movement parameters
    private float moveSpeed => PlayerStatsEffects.Instance.finalMoveSpeed; 
    private float jumpHeight => PlayerStatsEffects.Instance.finalJumpHeight;

    private float horizontal;
    private bool downPressed; // DOWNWARD CONTROL
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
    private float wallJumpingPowerX => PlayerStatsEffects.Instance.finalWallJumpingPower.x;
    private float wallJumpingPowerY => PlayerStatsEffects.Instance.finalWallJumpingPower.y;
    private int wallJumpCount = 0;
    private float maxWallJumps => PlayerStatsEffects.Instance.finalMaxWallJumps;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator not found on player!");
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        downPressed = Input.GetKey(KeyCode.S); // DOWNWARD CONTROL

        if (IsGrounded())
        {
            wallJumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Jumping!");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            CreateDust();
        }

        if (wallCheck != null)
        {
            WallSlide();
            WallJump();
        }

        DirectionFlip();

        rb.gravityScale = PlayerStatsEffects.Instance.finnalGravityScale;

        UpdateState();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (PlayerStatsEffects.Instance == null)
        {
            Debug.LogError("PlayerStatsEffects.Instance is null. Ensure it is properly initialized in the scene.");
            return;
        }

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

            // DOWNWARD CONTROL: Accelerate down when S is held and not grounded
            if (downPressed && !IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - 1.5f);
            }
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
            float jumpX = wallJumpingPowerX;
            float jumpY = wallJumpingPowerY;
            rb.linearVelocity = new Vector2(jumpX * wallJumpingDirection, jumpY);
            wallJumpingCounter = 0f;
            wallJumpCount++;
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

    void UpdateState()
    {
        if (isWallSliding)
        {
            state = MovementState.WallSliding;
        }
        else if (!IsGrounded())
        {
            if (rb.linearVelocity.y > 0.1f)
                state = MovementState.Jumping;
            else
                state = MovementState.Falling;
        }
        else if (Mathf.Abs(horizontal) > 0.1f)
        {
            state = MovementState.Running;
        }
        else
        {
            state = MovementState.Idle;
        }
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        anim.SetInteger("movementState", (int)state);
    }
}
