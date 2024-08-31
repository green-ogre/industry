using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public float attackCooldownDuration;
    public float attackDuration;
    public bool attack;
    public Collider2D hitBox;

    private float attackCooldown;
    private float attackDurationTimer;
    private InputAction moveAction;
    private bool facingRight;
    [SerializeField] private int damage;

    public int Damage()
    {
        return damage;
    }

    void Start()
    {
        facingRight = true;
        hitBox = GetComponent<BoxCollider2D>();
        hitBox.enabled = false;
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        if (attack && attackCooldown <= 0f)
        {
            attackDurationTimer = attackDuration;
            hitBox.enabled = true;
            attack = false;
        }

        if (attackDurationTimer > 0f)
        {
            attackDurationTimer -= Time.deltaTime;

            if (attackDurationTimer <= 0f)
            {
                hitBox.enabled = false;
                attackCooldown = attackCooldownDuration;
            }
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        float horz = moveAction.ReadValue<Vector2>().x;
        if (horz > 0 && !facingRight)
        {
            Flip();
        }
        else if (horz < 0 && facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        hitBox.offset = new Vector2(-hitBox.offset.x, hitBox.offset.y);
    }
}
