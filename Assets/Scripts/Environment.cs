using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {

	public EnvironmentVariables variables;

	private void Awake() {
		Debug.Assert(inst == null, "only one instance allowed per scene");
		inst = this;
	}

	public static Environment inst { get; private set; }
}
