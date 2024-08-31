using UnityEngine;

public class Knockback : MonoBehaviour
{
    private Vector2 knockbackVector;
    public float recievedKnockbackStrength;
    public bool inKnockback = false;
    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    private SlideController slideController;
    private float dashContactTimer;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        slideController = GetComponent<SlideController>();
    }

    public void HandleCollisionEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        // if (player && player.dashing)
        // {
        //     Rigidbody2D playerRigid = other.gameObject.GetComponent<Rigidbody2D>();
        //     float timeForPlayerToDodgeThrough = 0.1f;
        //     dashContactTimer = player.dashContactTimeout + timeForPlayerToDodgeThrough;
        //     knockbackVector = playerRigid.linearVelocity;
        //     slideController.enabled = false;
        //     inKnockback = true;
        // }
    }

    void FixedUpdate()
    {
        if (inKnockback)
        {
            if (dashContactTimer > 0)
            {
                dashContactTimer -= Time.deltaTime;
                rigidBody.linearVelocity = Vector2.zero;

                if (dashContactTimer <= 0f)
                {
                    rigidBody.bodyType = RigidbodyType2D.Dynamic;
                    boxCollider.enabled = true;
                    rigidBody.linearVelocityX = knockbackVector.normalized.x * recievedKnockbackStrength;
                }
            }
            else
            {
                if (rigidBody.linearVelocity.magnitude < 0.1f)
                {
                    slideController.enabled = true;
                    inKnockback = false;
                }
            }
        }
    }
}
