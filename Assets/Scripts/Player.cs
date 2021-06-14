using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static Player inst;

	public Projectile projectilePrefab;

	public Character character { get; private set; }
	private Rigidbody2D rigidBody;
	private Animator animator;
	private Vector2 movementDirection = Vector2.zero;
	private Vector2 fireDirection = Vector2.zero;
	private float lastFireTime;
	private float movementSpeed = 0;
	private Vector2 dashDirection = Vector2.zero;

	private bool isDashing { get { return dashDirection != Vector2.zero;  } }

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
	}

	void FixedUpdate() {
		UpdateMovement();	
	}

	void LateUpdate() {
		Animate();
	}

	void ProcessInput() {
		if (!isDashing) {
			var moveX = Input.GetAxisRaw("Horizontal");
			var moveY = Input.GetAxisRaw("Vertical");
			movementDirection = new Vector2(moveX, moveY);
			movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0f, 1f);
			movementDirection.Normalize();
		}

		if (!isDashing && Input.GetButtonDown("Fire1")) {
			fireDirection = aimDirection;
			lastFireTime = Time.realtimeSinceStartup;
			character.FireProjectile(projectilePrefab, aimDirection);
		}

		if (Input.GetButtonDown("Jump")) {
			Dash();
		}

		if (Time.realtimeSinceStartup - lastFireTime > 1f) {
			lastFireTime = 0;
			fireDirection = Vector2.zero;
		}
	}

	void UpdateMovement() {
		var vars = Environment.inst.variables;

		Vector2 targetVelocity;
		if (isDashing) {
			targetVelocity = dashDirection * vars.playerSpeed * 3;
		} else {
			targetVelocity = movementDirection * vars.playerSpeed;
		}

		var diff = rigidBody.velocity.Delta(targetVelocity, vars.playerSpeed);
		rigidBody.velocity += diff;
	}

	void Animate() {
		if (fireDirection.sqrMagnitude > 0) {
			animator.SetFloat("Horizontal", fireDirection.x);
			animator.SetFloat("Vertical", fireDirection.y);
		} else if (movementDirection.sqrMagnitude > 0) {
			animator.SetFloat("Horizontal", movementDirection.x);
			animator.SetFloat("Vertical", movementDirection.y);
		}
		animator.SetFloat("Speed", character.rigidBody.velocity.sqrMagnitude);
	}

	void Dash() {
		StartCoroutine(DashAsync());
	}

	IEnumerator DashAsync() {
		dashDirection = aimDirection;

		yield return new WaitForSeconds(0.2f);

		dashDirection = Vector2.zero;
	}
}
