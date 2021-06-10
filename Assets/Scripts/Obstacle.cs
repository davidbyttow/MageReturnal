using UnityEngine;
using System.Collections;

public class Obstacle : Entity {

	void Start() {

	}

	void Update() {

	}

	public override void OnProjectileHit(ProjectileHit hit) {
		hit.ignoreCollision = true;
	}
}
