using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerBodyType : int
{
	LABORER,
	ROBOT,
}

public class Player : MonoBehaviour
{
	public Collider2D boxCollider;
	public Animator animator;

	private InputAction moveAction;
	private InputAction jumpAction;

	public PlayerBodyType playerBodyType;
	public GameObject[] bodies;

	public TMP_Text debugText;

	public void SetPlayerBodyType(PlayerBodyType type)
	{
		foreach (var body in bodies)
		{
			body.gameObject.SetActive(false);
		}

		bodies[(int)type].transform.position = bodies[(int)playerBodyType].transform.position;
		bodies[(int)type].GetComponent<Rigidbody2D>().position = bodies[(int)playerBodyType].GetComponent<Rigidbody2D>().position;
		bodies[(int)type].gameObject.SetActive(true);

		playerBodyType = type;
	}

	private SlideController CurrentSlideController()
	{
		return bodies[(int)playerBodyType].GetComponent<SlideController>();
	}

	private SpriteRenderer CurrentSpriteRenderer()
	{
		return bodies[(int)playerBodyType].GetComponent<SpriteRenderer>();
	}

	public Vector3 GetCurrentPosition()
	{
		return bodies[(int)playerBodyType].transform.position;
	}

	public void SetPosition(Vector3 position)
	{
		bodies[(int)playerBodyType].transform.position = position;
		bodies[(int)playerBodyType].GetComponent<Rigidbody2D>().position = position;
	}

	public void SetOrientation(bool facingRight)
	{
		CurrentSlideController().facingRight = facingRight;
		CurrentSpriteRenderer().flipX = !facingRight;
	}

	private bool IsDashing()
	{
		var dash = bodies[(int)playerBodyType].GetComponent<Dash>();
		if (dash)
		{
			return dash.dashing;
		}
		else
		{
			return false;
		}
	}

	void Start()
	{
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");
		debugText = GetComponentInChildren<TMP_Text>();

		playerBodyType = PlayerBodyType.LABORER;
		SetPlayerBodyType(PlayerBodyType.LABORER);
	}

	void Update()
	{
		ShowDebug();

		CurrentSlideController().horizontalInput = moveAction.ReadValue<Vector2>().x;
		CurrentSlideController().jumpInput = jumpAction.ReadValue<float>() > 0;
	}

	private void ShowDebug()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetPlayerBodyType(PlayerBodyType.LABORER);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetPlayerBodyType(PlayerBodyType.ROBOT);
		}

		switch (playerBodyType)
		{
			case PlayerBodyType.LABORER:
				{
					debugText.text = "BodyType: Laborer";
					break;
				}
			case PlayerBodyType.ROBOT:
				{
					debugText.text = "BodyType: ROBOT";
					break;
				}
		}
	}
}
