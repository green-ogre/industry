using UnityEngine;

public class Bipedal : MonoBehaviour
{
    public Collider2D trigger;
    private Player player;

    void Update()
    {
        if (player && Input.GetKeyDown(KeyCode.E))
        {
            // player.SetPlayerBodyType(PlayerBodyType.BIPEDAL);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerObject = other.GetComponent<Player>();

        if (playerObject)
        {
            player = playerObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var playerObject = other.GetComponent<Player>();

        if (playerObject)
        {
            player = null;
        }
    }
}
