using UnityEngine;
using System.Collections;

public class RoomCamera : MonoBehaviour {

	public static RoomCamera inst;

	public Room currentRoom;
	public float roomChangeSpeed = 500;

	private void Awake() {
		inst = this;
	}

	void Start() {

	}

	void Update() {
		var roomCenter = currentRoom.roomCenter;
		var targetPos = new Vector3(roomCenter.x, roomCenter.y, transform.position.z);
		transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * roomChangeSpeed);
	}
}
