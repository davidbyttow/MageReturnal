using UnityEngine;


public class Door : MonoBehaviour {

	public RoomSide side;

	internal Room room;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			room.ExitDoor(side);
		}
	}
}
