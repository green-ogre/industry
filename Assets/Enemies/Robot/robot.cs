using UnityEngine;

public class Robot : MonoBehaviour
{
    public int maxHealth;
    private int health;
    public float maxAgro;
    public bool active;

    private SlideController slideController;
    private Knockback knockback;
    private Player player;

    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];
    private HealthBar healthBar;
    private Transform controllerTransform;

    void Start()
    {
        controllerTransform = transform.Find("RobotBody");
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        boxCollider = GetComponentInChildren<Collider2D>();
        slideController = GetComponentInChildren<SlideController>();
        knockback = GetComponentInChildren<Knockback>();
        healthBar = GetComponentInChildren<HealthBar>();
        player = GameObject.Find("player").GetComponent<Player>();
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
            Vector3 diff = player.GetCurrentPosition() - controllerTransform.position;
            if (Mathf.Abs(diff.magnitude) <= maxAgro)
            {
                if (active)
                {
                    if (player.GetCurrentPosition().x > controllerTransform.position.x)
                    {
                        slideController.horizontalInput = 1;
                    }
                    else
                    {
                        slideController.horizontalInput = -1;
                    }
                }
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
        // Player player = other.gameObject.GetComponent<Player>();
        // if (player && player.dashing)
        // {
        //     Debug.Log("enemy recieve collision");

        //     rigidBody.bodyType = RigidbodyType2D.Kinematic;
        //     boxCollider.enabled = false;
        //     slideController.enabled = false;
        //     knockback.HandleCollisionEnter2D(other);

        //     if (!invincible)
        //     {
        //         health -= 1;
        //         // AudioMaster.instance.PlayClip(soundFX[0], Vector3.zero, 1f);

        //         if (health <= 0)
        //         {
        //             player.SetPlayerBodyType(PlayerBodyType.DEFAULT);
        //         }
        //     }
        // }
    }
}
