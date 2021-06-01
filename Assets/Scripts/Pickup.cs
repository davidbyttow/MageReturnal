using UnityEngine;
using System.Collections;

public class Pickup : Entity {

	void Start() {

	}

	void Update() {

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			Destroy(gameObject);
		}
	}
}
