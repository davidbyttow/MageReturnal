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
	private Vector2 moveInput = Vector2.zero;

	void Awake() {
		inst = this;
		character = GetComponent<Character>();
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update() {
		var moveX = Input.GetAxisRaw("Horizontal");
		var moveY = Input.GetAxisRaw("Vertical");
		moveInput = new Vector2(moveX, moveY);
		character.velocity = new Vector2(moveInput.x * maxSpeed, moveInput.y * maxSpeed);

		if (moveX != 0 || moveY != 0) {
			animator.Play("RedMage_Walk_F");
		} else {
			animator.Play("RedMage_Idle_F");
		}

		if (Input.GetButtonDown("Fire1")) {
			character.FireProjectile(projectilePrefab, aimDirection);
		}
	}

	private Vector2 aimDirection {
		get {
			var screenPos = Camera.main.WorldToScreenPoint(transform.position);
			var cursorPos = Input.mousePosition;
			var dir = (cursorPos - screenPos).normalized;
			var dir2d = new Vector2(dir.x, dir.y);
			return dir2d;
		}
	}
}
