using UnityEngine;
using System.Collections;

public class Character : Entity {

	public int maxHealth = 50;
	private int health = 50;

	internal Rigidbody2D rigidBody;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		health = maxHealth;
	}

	void Update() {
	}

	void FixedUpdate() {
		
	}

	private void LateUpdate() {
	}

	public Projectile FireProjectile(Projectile projectile, Vector2 dir) {
		Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), proj.GetComponent<Collider2D>());
		proj.rigidBody.velocity = dir * 10;
		if (currentRoom) {
			currentRoom.AddToRoom(proj);
		}
		return proj;
	}

	private void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
			Destroy(gameObject);
		}
	}

	public override void OnProjectileHit(ProjectileHit hit) {
		TakeDamage(hit.projectile.damage);
	}
}
