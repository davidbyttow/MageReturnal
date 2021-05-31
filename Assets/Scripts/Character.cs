using UnityEngine;
using System.Collections;

public class Character : Entity {

	internal Vector2 velocity;

	private Rigidbody2D rigidBody;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		rigidBody.velocity = velocity;
	}

	public void FireProjectile(Projectile projectile, Vector2 dir) {
		Projectile proj = Instantiate(projectile, transform);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), proj.GetComponent<Collider2D>());
		proj.velocity = dir * 10;
		currentRoom.AddToRoom(proj);
	}
}
