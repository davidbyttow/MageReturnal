using UnityEngine;
using System.Collections;

public class Projectile : Entity {

	private Rigidbody2D rigidBody;

	internal Vector2 velocity;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		rigidBody.velocity = velocity;
	}

	private void OnCollisionEnter2D(Collision2D target) {
		Debug.Log("collision enter");
		Destroy(gameObject);
	}
}
