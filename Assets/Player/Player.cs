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
	private bool dashing;
	private bool recovering;

	private InputAction dashAction;

	// private CharacterController2D controllerRef;

	// Start is called before the first frame update
	void Start()
	{
		// controllerRef = GetComponent<CharacterController2D>();
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
		recovering = true;
	}

	void ExitDash()
	{
		rigidBody.gravityScale = gravityScale;
		slideController.SetJumpEnabled(true);
		slideController.SetSlideEnabled(true);
		rigidBody.linearVelocity = Vector2.zero;
		dashing = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var delta = Time.deltaTime;

		if (dashAction.ReadValue<float>() > 0 && dashRecovery <= 0)
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

		// if (dashRecovery <= 0 && dashing)
		// {
		// 	recovering = false;
		// }
	}
}
