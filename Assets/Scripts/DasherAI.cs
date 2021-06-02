using UnityEngine;
using System.Collections;

public class DasherAI : MonoBehaviour {

	private Character character;
	private float dashTimer;
	private bool dashing;
	private Vector3 target;

	public float waitTime = 1.5f;
	public float speed = 4;
	public float maxTravelDistance = 5;

	void Awake() {
		character = GetComponent<Character>();
		dashTimer = waitTime;
	}

	void Start() {

	}

	void Update() {
		if (dashTimer > 0) {
			dashTimer -= Time.deltaTime;
			if (dashTimer <= 0) {
				if (dashing) {
					character.velocity = Vector2.zero;
					dashTimer = waitTime;
					dashing = false;
				} else {
					target = Player.inst.transform.position;
					dashTimer = 3;
					var toTarget = Player.inst.transform.position - transform.position;
					var dir = toTarget.normalized;
					var dist = Mathf.Min(toTarget.magnitude, maxTravelDistance);
					dashTimer = dist / speed;
					character.velocity = dir * speed;
					dashing = true;
				}
			}
		}
	}
}
