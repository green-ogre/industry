using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "player")
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "player")
        {
            player = null;
        }
    }
}
