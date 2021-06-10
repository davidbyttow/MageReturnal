using UnityEngine;
using System.Collections;

public class Enemy : Entity {

	public Character character { get; private set; }

	void Awake() {
		character = GetComponent<Character>();
	}

	void Update() {

	}

	public override void OnProjectileHit(ProjectileHit hit) {
	}
}
