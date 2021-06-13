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

		//rigidBody.AddForce(velocity, ForceMode2D.Impulse);
//		rigidBody.velocity = velocity;
	}

	void FixedUpdate() {
		//rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
		
	}

	private void LateUpdate() {
//		velocity = rigidBody.velocity;
	}

	public void FireProjectile(Projectile projectile, Vector2 dir) {
		Projectile proj = Instantiate(projectile, transform);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), proj.GetComponent<Collider2D>());
		proj.velocity = dir * 10;
		if (currentRoom) {
			currentRoom.AddToRoom(proj);
		}
	}

	private void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
			Destroy(gameObject);
		}
	}

	public override void OnProjectileHit(ProjectileHit hit) {
		//TakeDamage(hit.projectile.damage);
	}
}
