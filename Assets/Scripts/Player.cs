using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float maxSpeed = 20;

	private Rigidbody2D rigidBody;
	private Vector2 moveInput = Vector2.zero;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void Update() {
		var moveX = Input.GetAxisRaw("Horizontal");
		var moveY = Input.GetAxisRaw("Vertical");
		moveInput = new Vector2(moveX, moveY);

	}

	private void FixedUpdate() {
		rigidBody.velocity = new Vector2(moveInput.x * maxSpeed, moveInput.y * maxSpeed);
	}
}
