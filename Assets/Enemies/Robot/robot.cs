using UnityEngine;

public class Robot : MonoBehaviour
{
    public float maxHealth;
    private float health;

    [SerializeField] public float maxMoveSpeed = 0.8f;
    [SerializeField] public float moveSpeed = 0.8f;
    [SerializeField] public float agroDist = 5f;

    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    private float dashContactTimer;
    private Vector2 knockbackVector;
    public float recievedKnockbackStrength;
    public bool invincible = false;
    public AudioSource takeHit;

    void Start()
    {
        // target = GameObject.Find("player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (dashContactTimer > 0)
        {
            dashContactTimer -= Time.deltaTime;
            rigidBody.linearVelocity = Vector2.zero;

            if (dashContactTimer <= 0f)
            {
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                boxCollider.enabled = true;
                rigidBody.linearVelocity = knockbackVector.normalized * recievedKnockbackStrength;
            }
        }
        else if (target)
        {
            Vector3 diff = target.position - transform.position;
            moveDirection = diff.normalized;
            var newVelocity = Vector2.ClampMagnitude(new Vector2(moveDirection.x, moveDirection.y) * moveSpeed, maxMoveSpeed);
            rigidBody.linearVelocity = newVelocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "player")
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

            if (!invincible)
            {
                health -= 1f;
                AudioMaster.instance.PlayClip(takeHit.clip, Vector3.zero, 1f);
            }
        }
    }
}
