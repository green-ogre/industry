using UnityEngine;

public class Robot : MonoBehaviour
{
    public float maxHealth;
    private float health;

    public float maxMoveSpeed = 0.8f;
    public float moveSpeed = 0.8f;
    public float agroDist = 5f;

    private SlideController slideController;

    private Knockback knockback;

    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];

    void Start()
    {
        // target = GameObject.Find("player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        slideController = GetComponent<SlideController>();
        knockback = GetComponent<Knockback>();
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }

        if (!knockback.inKnockback)
        {
            Vector3 diff = target.position - transform.position;
            slideController.horizontalInput = diff.x;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player && player.dashing)
        {
            Debug.Log("enemy recieve collision");

            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            boxCollider.enabled = false;
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

            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            boxCollider.enabled = false;
            slideController.enabled = false;

            if (!invincible)
            {
                health -= 1f;
                AudioMaster.instance.PlayClip(soundFX[0], Vector3.zero, 1f);
            }
        }
    }
}
