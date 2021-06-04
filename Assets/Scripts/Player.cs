using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static Player inst;

	public float maxSpeed = 20;
	public Projectile projectilePrefab;

	public Character character { get; private set; }
	private Rigidbody2D rigidBody;
	private Animator animator;
	private Vector2 movementDirection = Vector2.zero;
	private float movementSpeed = 0;

	private Vector2 aimDirection {
		get {
			var screenPos = Camera.main.WorldToScreenPoint(transform.position);
			var cursorPos = Input.mousePosition;
			var dir = (cursorPos - screenPos).normalized;
			var dir2d = new Vector2(dir.x, dir.y);
			return dir2d;
		}
	}

	void Awake() {
		inst = this;
		character = GetComponent<Character>();
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update() {
		ProcessInput();
		UpdateMovement();
		Animate();
	}

	void ProcessInput() {
		var moveX = Input.GetAxisRaw("Horizontal");
		var moveY = Input.GetAxisRaw("Vertical");
		movementDirection = new Vector2(moveX, moveY);
		movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0f, 1f);
		movementDirection.Normalize();

		if (Input.GetButtonDown("Fire1")) {
			character.FireProjectile(projectilePrefab, aimDirection);
		}
	}

	void UpdateMovement() {
		character.velocity = movementDirection * movementSpeed * maxSpeed;
	}

	void Animate() {
		animator.SetFloat("Horizontal", movementDirection.x);
		animator.SetFloat("Vertical", movementDirection.y);
		animator.SetFloat("Speed", character.velocity.sqrMagnitude);
	}
}
