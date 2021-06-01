using UnityEngine;
using System;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

	public int gridSize = 9;
	public float roomPadding = 2;

	public Enemy[] enemyPrefabs;
	public Room roomPrefab;
	public Room bossRoomPrefab;
	private Room startRoom;

	private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

	void Awake() {
	}

	void Start() {
		Room startRoom = null;
		var cells = PCG.Generate(DateTime.UtcNow.Millisecond, gridSize, gridSize);
		for (var y = gridSize - 1; y >= 0; --y) {
			for (var x = 0; x < gridSize; ++x) {
				var cell = cells[x, y];
				if (cell == '#') {
					continue;
				}

				Room room;
				if (cell == 'B') {
					room = CreateRoom(new Vector2Int(x, y), bossRoomPrefab);
				} else {
					room = CreateRoom(new Vector2Int(x, y), roomPrefab);
				}

				if (cell == 'S') {
					startRoom = room;
				}

				var offset = new Vector3(
					x * roomPrefab.width + roomPadding * (x - 1),
					y * roomPrefab.height + roomPadding * (y - 1),
					0
				);
				room.transform.position = offset;
			}
		}

		//CreateRoom(new Vector2Int(x + 1, y));
		//CreateRoom(new Vector2Int(x + 2, y));
		//CreateRoom(new Vector2Int(x, y + 1));
		ConnectRooms();

		//if (enemyPrefabs.Length > 0) {
		//	SpawnEnemy(enemyPrefabs[0], startRoom);
		//}

		Player.inst.transform.position = startRoom.transform.position;
		startRoom.AddToRoom(Player.inst.character);
		RoomCamera.inst.currentRoom = startRoom;
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
			room.ConnectRooms(side, other);
		}
	}

	Room CreateRoom(Vector2Int pos, Room roomPrefab) {
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
