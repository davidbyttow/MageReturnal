using UnityEngine;
using System.Collections;

public class RoomCamera : MonoBehaviour {

	public static RoomCamera inst;

	private Room currentRoom;
	public float roomChangeSpeed = 100;

	private void Awake() {
		inst = this;
	}

	public void UpdateRoom(Room room) {
		if (!currentRoom) {
			var roomCenter = room.roomCenter;
			transform.position = new Vector3(roomCenter.x, roomCenter.y, transform.position.z);
		}
		currentRoom = room;
	}

	void Update() {
		if (currentRoom) {
			var roomCenter = currentRoom.roomCenter;
			var targetPos = new Vector3(roomCenter.x, roomCenter.y, transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * roomChangeSpeed);
		}
	}
}
