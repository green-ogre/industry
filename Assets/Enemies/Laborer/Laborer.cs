using UnityEngine;

public class Laborer : MonoBehaviour
{
    public float maxAgro;
    public bool active;

    private SlideController slideController;
    private Knockback knockback;
    private Transform player;

    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];
    private HealthBar healthBar;
    private Transform controllerTransform;


    void Start()
    {
        controllerTransform = transform.Find("LaborerBody");
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        boxCollider = GetComponentInChildren<Collider2D>();
        slideController = GetComponentInChildren<SlideController>();
        knockback = GetComponentInChildren<Knockback>();
        // healthBar = GetComponentInChildren<HealthBar>();
        player = GameObject.Find("player").transform;
    }

    void Update()
    {
        if (!knockback.inKnockback)
        {
            Vector3 diff = player.position - controllerTransform.position;
            if (Mathf.Abs(diff.magnitude) <= maxAgro)
            {
                if (active)
                {
                    if (player.position.x > controllerTransform.position.x)
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
}
