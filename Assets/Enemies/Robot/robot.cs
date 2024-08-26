using UnityEngine;

public class Robot : MonoBehaviour
{
    public int maxHealth;
    private int health;
    public float maxAgro;

    private SlideController slideController;
    private Knockback knockback;
    private Transform player;

    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];
    private HealthBar healthBar;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        slideController = GetComponent<SlideController>();
        knockback = GetComponent<Knockback>();
        healthBar = GetComponentInChildren<HealthBar>();
        player = GameObject.Find("player").transform;
        health = maxHealth;
    }

    void Update()
    {
        healthBar.health = health;

        if (health <= 0f)
        {
            Destroy(gameObject);
        }

        if (!knockback.inKnockback)
        {
            Vector3 diff = player.position - transform.position;
            if (diff.magnitude <= maxAgro)
            {
                slideController.horizontalInput = diff.x;
            }
            else
            {
                slideController.horizontalInput = 0f;
            }
        }
        else if (!knockback.inKnockback)
        {
            slideController.horizontalInput = 0f;
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
            knockback.HandleCollisionEnter2D(other);

            if (!invincible)
            {
                health -= 1;
                AudioMaster.instance.PlayClip(soundFX[0], Vector3.zero, 1f);
            }
        }
    }
}
