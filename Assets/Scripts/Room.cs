using UnityEngine;


public enum RoomSide {
	Top,
	Right,
	Bottom,
	Left,
}

public static class RoomSides {
	public static RoomSide Opposite(RoomSide side) {
		switch (side) {
			case RoomSide.Top: return RoomSide.Bottom;
			case RoomSide.Right: return RoomSide.Left;
			case RoomSide.Left: return RoomSide.Right;
			case RoomSide.Bottom: return RoomSide.Top;
		}
		Debug.AssertFormat(false, "Unknown side {0}", side);
		return RoomSide.Right;
	}

	public static Vector2Int Dir(RoomSide side) {
		switch (side) {
			case RoomSide.Top: return new Vector2Int(0, 1);
			case RoomSide.Right: return new Vector2Int(1, 0);
			case RoomSide.Left: return new Vector2Int(-1, 0);
			case RoomSide.Bottom: return new Vector2Int(0, -1);
		}
		Debug.AssertFormat(false, "Unknown side {0}", side);
		return Vector2Int.zero;
	}
}

public class Room : MonoBehaviour {

	public int width;
	public int height;
	public Door[] doors;

	internal Vector2Int pos;
	internal RoomController controller;

	void Awake() {
		foreach (var door in doors) {
			door.room = this;
			door.gameObject.SetActive(false);
		}
	}

	Entity[] GetEntitiesInRoom() {
		return GetComponentsInChildren<Entity>(true);
	}

	public void ConnectRooms(RoomSide side, Room other) {
		var srcDoor = GetDoor(side);
		var dstDoor = other.GetDoor(RoomSides.Opposite(side));
		srcDoor.gameObject.SetActive(true);
		dstDoor.gameObject.SetActive(true);
	}

	public void ActivateRoom() {
		gameObject.SetActive(true);
	}

	public void DeactivateRoom() {
		gameObject.SetActive(false);
	}

	public void ExitDoor(RoomSide side) {
		controller.MoveToRoom(this, side);
	}

	public void EnterRoom(Player player, RoomSide side) {
		var doorOffset = 1.5f;
		var door = GetDoor(side);
		var dir = RoomSides.Dir(side);
		var offset = new Vector3(
			-dir.x * doorOffset,
			-dir.y * doorOffset,
			0
		);
		player.transform.position = door.transform.position + offset;
		AddToRoom(player.character);
	}

	public void AddToRoom(Entity entity) {
		entity.currentRoom = this;
		entity.transform.SetParent(transform, true);
	}

	Door FindDoor(RoomSide side) {
		foreach (var d in doors) {
			if (d.side == side) {
				return d;
			}
		}
		return null;
	}

	Door GetDoor(RoomSide side) {
		var door = FindDoor(side);
		if (!door) {
			Debug.LogWarningFormat("Could not find door for side {0}", side);
		}
		return door;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
	}

	public Vector2 roomCenter {
		get {
			return transform.position;
		}
	}
}
