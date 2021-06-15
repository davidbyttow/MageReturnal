using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Character character { get; private set; }

	void Awake() {
		character = GetComponent<Character>();
		var material = GetComponent<Renderer>().material;
		material.SetFloat("_HitEffectBlend", 0.1f);
	}

	void Update() {

	}
}
