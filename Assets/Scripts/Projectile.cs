using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Rigidbody2D rigidBody;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	public void SetVelocity(Vector2 velocity) {
		rigidBody.AddForce(velocity);
	}

	void Update() {
		
	}

	private void OnCollisionEnter2D(Collision2D target) {
		Debug.Log("collision enter");
		Destroy(gameObject);
	}
}
