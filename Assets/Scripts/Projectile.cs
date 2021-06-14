using UnityEngine;
using System.Collections;

public struct ProjectileHit {
	public Projectile projectile;
	public Collision2D collision;
	public bool ignoreCollision;
}

public class Projectile : Entity {

	public int damage = 10;

	internal Rigidbody2D rigidBody;

	private SpriteRenderer spriteRenderer;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
	}

	private void OnCollisionEnter2D(Collision2D target) {
		var hit = new ProjectileHit();
		hit.projectile = this;
		hit.collision = target;

		var entity = target.collider.GetComponent<Entity>();
		if (entity) {
			entity.OnProjectileHit(hit);

			if (hit.collision.rigidbody != null) {
				hit.collision.rigidbody.AddForce(transform.DirectionTo(hit.collision.collider.transform) * 100);
			}
		}

		if (!hit.ignoreCollision) {
			StartCoroutine(StopProjectile());
		}
	}

	IEnumerator StopProjectile() {
		GetComponent<Collider2D>().isTrigger = true;
		rigidBody.velocity = Vector2.zero;
		rigidBody.isKinematic = true;
		spriteRenderer.forceRenderingOff = true;

		var systems = GetComponentsInChildren<ParticleSystem>();
		foreach (var ps in systems) {
			ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}

		var trails = GetComponentsInChildren<TrailRenderer>();
		foreach (var trail in trails) {
			trail.emitting = false;
			trail.time = 0.2f;
		}

		var done = false;
		while (!done) {
			var hasParticles = false;
			foreach (var ps in systems) {
				if (ps.particleCount > 0) {
					hasParticles = true;
					break;
				}
			}
			done = !hasParticles;
			yield return null;
		}

		Destroy(gameObject);
		yield return null;
	}
}
