using System.Collections.Generic;
using System.Linq;
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
	private InputAction attackAction;

	private List<TakeOver> nearbyEnemies = new();

	public PlayerBodyType playerBodyType;
	public GameObject[] bodies;

	private Vector2 lastInputVector;

	public TMP_Text debugText;

	public void InsertEnemy(TakeOver enemy)
	{
		if (!nearbyEnemies.Contains(enemy))
		{
			nearbyEnemies.Add(enemy);
		}
	}

	public void RemoveEnemy(TakeOver enemy)
	{
		nearbyEnemies.Remove(enemy);
	}

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

	private Attack CurrentAttack()
	{
		return bodies[(int)playerBodyType].GetComponentInChildren<Attack>();
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
		attackAction = InputSystem.actions.FindAction("Attack");
		debugText = GetComponentInChildren<TMP_Text>();
		playerBodyType = PlayerBodyType.LABORER;
		SetPlayerBodyType(PlayerBodyType.LABORER);
	}


	void Update()
	{
		ShowDebug();

		CurrentSlideController().horizontalInput = moveAction.ReadValue<Vector2>().x;
		CurrentSlideController().jumpInput = jumpAction.ReadValue<float>() > 0;

		if (attackAction.ReadValue<float>() > 0f)
		{
			var attack = CurrentAttack();
			// Debug.Log("try attack input");
			if (attack)
			{
				// Debug.Log("attack input");
				attack.attack = true;
			}
		}
	}

	void LateUpdate()
	{
		Vector2 lastInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
		if (lastInput.magnitude > 0.2)
		{
			lastInputVector = lastInput;
		}
		Vector2 currentPosition = GetCurrentPosition();

		nearbyEnemies.RemoveAll(e => e == null);

		// Sort first by comparing the direction of player intention to the direction to the enemy.
		// For enemies where this value is identical, sort by distance.
		nearbyEnemies.Sort((a, b) =>
		{
			var comp = Vector2.Dot(
				(a.Position() - currentPosition).normalized,
				lastInputVector.normalized
			)
			.CompareTo(
				Vector2.Dot(
					(b.Position() - currentPosition).normalized,
					lastInputVector.normalized
				)
			);

			if (comp == 0)
			{
				return Vector2.Distance(
					a.Position(),
					transform.position
				)
				.CompareTo(
					Vector2.Distance(
						b.Position(),
						transform.position
					)
				);
			}

			return comp;
		});

		foreach (var enemy in nearbyEnemies)
		{
			enemy.SetSelected(false);
		}

		if (nearbyEnemies.Count > 0)
		{
			var closest = nearbyEnemies
				.AsEnumerable()
				.Reverse()
				.Where(e => Vector2.Dot(e.Position() - currentPosition, lastInputVector) > 0)
				.FirstOrDefault();

			if (closest != null)
			{
				closest.SetSelected(true);

				if (Input.GetKeyDown(KeyCode.E))
				{
					// TODO: see if the last input is pointing towards you
					SetPlayerBodyType(closest.playerBodyType);
					SetPosition(closest.Position());
					SetOrientation(closest.Orientation());

					// TODO: need a better way to despawn enemies that are taken over
					closest.SetDead();
					nearbyEnemies.RemoveAt(nearbyEnemies.Count - 1);
				}
			}
		}
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

		debugText.text = System.String.Format("{0}", playerBodyType);
	}
}
