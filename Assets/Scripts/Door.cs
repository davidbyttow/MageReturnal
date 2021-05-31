using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public enum Side {
		Top,
		Right,
		Bottom,
		Left,
	}

	public Side side;

	// TODO: Remove this and use grid instead
	public Door otherSide;

	private GameObject player;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update() {

	}

	private void OnTriggerEnter2D(Collider2D other) {
		// TODO: Move this logic to RoomController

		var offset = 2f;
		if (other.tag == "Player") {
			switch (side) {
				case Side.Right:
					player.transform.position = new Vector2(otherSide.transform.position.x + offset, player.transform.position.y);
					break;
				case Side.Left:
					player.transform.position = new Vector2(otherSide.transform.position.x - offset, player.transform.position.y);
					break;
			}
			if (otherSide) {
				var room = otherSide.GetComponentInParent<Room>();
				if (room) {
					RoomCamera.inst.currentRoom = room;
				}
			}
		}
	}
}
