using NUnit.Framework.Constraints;
using UnityEngine;

public class SlideController : MonoBehaviour
{

    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private float MaximumGravitySpeed = 10f;
    [SerializeField] private float MaximumJumpTime = 0.5f;
    [SerializeField] private float MovementSpeed = 5f;

    public Rigidbody2D.SlideMovement SlideMovement = new Rigidbody2D.SlideMovement();
    private Rigidbody2D.SlideResults slideResults;
    private Rigidbody2D Rigidbody;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool jumpReleased = true;
    private float jumpHeld = 0;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        float horz = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal")) * MovementSpeed;
        bool jump = Input.GetAxis("Jump") > 0;

        bool justReleased = false;
        if (!jumpReleased && !jump)
        {
            jumpReleased = true;
            justReleased = true;
        }

        if (!jumpReleased)
        {
            jumpHeld += Time.fixedDeltaTime;
        }
        else
        {
            jumpHeld = 0;
        }

        // Calculate the horizontal velocity from keyboard input.
        var velocity = new Vector2(horz, 0f);

        if (slideResults.surfaceHit && jump && jumpReleased)
        {
            jumpReleased = false;
        }

        float jumpSlide;
        float jumpVelocity;

        if (!jumpReleased && jumpHeld < MaximumJumpTime)
        {
            // Rigidbody.AddForceY(JumpForce, ForceMode2D.Impulse);
            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, JumpForce);
        }

        if (Rigidbody.linearVelocityY < -MaximumGravitySpeed)
        {
            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, -MaximumGravitySpeed);
        }

        if (horz > 0 && !facingRight)
        {
            Flip();
        }
        else if (horz < 0 && facingRight)
        {
            Flip();
        }

        slideResults = Rigidbody.Slide(velocity, Time.deltaTime, SlideMovement);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
