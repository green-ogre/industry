using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] private float DashSpeed = 15f;
	[SerializeField] private float DashTime = 0.1f;
	[SerializeField] private float DashRecoveryTime = 0.25f;

	private float dashTimer = 0;
	private float dashRecovery = 0;
	private SlideController slideController;
	private Rigidbody2D rigidBody;
	private float gravityScale;
	public bool dashing;
	private bool dashPressed = false;

	private InputAction dashAction;
	public Animator animator;


	// Start is called before the first frame update
	void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		dashAction = InputSystem.actions.FindAction("Dash");
		slideController = GetComponent<SlideController>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void EnterDash()
	{
		gravityScale = rigidBody.gravityScale;
		rigidBody.gravityScale = 0;
		slideController.SetJumpEnabled(false);
		slideController.SetSlideEnabled(false);
		dashing = true;
		dashPressed = true;
	}

	void ExitDash()
	{
		rigidBody.gravityScale = gravityScale;
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

		// controllerRef.Move(horz, jump);
		bool isGrounded = Mathf.Abs(rigidBody.linearVelocityY) < 1e-10;
		animator.SetBool("isFalling", !isGrounded);
		float x = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal"));
		animator.SetBool("isRunning", Mathf.Abs(x) > 1e-10 && isGrounded);

		if (dashInput && dashRecovery <= 0 && !dashPressed)
		{
			dashTimer = DashTime;
			dashRecovery = DashTime + DashRecoveryTime;

			EnterDash();
		}

		if (dashTimer > 0)
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

	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "hook")
		{
			Debug.Log("this is a hook");
		}
	}
}
