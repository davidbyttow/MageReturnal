using UnityEngine;
using System.Collections;


public class FlyAI : MonoBehaviour {

	public float idleDuration = 1.5f;
	public float dashDuration = 2;
	public float maxTravelDistance = 5;
	public float acceleration = 100;
	public float maxSpeed = 20;

	private BehaviorTree<FlyAI> behaviorTree;
	private Vector3 attackTarget;
	private float stateElapsed = 0;
	private Vector3 movementDirection = Vector3.zero;

	private Rigidbody2D rigidBody;

	private void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		var seq = new SequenceNode<FlyAI>(resetStateTimer, idle, resetStateTimer, new AttackBehavior());
		var root = new LoopNode<FlyAI>(seq);
		behaviorTree = new BehaviorTree<FlyAI>(root);
		behaviorTree.Init(this);
	}

	private void Update() {
		behaviorTree.Update(this);
	}

	private void FixedUpdate() {
		if (movementDirection.sqrMagnitude > 0) {
			rigidBody.AddForce(movementDirection * acceleration);
		}

		if (rigidBody.velocity.magnitude > maxSpeed) {
			rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
		}
	}

	private readonly ActionDelegate<FlyAI> resetStateTimer = (ctx) => {
		ctx.data.stateElapsed = 0;
		return BehaviorNodeStatus.Success;
	};

	private readonly ActionDelegate<FlyAI> idle = (ctx) => {
		ctx.data.movementDirection = Vector3.zero;
		ctx.data.stateElapsed += Time.deltaTime;
		if (ctx.data.stateElapsed > ctx.data.idleDuration) {
			return BehaviorNodeStatus.Success;
		}
		return BehaviorNodeStatus.Running;
	};

	class AttackBehavior : BehaviorNode<FlyAI> {
		public override void Init(BehaviorContext<FlyAI> ctx) {
			ctx.data.attackTarget = Player.inst.transform.position;
		}

		public override BehaviorNodeStatus Update(BehaviorContext<FlyAI> ctx) {
			ctx.data.stateElapsed += Time.deltaTime;
			if (ctx.data.stateElapsed > ctx.data.dashDuration) {
				return BehaviorNodeStatus.Success;
			}
			var toTarget = ctx.data.attackTarget - ctx.data.transform.position;
			var dir = toTarget.normalized;
			ctx.data.movementDirection = dir;
			return BehaviorNodeStatus.Running;
		}
	}

	private readonly ActionDelegate<FlyAI> attack = (ctx) => {
		ctx.data.stateElapsed += Time.deltaTime;
		if (ctx.data.stateElapsed < 2) {
			return BehaviorNodeStatus.Running;
		}
		return BehaviorNodeStatus.Success;
	};
}

