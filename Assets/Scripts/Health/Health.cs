using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int max;
    private int current;

    public bool IsDead()
    {
        return current <= 0f;
    }

    public void SetDead()
    {
        current = 0;
    }

    public int Max()
    {
        return max;
    }

    public int Current()
    {
        return current;
    }

    void Start()
    {
        current = max;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var attack = other.GetComponent<Attack>();
        if (attack)
        {
            current -= attack.Damage();
        }
    }
}
