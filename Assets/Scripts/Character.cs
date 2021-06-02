using UnityEngine;
using System.Collections;

public class Character : Entity {

	public int maxHealth = 50;
	private int health = 50;

	internal Vector2 velocity;

	private Rigidbody2D rigidBody;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		health = maxHealth;
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

	private void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
			Destroy(gameObject);
		}
	}

	public void OnProjectileHit(Projectile proj, Collision2D collision) {
		TakeDamage(proj.damage);
	}
}
