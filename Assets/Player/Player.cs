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

	private float dashTimer = 0;
	private float dashRecovery = 0;

	private float gravityScale;
	public bool dashing;
	private bool dashPressed = false;

	private bool dashContact = false;
	public float dashContactTimeout = 0.1f;
	private float dashContactWidth;
	private float dashContactTimer;

	private bool dashRefreshed = false;

	private SlideController slideController;
	private Rigidbody2D rigidBody;
	private Collider2D boxCollider;
	private InputAction dashAction;
	public Animator animator;


	// Start is called before the first frame update
	void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		dashAction = InputSystem.actions.FindAction("Dash");
		slideController = GetComponent<SlideController>();
		rigidBody = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<Collider2D>();

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
	}

	void ExitDash()
	{
		// rigidBody.gravityScale = gravityScale;
		rigidBody.gravityScale = 1f;
		slideController.SetJumpEnabled(true);
		slideController.SetSlideEnabled(true);
		rigidBody.linearVelocity = Vector2.zero;
		// rigidBody.linearVelocity = new Vector2(slideController.lastVelocity.x, 0);
		dashing = false;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		var delta = Time.deltaTime;
		var dashInput = dashAction.ReadValue<float>() > 0;

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
			rigidBody.linearVelocity = slideController.lastDirection.normalized * DashSpeed;
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
			rigidBody.linearVelocityX = 0f;

			if (dashContactTimer <= 0f)
			{
				dashContactTimer = dashContactTimeout;
				dashContact = false;
				slideController.enabled = true;
				dashRefreshed = true;

				rigidBody.MovePosition(new Vector2(rigidBody.position.x + dashContactWidth, rigidBody.position.y));
				// EnterDash();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (dashing && (other.includeLayers.value & LayerMask.GetMask("DashInteract")) > 0)
		{
			Debug.Log("dash interact");

			dashContact = true;
			slideController.enabled = false;

			if (other.transform.position.x < transform.position.x)
			{
				dashContactWidth = -(float)other.bounds.size.x - (float)boxCollider.bounds.size.x;
			}
			else
			{
				dashContactWidth = (float)other.bounds.size.x + (float)boxCollider.bounds.size.x;
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

			if (other.transform.position.x < transform.position.x)
			{
				dashContactWidth = -(float)other.collider.bounds.size.x - (float)boxCollider.bounds.size.x;
			}
			else
			{
				dashContactWidth = (float)other.collider.bounds.size.x + (float)boxCollider.bounds.size.x;
			}
		}
	}
}
