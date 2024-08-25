using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 0.8f;
    [SerializeField] public float agroDist = 5f;

    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody2D rigidBody;

    void Start()
    {
        target = GameObject.Find("player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target) {

            
            if (Vector3.Distance(target.position, transform.position) > agroDist) {
                moveDirection = Vector3.zero;
            } 
            else 
            {
                Vector3 diff = target.position - transform.position;
                moveDirection = (diff).normalized;
            }
        }
    }

    void FixedUpdate()
    {
        if (target) {
            rigidBody.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }
}
