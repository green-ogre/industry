using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	// private CharacterController2D controllerRef;

	public Animator animator;
	private Rigidbody2D rigidBody;

	// Start is called before the first frame update
	void Start()
	{
		// controllerRef = GetComponent<CharacterController2D>();
		// animator.SetFloat("speed", 2.0);
        rigidBody = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		// float horz = Input.GetAxisRaw("Horizontal");
		// bool jump = Input.GetAxis("Jump") > 0;

		// controllerRef.Move(horz, jump);
		bool isGrounded = Mathf.Abs(rigidBody.linearVelocityY) < 1e-10;
		animator.SetBool("isFalling", !isGrounded);
		float x = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal"));
		animator.SetBool("isRunning", Mathf.Abs(x) > 1e-10 && isGrounded);
	}
}
