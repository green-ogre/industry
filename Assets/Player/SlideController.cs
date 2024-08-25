using UnityEngine;

public class SlideController : MonoBehaviour
{

    [SerializeField] private float JumpForce = 10f;
    [SerializeField] private float MovementSpeed = 2f;
    [SerializeField] private float MaximumGravitySpeed = 10f;

    public Rigidbody2D.SlideMovement SlideMovement = new Rigidbody2D.SlideMovement();
    public Rigidbody2D.SlideResults SlideResults;
    private Rigidbody2D Rigidbody;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool jumpReleased = true;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        float horz = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal")) * MovementSpeed;
        bool jump = Input.GetAxis("Jump") > 0;

        if (!jumpReleased && !jump)
        {
            jumpReleased = true;
        }

        // Calculate the horizontal velocity from keyboard input.
        var velocity = new Vector2(horz, 0f);

        if (SlideResults.surfaceHit && jump && jumpReleased)
        {
            jumpReleased = false;
            Rigidbody.AddForceY(JumpForce, ForceMode2D.Impulse);
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

        SlideResults = Rigidbody.Slide(velocity, Time.deltaTime, SlideMovement);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
