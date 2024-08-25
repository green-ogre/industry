using UnityEngine;
using UnityEngine.InputSystem;

public class SlideController : MonoBehaviour
{

    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private float HeldJumpForce = 40f;
    [SerializeField] private float MaximumFallSpeed = 20f;
    [SerializeField] private float MaximumJumpTime = 0.2f;
    [SerializeField] private float MovementSpeed = 5f;

    public Rigidbody2D.SlideMovement SlideMovement = new Rigidbody2D.SlideMovement();
    private Rigidbody2D.SlideResults slideResults;
    private Rigidbody2D Rigidbody;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool jumpReleased = true;
    private float jumpHeld = 0;

    public Vector2 lastDirection;
    private bool jumpEnabled = true;
    private bool slideEnabled = true;

    private InputAction moveAction;
    private InputAction jumpAction;

    public void SetJumpEnabled(bool enabled)
    {
        // clear state when disabling
        if (jumpEnabled && !enabled)
        {
            jumpReleased = true;
            jumpHeld = 0;
        }

        jumpEnabled = enabled;
    }

    public void SetSlideEnabled(bool enabled)
    {
        slideEnabled = enabled;
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     // we'll assume a single contact point
    //     if (collision.contactCount == 0)
    //     {
    //         return;
    //     }

    //     var contact = collision.GetContact(0);

    //     Debug.Log(contact.normal);
    // }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void FixedUpdate()
    {
        if (jumpEnabled)
        {
            bool justJumped = UpdateJump(Time.fixedDeltaTime);
            if (justJumped)
            {
                Rigidbody.AddForceY(JumpForce, ForceMode2D.Impulse);
            }
            else if (!jumpReleased && jumpHeld < MaximumJumpTime)
            {
                Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, Rigidbody.linearVelocityY + HeldJumpForce * Time.fixedDeltaTime);
            }
        }

        if (slideEnabled)
        {
            var velocity = CalculateVelocity();

            if (Rigidbody.linearVelocityY < -MaximumFallSpeed)
            {
                Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, -MaximumFallSpeed);
            }

            slideResults = Rigidbody.Slide(velocity, Time.deltaTime, SlideMovement);
        }
    }

    private Vector2 CalculateVelocity()
    {
        float horz = AxisNormalize.Movement(moveAction.ReadValue<Vector2>().x) * MovementSpeed;
        if (horz > 0 && !facingRight)
        {
            lastDirection = new Vector2(horz, 0);
            Flip();
        }
        else if (horz < 0 && facingRight)
        {
            lastDirection = new Vector2(horz, 0);
            Flip();
        }

        return new Vector2(horz, 0f);
    }

    private bool UpdateJump(float delta)
    {
        if (!jumpEnabled)
        {
            return false;
        }

        bool jump = jumpAction.ReadValue<float>() > 0;

        if (!jumpReleased && !jump)
        {
            jumpReleased = true;
        }

        if (!jumpReleased)
        {
            jumpHeld += delta;
        }
        else
        {
            jumpHeld = 0;
        }

        bool justPressed = false;
        if (slideResults.surfaceHit && jump && jumpReleased)
        {
            jumpReleased = false;
            justPressed = true;
        }

        return justPressed;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
