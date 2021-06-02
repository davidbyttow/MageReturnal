using UnityEngine;
using System.Collections;

public class Projectile : Entity {

	public int damage = 10;

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

		var character = target.gameObject.GetComponent<Character>();
		if (character) {
			Debug.Log("hit character");
			character.OnProjectileHit(this, target);
		}
		Destroy(gameObject);
	}
}
