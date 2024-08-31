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
	[SerializeField] private float DashSpeed = 15f;
	[SerializeField] private float DashTime = 0.1f;
	[SerializeField] private float DashRecoveryTime = 0.25f;
	[SerializeField, Range(0, 1)] private float DashMomentum = 0.3f;
	[SerializeField] private LayerMask DashMask;

	private float dashTimer = 0;
	private float dashRecovery = 0;

	private float gravityScale;
	public bool dashing;
	private bool dashPressed = false;
	private bool dashAirRecovery = true;

	private bool dashContact = false;
	public float dashContactTimeout = 0.1f;
	private float dashContactWidth;
	private float dashContactHeight;
	private float dashContactTimer;

	private Vector2 lastInputVector;

	private Rigidbody2D rigidBody;
	public Collider2D boxCollider;
	private Collider2D dashInteractCollider;
	private InputAction dashAction;
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
		var currentController = CurrentSlideController();
		var newController = bodies[(int)type].GetComponent<SlideController>();
		newController.facingRight = currentController.facingRight;

		bodies[(int)type].transform.position = bodies[(int)playerBodyType].transform.position;
		bodies[(int)type].GetComponent<Rigidbody2D>().position = bodies[(int)playerBodyType].GetComponent<Rigidbody2D>().position;
		bodies[(int)type].gameObject.SetActive(true);

		playerBodyType = type;
	}

	private SlideController CurrentSlideController()
	{
		return bodies[(int)playerBodyType].GetComponent<SlideController>();
	}

	// Start is called before the first frame update
	void Start()
	{
		dashAction = InputSystem.actions.FindAction("Dash");
		rigidBody = GetComponent<Rigidbody2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");
		dashContactTimer = dashContactTimeout;
		debugText = GetComponentInChildren<TMP_Text>();

		playerBodyType = PlayerBodyType.LABORER;
		SetPlayerBodyType(PlayerBodyType.LABORER);
	}

	void EnterDash()
	{
		dashTimer = DashTime;
		dashRecovery = DashTime + DashRecoveryTime;

		// gravityScale = rigidBody.gravityScale;
		rigidBody.gravityScale = 0;
		CurrentSlideController().SetJumpEnabled(false);
		CurrentSlideController().SetSlideEnabled(false);
		dashing = true;
		dashPressed = true;
		// rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		dashAirRecovery = false;
		interactingColliders.Clear();
	}

	void ExitDash()
	{
		// rigidBody.gravityScale = gravityScale;
		rigidBody.gravityScale = 1f;
		CurrentSlideController().SetJumpEnabled(true);
		CurrentSlideController().SetSlideEnabled(true);
		rigidBody.linearVelocity = DashVector() * DashMomentum;
		// rigidBody.linearVelocity = Vector2.zero;
		dashing = false;
		// rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
	}

	Vector2 DashVector()
	{
		return lastInputVector * DashSpeed;
	}

	void RefreshDash()
	{
		dashAirRecovery = true;
		dashRecovery = 0;
	}

	void Update()
	{
		ShowDebug();
	}

	private List<Collider2D> interactingColliders = new();

	// Update is called once per frame
	void FixedUpdate()
	{
		CurrentSlideController().horizontalInput = moveAction.ReadValue<Vector2>().x;
		CurrentSlideController().jumpInput = jumpAction.ReadValue<float>() > 0;


		var delta = Time.deltaTime;

		if (!dashing)
		{
			Vector2 lastInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
			if (lastInput.magnitude > 0.2)
			{
				lastInputVector = lastInput;
			}
		}

		// controllerRef.Move(horz, jump);


		if (playerBodyType == PlayerBodyType.LABORER)
		{
			var dashInput = dashAction.ReadValue<float>() > 0;
			if (dashInput && dashAirRecovery && dashRecovery <= 0 && !dashPressed)
			{
				EnterDash();
			}

			if (dashTimer > 0 && !dashContact)
			{
				dashTimer -= delta;
				// rigidBody.linearVelocity = CurrentSlideController().lastDirection.normalized * DashSpeed;
				rigidBody.linearVelocity = DashVector();

				List<RaycastHit2D> results = new();
				var filter = new ContactFilter2D();
				filter.SetLayerMask(new LayerMask() { value = DashMask.value });
				filter.useTriggers = true;

				// 	boxCollider.Cast(rigidBody.linearVelocity.normalized, filter, results, rigidBody.linearVelocity.magnitude * Time.deltaTime);
				// 	foreach (var hit in results)
				// 	{
				// 		if (!hit)
				// 		{
				// 			continue;
				// 		}

				// 		if (interactingColliders.Contains(hit.collider))
				// 		{
				// 			continue;
				// 		}
				// 		interactingColliders.Add(hit.collider);

				// 		var other = hit.collider;
				// 		rigidBody.linearVelocity = Vector2.zero;

				// 		rigidBody.position = hit.centroid;

				// 		dashContact = true;
				// 		CurrentSlideController().enabled = false;
				// 		dashInteractCollider = other;


				// 		if (other.transform.position.x < transform.position.x)
				// 		{
				// 			dashContactWidth = -(float)other.bounds.size.x - (float)boxCollider.bounds.size.x;
				// 		}
				// 		else
				// 		{
				// 			dashContactWidth = (float)other.bounds.size.x + (float)boxCollider.bounds.size.x;
				// 		}

				// 		if (other.transform.position.y > transform.position.y)
				// 		{
				// 			dashContactHeight = -(float)other.bounds.size.y - (float)boxCollider.bounds.size.y;
				// 		}
				// 		else
				// 		{
				// 			dashContactHeight = (float)other.bounds.size.y + (float)boxCollider.bounds.size.y;
				// 		}
				// 	}
			}

			if (dashTimer <= 0 && dashing)
			{
				ExitDash();
			}

			if (dashTimer <= 0 && CurrentSlideController().isGrounded && !dashAirRecovery)
			{
				dashAirRecovery = true;
			}

			if (dashRecovery > 0)
			{
				dashRecovery -= delta;
			}

			if (!dashInput && dashPressed)
			{
				dashPressed = false;
			}

			if (dashContact)
			{
				// dashContactTimer -= Time.deltaTime;

				// if (dashContactTimer <= 0f)
				// {
				// 	dashContactTimer = dashContactTimeout;
				// 	dashContact = false;
				// 	CurrentSlideController().enabled = true;
				// 	RefreshDash();

				// 	if (dashInteractCollider)
				// 	{
				// 		TeleportThroughObject(dashInteractCollider);
				// 	}
				// }
			}
		}
	}

	public void TeleportThroughObject(Collider2D other)
	{
		// Use the velocity direction for teleportation
		Vector3 teleportDirection = lastInputVector;

		// Get the bounds of both objects
		Bounds teleportingBounds = boxCollider.bounds;
		Bounds stationaryBounds = other.bounds;

		// Calculate the distance to teleport
		float distanceToTeleport = CalculateTeleportDistance(teleportingBounds, stationaryBounds, teleportDirection);

		// Calculate new position
		Vector3 newPosition = transform.position + teleportDirection * distanceToTeleport;

		// Teleport the object by directly setting its position
		rigidBody.MovePosition(newPosition);

		// Optionally, you might want to preserve the velocity
		// If you want to stop the object instead, uncomment the next line
		// objectToTeleport.velocity = Vector2.zero;
	}

	private float CalculateTeleportDistance(Bounds teleportingBounds, Bounds stationaryBounds, Vector2 direction)
	{
		// Cast a ray from the teleporting object through the stationary object
		RaycastHit2D hit = Physics2D.Raycast(teleportingBounds.center, direction);

		// Calculate the distance to the far side of the stationary object
		float distanceToFarSide = Vector2.Dot(stationaryBounds.max - teleportingBounds.center, direction);

		// Add a small offset to ensure clearance
		return distanceToFarSide + teleportingBounds.extents.magnitude + 0.1f;
	}

	private static bool ContainsDashInteract(Collider2D other)
	{
		return (other.includeLayers.value & (1 << LayerMask.GetMask("DashInteract"))) > 0 || (other.includeLayers.value & (1 << LayerMask.GetMask("Enemy"))) > 0;
	}

	private void HandleDashCollision(Collider2D other)
	{
		if (dashing && ContainsDashInteract(other))
		{
			dashContact = true;
			CurrentSlideController().enabled = false;
			dashInteractCollider = other;


			if (other.transform.position.x < transform.position.x)
			{
				dashContactWidth = -(float)other.bounds.size.x - (float)boxCollider.bounds.size.x;
			}
			else
			{
				dashContactWidth = (float)other.bounds.size.x + (float)boxCollider.bounds.size.x;
			}

			if (other.transform.position.y > transform.position.y)
			{
				dashContactHeight = -(float)other.bounds.size.y - (float)boxCollider.bounds.size.y;
			}
			else
			{
				dashContactHeight = (float)other.bounds.size.y + (float)boxCollider.bounds.size.y;
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

	// private void OnTriggerEnter2D(Collider2D other)
	// {
	// 	HandleDashCollision(other);
	// }

	// private void OnCollisionEnter2D(Collision2D other)
	// {
	// 	HandleDashCollision(other.collider);
	// }
}
