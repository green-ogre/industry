using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	private CharacterController2D controllerRef;

	// Start is called before the first frame update
	void Start()
	{
		controllerRef = GetComponent<CharacterController2D>();
	}

	// Update is called once per frame
	void Update()
	{
		float horz = Input.GetAxisRaw("Horizontal");
		bool jump = Input.GetAxis("Jump") > 0;

		controllerRef.Move(horz, jump);
	}
}
