using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] private float DashSpeed = 15f;
	[SerializeField] private float DashTime = 0.1f;
	[SerializeField] private float DashRecoveryTime = 0.25f;
	[SerializeField, Range(0, 1)] private float DashMomentum = 0.3f;

	private float dashTimer = 0;
	private float dashRecovery = 0;

	private float gravityScale;
	public bool dashing;
	private bool dashPressed = false;

	private bool dashContact = false;
	public float dashContactTimeout = 0.1f;
	private float dashContactWidth;
	private float dashContactHeight;
	private float dashContactTimer;

	private bool dashRefreshed = false;

	private Vector2 lastInputVector;

	private SlideController slideController;
	private Rigidbody2D rigidBody;
	private Collider2D boxCollider;
	private Collider2D dashInteractCollider;
	private InputAction dashAction;
	public Animator animator;

	private InputAction moveAction;
	private InputAction jumpAction;

	// Start is called before the first frame update
	void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		dashAction = InputSystem.actions.FindAction("Dash");
		slideController = GetComponent<SlideController>();
		rigidBody = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<Collider2D>();
		moveAction = InputSystem.actions.FindAction("Move");
		jumpAction = InputSystem.actions.FindAction("Jump");

		dashContactTimer = dashContactTimeout;
	}

	void EnterDash()
	{
		dashTimer = DashTime;
		dashRecovery = DashTime + DashRecoveryTime;

		// gravityScale = rigidBody.gravityScale;
		rigidBody.gravityScale = 0;
		slideController.SetJumpEnabled(false);
		slideController.SetSlideEnabled(false);
		dashing = true;
		dashPressed = true;
		rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
	}

	void ExitDash()
	{
		// rigidBody.gravityScale = gravityScale;
		rigidBody.gravityScale = 1f;
		slideController.SetJumpEnabled(true);
		slideController.SetSlideEnabled(true);
		rigidBody.linearVelocity = DashVector() * DashMomentum;
		// rigidBody.linearVelocity = new Vector2(slideController.lastVelocity.x, 0);
		dashing = false;
		rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
	}

	Vector2 DashVector()
	{
		return lastInputVector * DashSpeed;
	}

	void Update()
	{
		slideController.horizontalInput = moveAction.ReadValue<Vector2>().x;
		slideController.jumpInput = jumpAction.ReadValue<float>() > 0;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		var delta = Time.deltaTime;
		var dashInput = dashAction.ReadValue<float>() > 0;

		if (!dashing)
		{
			Vector2 lastInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
			if (lastInput.magnitude > 0.2)
			{
				lastInputVector = lastInput;
			}
		}

		// controllerRef.Move(horz, jump);
		bool isGrounded = slideController.isGrounded;
		animator.SetBool("isFalling", !isGrounded);
		float x = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal"));
		animator.SetBool("isRunning", Mathf.Abs(x) > 1e-10 && isGrounded);

		if (dashRefreshed)
		{
			dashRecovery = 0f;
			dashRefreshed = false;
		}

		if (dashInput && dashRecovery <= 0 && !dashPressed)
		{
			EnterDash();
		}

		if (dashTimer > 0 && !dashContact)
		{
			dashTimer -= delta;
			// rigidBody.linearVelocity = slideController.lastDirection.normalized * DashSpeed;
			rigidBody.linearVelocity = DashVector();
		}

		if (dashTimer <= 0 && dashing)
		{
			ExitDash();
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
			dashContactTimer -= Time.deltaTime;
			rigidBody.linearVelocity = Vector2.zero;

			if (dashContactTimer <= 0f)
			{
				dashContactTimer = dashContactTimeout;
				dashContact = false;
				slideController.enabled = true;
				dashRefreshed = true;

				// rigidBody.MovePosition(new Vector2(rigidBody.position.x + dashContactWidth, rigidBody.position.y + dashContactHeight));

				if (dashInteractCollider)
				{
					TeleportThroughObject(dashInteractCollider);
				}
				// EnterDash();
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

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (dashing && (other.includeLayers.value & LayerMask.GetMask("DashInteract")) > 0)
		{
			Debug.Log("dash interact");

			dashContact = true;
			slideController.enabled = false;
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

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (dashing && (other.collider.includeLayers.value & LayerMask.GetMask("DashInteract")) > 0)
		{
			Debug.Log("dash interact");

			dashContact = true;
			slideController.enabled = false;
			dashInteractCollider = other.collider;

			if (other.transform.position.x < transform.position.x)
			{
				dashContactWidth = -(float)other.collider.bounds.size.x - (float)boxCollider.bounds.size.x;
			}
			else
			{
				dashContactWidth = (float)other.collider.bounds.size.x + (float)boxCollider.bounds.size.x;
			}

			if (other.transform.position.y < transform.position.y)
			{
				dashContactHeight = -(float)other.collider.bounds.size.y - (float)boxCollider.bounds.size.y;
			}
			else
			{
				dashContactHeight = (float)other.collider.bounds.size.y + (float)boxCollider.bounds.size.y;
			}
		}
	}
}
