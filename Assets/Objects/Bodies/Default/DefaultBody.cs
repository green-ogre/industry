using UnityEngine;

public class DefaultBody : MonoBehaviour
{
    public Collider2D trigger;
    private Player player;

    void Update()
    {
        if (player && Input.GetKeyDown(KeyCode.E))
        {
            //             player.SetPlayerBodyType(PlayerBodyType.DEFAULT);
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
