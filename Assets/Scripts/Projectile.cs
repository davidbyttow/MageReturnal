using UnityEngine;
using System.Collections;

public struct ProjectileHit {
	public Projectile projectile;
	public Collision2D collision;
	public bool ignoreCollision;
}

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

		var hit = new ProjectileHit();
		hit.projectile = this;
		hit.collision = target;

		var entity = target.collider.GetComponent<Entity>();
		if (entity) {
			entity.OnProjectileHit(hit);
		}

		if (!hit.ignoreCollision) {
			Destroy(gameObject);
		}
	}
}
