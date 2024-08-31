using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakeOver : MonoBehaviour
{
    public float takeOverDist;
    public PlayerBodyType playerBodyType;

    private SpriteRenderer renderer;
    private Player player;
    private SlideController slideController;
    private Vector2 lastInputVector;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        slideController = GetComponent<SlideController>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lastInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        if (lastInput.magnitude > 0.2)
        {
            lastInputVector = lastInput;
        }

        var pp = player.GetCurrentPosition();
        var diff = pp - transform.position;
        if (Mathf.Abs(diff.magnitude) <= takeOverDist)
        {
            renderer.color = new Color(1f, 0f, 0f, 1f);
            if (Input.GetKeyDown(KeyCode.E))
            {
                // TODO: see if the last input is pointing towards you
                player.SetPlayerBodyType(playerBodyType);
                player.SetPosition(transform.position);
                player.SetOrientation(slideController.facingRight);
                Destroy(gameObject);
            }
        }
        else
        {
            renderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
