using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SlideController : MonoBehaviour
{
    [SerializeField] private float MovementSpeed = 5f;
    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private float HeldJumpForce = 40f;
    [SerializeField] private float MaximumFallSpeed = 20f;
    [SerializeField] private float MaximumJumpTime = 0.2f;
    [SerializeField, Range(0, 1)] private float AirAcceleration = 0.3f;
    [SerializeField, Range(0, 1)] private float AirDamping = 0.95f;

    private Rigidbody2D.SlideMovement SlideMovement = new();
    private Rigidbody2D.SlideResults slideResults;
    private Rigidbody2D Rigidbody;

    public bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool jumpReleased = true;
    private float jumpHeld = 0;

    public Vector2 lastDirection;
    public Vector2 lastVelocity;
    private bool jumpEnabled = true;
    private bool slideEnabled = true;
    public bool isGrounded = true;

    [System.NonSerialized] public float horizontalInput;
    [System.NonSerialized] public bool jumpInput;

    private Vector2 effectiveVelocity;

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
        if (!Rigidbody)
        {
            Rigidbody = GetComponentInParent<Rigidbody2D>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
        }

        SlideMovement = new Rigidbody2D.SlideMovement()
        {
            maxIterations = 5,
            gravity = Vector2.zero,
            surfaceAnchor = new Vector2(0, -0.05f),
            useLayerMask = true,
            layerMask = new LayerMask() { value = (1 << LayerMask.NameToLayer("Ground")) },
        };
    }

    void FixedUpdate()
    {
        var previousGrounded = isGrounded;

        if (slideEnabled)
        {
            // ground movement
            if (isGrounded)
            {
                var velocity = CalculateVelocity();

                lastVelocity = new Vector2(velocity.x, Rigidbody.linearVelocityY);

                var previousPos = Rigidbody.position;
                slideResults = Rigidbody.Slide(velocity, Time.deltaTime, SlideMovement);
                effectiveVelocity = new Vector2((slideResults.position.x - previousPos.x) / Time.deltaTime, Rigidbody.linearVelocityY);
            }
            // air movement
            else
            {
                var velocity = CalculateVelocity();

                Rigidbody.linearVelocity *= new Vector2(AirDamping, 1);

                if (velocity.x > 0)
                {
                    if (Rigidbody.linearVelocityX < MovementSpeed)
                    {
                        Rigidbody.linearVelocity += new Vector2(velocity.x * AirAcceleration, 0);
                        if (Rigidbody.linearVelocityX > MovementSpeed)
                        {
                            Rigidbody.linearVelocity = new Vector2(MovementSpeed, Rigidbody.linearVelocityY);
                        }
                    }
                }
                else if (velocity.x < 0)
                {
                    if (Rigidbody.linearVelocityX > -MovementSpeed)
                    {
                        Rigidbody.linearVelocity += new Vector2(velocity.x * AirAcceleration, 0);
                        if (Rigidbody.linearVelocityX < -MovementSpeed)
                        {
                            Rigidbody.linearVelocity = new Vector2(-MovementSpeed, Rigidbody.linearVelocityY);
                        }
                    }
                }

                if (Rigidbody.linearVelocityY < -MaximumFallSpeed)
                {
                    Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, -MaximumFallSpeed);
                }
            }
        }


        UpdateGrounded();

        if (jumpEnabled)
        {
            bool justJumped = UpdateJump(Time.fixedDeltaTime);
            if (justJumped)
            {
                // Rigidbody.AddForceY(JumpForce, ForceMode2D.Impulse);
                Rigidbody.linearVelocity = new Vector2(0, JumpForce + effectiveVelocity.y);
                isGrounded = false;
            }
            else if (!jumpReleased && jumpHeld < MaximumJumpTime)
            {
                Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocityX, Rigidbody.linearVelocityY + HeldJumpForce * Time.fixedDeltaTime);
            }
        }

        if (previousGrounded && !isGrounded)
        {
            AirTransition();
        }
    }

    /// <summary>
    /// Transition from ground sliding to air movement.
    /// </summary>
    private void AirTransition()
    {
        Rigidbody.linearVelocity += new Vector2(effectiveVelocity.x, 0);
    }

    private Vector2 CalculateVelocity()
    {
        float horz = AxisNormalize.Movement(horizontalInput) * MovementSpeed;
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

        if (!jumpReleased && !jumpInput)
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
        if (isGrounded && jumpInput && jumpReleased)
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

        // spriteRenderer.flipX = !spriteRenderer.flipX;
        var scale = transform.localScale;
        if (facingRight && scale.x < 0f)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
        }
        else if (!facingRight && scale.x > 0f)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
        }
    }

    private void UpdateGrounded()
    {
        if (slideEnabled && isGrounded)
        {
            isGrounded = slideResults.surfaceHit;
        }
        else
        {
            var oldDisable = SlideMovement.useNoMove;
            SlideMovement.useNoMove = true;
            var result = Rigidbody.Slide(Vector2.zero, Time.deltaTime, SlideMovement);

            isGrounded = result.surfaceHit;
            SlideMovement.useNoMove = oldDisable;
        }

    }
}
