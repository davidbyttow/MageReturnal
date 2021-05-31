using UnityEngine;
using System.Collections;

public class RoomController : MonoBehaviour {

	public Room roomPrefab;
	private Room startRoom;

	void Awake() {
	}

	void Start() {
		startRoom = Instantiate(roomPrefab);
		RoomCamera.inst.currentRoom = startRoom;
		AddRoom(startRoom, startRoom.rightDoor);
	}

	void Update() {

	}

	void AddRoom(Room src, Door srcDoor) {
		var padding = 2;
		switch (srcDoor.side) {
			case Door.Side.Right:
				var room = Instantiate(roomPrefab);
				room.transform.position = new Vector2(src.transform.position.x + src.width * 0.5f + padding + room.width * 0.5f, 0);
				room.leftDoor.otherSide = srcDoor;
				srcDoor.otherSide = room.leftDoor;
				break;
			default:
				Debug.LogFormat("unknown side {0}", srcDoor.side);
				return;
		}
	}

	void CreateRoom(Room prefab, int x, int y) {
	}
}
