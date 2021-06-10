using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

	internal Room currentRoom;

	public virtual void OnProjectileHit(ProjectileHit hit) { }
}
