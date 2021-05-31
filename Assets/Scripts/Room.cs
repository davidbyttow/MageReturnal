using UnityEngine;


public class Room : MonoBehaviour {

	public int width;
	public int height;
	public Door leftDoor;
	public Door rightDoor;

	void Start() {

	}

	void Update() {

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
