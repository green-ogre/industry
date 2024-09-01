using System.Collections;
using TMPro;
using UnityEngine;

enum RobotState
{
    WANDERING,
    CHASING,
    ATTACKING,
}

enum SelectedPoint
{
    RIGHT,
    LEFT
}

public class Robot : MonoBehaviour
{
    public float maxAgro;
    public bool active;

    private SlideController slideController;
    private Knockback knockback;
    private Player player;

    private Rigidbody2D rigidBody;
    private Collider2D boxCollider;
    public bool invincible = false;
    public AudioClip[] soundFX = new AudioClip[5];
    // private HealthBar healthBar;
    private Health health;
    private Transform controllerTransform;

    private RobotState state;
    public GameObject rightPoint;
    public GameObject leftPoint;

    private SelectedPoint selectedPoint;
    private float waitTimer;
    private Attack attack;
    public float attackCooldownDuration;
    public float attackRange;
    private float attackCooldown;

    private TMP_Text debugText;

    void Start()
    {
        debugText = GetComponentInChildren<TMP_Text>();

        attack = GetComponentInChildren<Attack>();
        selectedPoint = SelectedPoint.RIGHT;
        state = RobotState.WANDERING;
        controllerTransform = transform.Find("RobotBody");
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        boxCollider = GetComponentInChildren<Collider2D>();
        slideController = GetComponentInChildren<SlideController>();
        knockback = GetComponentInChildren<Knockback>();
        // healthBar = GetComponentInChildren<HealthBar>();
        health = GetComponentInChildren<Health>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

    void Update()
    {
        ShowDebug();

        if (health.IsDead())
        {
            Destroy(gameObject);
        }

        switch (state)
        {
            case RobotState.WANDERING:
                {
                    Wander();
                    break;
                }
            case RobotState.CHASING:
                {
                    Chase();
                    break;
                }
            case RobotState.ATTACKING:
                {
                    Attack();
                    break;
                }
        }
    }

    private void Wander()
    {
        attackCooldown = 0f;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            slideController.horizontalInput = 0f;
            return;
        }

        var point = selectedPoint switch
        {
            SelectedPoint.RIGHT => rightPoint,
            SelectedPoint.LEFT => leftPoint,
        };

        if (Mathf.Abs(rigidBody.position.x - point.transform.position.x) < 1f)
        {
            PauseForRandSeconds();
            switch (selectedPoint)
            {
                case SelectedPoint.RIGHT:
                    {
                        selectedPoint = SelectedPoint.LEFT;
                        break;
                    }
                case SelectedPoint.LEFT:
                    {
                        selectedPoint = SelectedPoint.RIGHT;
                        break;
                    }
            }
        }
        else
        {
            switch (selectedPoint)
            {
                case SelectedPoint.RIGHT:
                    {
                        slideController.horizontalInput = 1;
                        break;
                    }
                case SelectedPoint.LEFT:
                    {
                        slideController.horizontalInput = -1;
                        break;
                    }
            }
        }

        if (PlayerInDistance())
        {
            if (active)
            {
                state = RobotState.CHASING;
            }
        }
    }

    private bool PlayerInDistance()
    {
        return Mathf.Abs(player.GetCurrentPosition().x - controllerTransform.position.x) <= maxAgro;
    }

    private void Chase()
    {
        // if (!knockback.inKnockback)
        // {

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        if (PlayerInDistance())
        {
            if (attackCooldown > 0f)
            {
                attackCooldown -= Time.deltaTime;
            }

            var diff = player.GetCurrentPosition().x - controllerTransform.position.x;
            if (Mathf.Abs(diff) <= attackRange && attackCooldown <= 0f)
            {
                state = RobotState.ATTACKING;
                attackCooldown = attackCooldownDuration;
                slideController.horizontalInput = 0;
            }
            else if (Mathf.Abs(diff) <= attackRange)
            {
                slideController.horizontalInput = 0;
            }
            else
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
            PauseForRandSeconds();
            state = RobotState.WANDERING;
        }
        // }
        // else if (!knockback.inKnockback)
        // {
        //     slideController.horizontalInput = 0f;
        //     state = RobotState.WANDERING;
        //     PauseForRandSeconds();
        // }
    }

    private void Attack()
    {
        Debug.Log("robot attack!");
        attack.attack = true;
        // PauseForRandSeconds(0.5f, 1.5f);
        state = RobotState.CHASING;
    }

    private void PauseForRandSeconds()
    {
        waitTimer = Random.Range(1f, 3f);
    }

    private void PauseForRandSeconds(float min, float max)
    {
        waitTimer = Random.Range(min, max);
    }

    private IEnumerator ChooseWanderDirection()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    private void ShowDebug()
    {
        debugText.text = System.String.Format("State: {0}\nAttack Cooldown: {1}\nDist: {2}", state, attackCooldown, Mathf.Abs(player.GetCurrentPosition().x - controllerTransform.position.x));
    }
}
