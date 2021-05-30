using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float maxSpeed = 20;
	public Projectile projectilePrefab;

	private Rigidbody2D rigidBody;
	private Vector2 moveInput = Vector2.zero;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void Update() {
		var moveX = Input.GetAxisRaw("Horizontal");
		var moveY = Input.GetAxisRaw("Vertical");
		moveInput = new Vector2(moveX, moveY);

		if (Input.GetButtonDown("Fire1")) {
			FireProjectile(aimDirection);
		}
	}

	private void FixedUpdate() {
		rigidBody.velocity = new Vector2(moveInput.x * maxSpeed, moveInput.y * maxSpeed);
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

	private void FireProjectile(Vector2 dir) {
		Projectile proj = Instantiate(projectilePrefab, transform);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), proj.GetComponent<Collider2D>());
		proj.SetVelocity(rigidBody.velocity + dir * 400);
	}
}
