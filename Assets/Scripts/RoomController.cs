using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

	public int gridSize = 6;
	public float roomPadding = 2;

	public Enemy[] enemyPrefabs;
	public Room roomPrefab;
	private Room startRoom;

	private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

	void Awake() {
	}

	void Start() {
		int x = gridSize / 2;
		int y = gridSize / 2;
		Debug.Log($"Start room at {x},{y}");
		startRoom = CreateRoom(new Vector2Int(x, y));
		startRoom.AddToRoom(Player.inst.character);
		RoomCamera.inst.currentRoom = startRoom;

		CreateRoom(new Vector2Int(x + 1, y));
		CreateRoom(new Vector2Int(x + 2, y));
		CreateRoom(new Vector2Int(x, y + 1));
		ConnectRooms();

		if (enemyPrefabs.Length > 0) {
			SpawnEnemy(enemyPrefabs[0], startRoom);
		}

		startRoom.ActivateRoom();
	}

	void ConnectRooms() {
		foreach (var r in rooms) {
			var pos = r.Key;
			if (pos.x == 0 || pos.y == 0 || pos.x + 1 >= gridSize || pos.y + 1 >= gridSize) {
				continue;
			}
			var room = r.Value;
			foreach (var side in Enum.GetValues(typeof(RoomSide))) {
				ConnectRoomIfPresent(room, (RoomSide)side);
			}
		}
	}

	void ConnectRoomIfPresent(Room room, RoomSide side) {
		var dir = RoomSides.Dir(side);
		var otherPos = room.pos + dir;
		var other = FindRoom(otherPos);
		if (other) {
			var offset = new Vector3(
				dir.x * (room.width * 0.5f + other.width * 0.5f + roomPadding),
				dir.y * (room.height * 0.5f + other.height * 0.5f + roomPadding),
				0
			);
			other.transform.position = room.transform.position + offset;
			room.ConnectRooms(side, other);
		}
	}

	Room CreateRoom(Vector2Int pos) {
		var room = Instantiate(roomPrefab);
		room.controller = this;
		room.pos = pos;
		rooms.Add(pos, room);
		return room;
	}

	Room FindRoom(Vector2Int pos) {
		if (!rooms.ContainsKey(pos)) {
			return null;
		}
		return rooms[pos];
	}

	public void MoveToRoom(Room from, RoomSide side) {
		var dir = RoomSides.Dir(side);
		var dstPos = from.pos + dir;
		var room = FindRoom(dstPos);
		var otherSide = RoomSides.Opposite(side);
		room.EnterRoom(Player.inst, otherSide);
		from.DeactivateRoom();
		room.ActivateRoom();
		RoomCamera.inst.currentRoom = room;
	}

	public void SpawnEnemy(Enemy enemy, Room room) {
		enemy = Instantiate(enemy);
		enemy.transform.position = room.transform.position + new Vector3(3, 3, 0);
		room.AddToRoom(enemy.character);
	}
}
