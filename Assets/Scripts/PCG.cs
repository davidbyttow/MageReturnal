using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class PCG {

	public enum RoomType {
		Start,
		Normal,
		Boss,
		Special,
	}

	public class RoomDef {
		public RoomType type = RoomType.Normal;
	}

	private static Vector2Int[] dirs = new Vector2Int[4] {
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
	};

	private static int CountRooms(char[,] cells, int width, int height) {
		var n = 0;
		for (var i = 0; i < width; ++i) {
			for (var j = 0; j < height; ++j) {
				if (cells[i, j] != '#') {
					n++;
				}
			}
		}
		return n;
	}

	public static char[,] Generate(int seed, int width, int height) {
		System.Random rng = new System.Random(seed);
		int desiredRoomCount = 8;
		int roomCount = 0;
		int iters = 0;
		char[,] cells = null;
		while (roomCount < desiredRoomCount) {
			Debug.Assert(iters++ < 10, $"failed to generate a map with at least {desiredRoomCount} rooms");
			cells = TryGenerate(rng, width, height);
			roomCount = CountRooms(cells, width, height);
		}
		return cells;
	}

	private static char[,] TryGenerate(System.Random rng, int width, int height) {
		var maxRooms = 10;
		var cells = new char[width, height];
		for (var i = 0; i < width; ++i) {
			for (var j = 0; j < height; ++j) {
				cells[i, j] = '#';
			}
		}

		var startLoc = new Vector2Int(width / 2, height / 2);
		var queue = new Queue<Vector2Int>();
		var roomCount = 0;

		var endRooms = new List<Vector2Int>();
		queue.Enqueue(startLoc);

		Func<string> printCells = delegate () {
			var str = "";
			for (var y = 0; y < height; ++y) {
				for (var x = 0; x < width; ++x) {
					str += cells[x, y];
				}
				str += '\n';
			}
			return str;
		};

		Func<Vector2Int, int> neighbors = delegate (Vector2Int loc) {
			int n = 0;
			foreach (var dir in dirs) {
				var neighbor = loc + dir;
				if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= width || neighbor.y >= height) {
					continue;
				}
				if (cells[neighbor.x, neighbor.y] != '#') {
					n++;
				}
			}
			return n;
		};

		Func<Vector2Int, bool> visit = delegate (Vector2Int loc) {
			if (roomCount >= maxRooms) {
				return false;
			}
			if (loc.x < 0 || loc.y < 0 || loc.x >= width || loc.y >= height) {
				return false;
			}
			if (cells[loc.x, loc.y] != '#') {
				return false;
			}
			if (neighbors(loc) > 1) {
				return false;
			}
			if (loc != startLoc && rng.NextDouble() < 0.5f) {
				return false;
			}
			cells[loc.x, loc.y] = '0';
			queue.Enqueue(loc);
			roomCount++;
			return true;
		};

		visit(startLoc);

		while (queue.Count > 0) {
			var loc = queue.Dequeue();
			var shuffled = Enumerable.OrderBy(dirs, c => rng.Next()).ToArray();
			var roomCreated = false;
			foreach (var dir in shuffled) {
				var neighbor = loc + dir;
				roomCreated |= visit(neighbor);
			}
			if (!roomCreated) {
				endRooms.Add(loc);
			}
		}

		cells[startLoc.x, startLoc.y] = 'S';
		Debug.Assert(endRooms.Count > 0);

		var bossLoc= endRooms.Last();
		cells[bossLoc.x, bossLoc.y] = 'B';
		endRooms.RemoveAt(endRooms.Count - 1);

		Debug.Log($"After:\n{printCells()}");

		return cells;
	}

}
