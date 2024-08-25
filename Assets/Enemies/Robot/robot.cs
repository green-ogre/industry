using UnityEngine;

public class Robot : MonoBehaviour
{
    public float maxHealth;
    private float health;

    [SerializeField] public float maxMoveSpeed = 0.8f;
    [SerializeField] public float moveSpeed = 0.8f;
    [SerializeField] public float agroDist = 5f;

    private SlideController slideController;

    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    private float dashContactTimer;
    private Vector2 knockbackVector;
    public float recievedKnockbackStrength;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];

    void Start()
    {
        // target = GameObject.Find("player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        slideController = GetComponent<SlideController>();
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }

        if (dashContactTimer > 0)
        {
            dashContactTimer -= Time.deltaTime;
            rigidBody.linearVelocity = Vector2.zero;

            if (dashContactTimer <= 0f)
            {
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                boxCollider.enabled = true;
                // slideController.enabled = true;
                rigidBody.linearVelocityX = knockbackVector.normalized.x * recievedKnockbackStrength;
                // rigidBody.linearVelocity = knockbackVector.normalized * recievedKnockbackStrength;
            }
        }
        else if (target)
        {

            if (slideController.enabled)
            {

                Vector3 diff = target.position - transform.position;
                slideController.horizontalInput = diff.x;
                // moveDirection = diff.normalized;
                // var newVelocity = Vector2.ClampMagnitude(new Vector2(moveDirection.x, moveDirection.y) * moveSpeed, maxMoveSpeed);
                // rigidBody.linearVelocity = newVelocity;
            }
            else
            {
                if (rigidBody.linearVelocity.magnitude < 0.1f)
                {
                    slideController.enabled = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.name == "player")
        // {
        //     target = other.transform;
        // }

        Player player = other.gameObject.GetComponent<Player>();
        if (player && player.dashing)
        {
            Debug.Log("enemy recieve collision");

            float timeForPlayerToDodgeThrough = 0.1f;
            dashContactTimer = player.dashContactTimeout + timeForPlayerToDodgeThrough;
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            boxCollider.enabled = false;
            Rigidbody2D playerRigid = other.gameObject.GetComponent<Rigidbody2D>();
            knockbackVector = playerRigid.linearVelocity;
            slideController.enabled = false;

            if (!invincible)
            {
                health -= 1f;
                AudioMaster.instance.PlayClip(soundFX[0], Vector3.zero, 1f);
            }
            target = other.transform;
        }
        else if (player)
        {
            target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "player")
        {
            target = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player && player.dashing)
        {
            Debug.Log("enemy recieve collision");

            float timeForPlayerToDodgeThrough = 0.1f;
            dashContactTimer = player.dashContactTimeout + timeForPlayerToDodgeThrough;
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            boxCollider.enabled = false;
            Rigidbody2D playerRigid = other.gameObject.GetComponent<Rigidbody2D>();
            knockbackVector = playerRigid.linearVelocity;
            slideController.enabled = false;

            if (!invincible)
            {
                health -= 1f;
                AudioMaster.instance.PlayClip(soundFX[0], Vector3.zero, 1f);
            }
        }
    }
}
