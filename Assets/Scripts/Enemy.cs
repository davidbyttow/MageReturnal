using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Character character { get; private set; }

	void Awake() {
		character = GetComponent<Character>();
	}

	void Update() {

	}
}
