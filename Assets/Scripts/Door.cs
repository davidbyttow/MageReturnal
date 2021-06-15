using UnityEngine;


public class Door : MonoBehaviour {

	public RoomSide side;

	internal Room room;

	private SpriteRenderer spriteRenderer;
	private Collider2D coll;
	private bool locked = false;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		coll = GetComponent<Collider2D>();
	}

	public void Lock() {
		locked = false;
		spriteRenderer.color = Color.red;
		coll.isTrigger = false;
	}

	public void Unlock() {
		locked = true;
		spriteRenderer.color = Color.green;
		coll.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			room.ExitDoor(side);
		}
	}
}
