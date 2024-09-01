using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakeOver : MonoBehaviour
{
    public float takeOverDist;
    public float takeOverHealth;
    public PlayerBodyType playerBodyType;

    private SpriteRenderer renderer;
    private Player player;
    private SlideController slideController;
    private Vector2 lastInputVector;
    private Health health;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        slideController = GetComponent<SlideController>();
        health = transform.parent.gameObject.GetComponentInChildren<Health>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

    public Vector2 Position()
    {
        return transform.position;
    }

    public void SetDead()
    {
        health.SetDead();
    }

    public bool Orientation()
    {
        return slideController.facingRight;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            renderer.color = new Color(1f, 0f, 0f, 1f);
        }
        else
        {
            renderer.color = new Color(1f, 1f, 1f, 1f);
        }
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
        if (Mathf.Abs(diff.magnitude) <= takeOverDist && health.Current() <= takeOverHealth)
        {
            player.InsertEnemy(this);
        }
        else
        {
            player.RemoveEnemy(this);
            SetSelected(false);
        }
    }
}
