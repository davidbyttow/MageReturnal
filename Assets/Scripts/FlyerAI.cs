using UnityEngine;


public class FlyerAI : MonoBehaviour {

	public float idleDuration = 1.5f;
	public float speed = 10;

	private Rigidbody2D rigidBody;

	private BehaviorTree<FlyerAI> behaviorTree;
	private float stateElapsed = 0;

	private Vector3 attackTarget;
	private float dashDuration = 2;
	private Vector2 movementDirection = Vector2.zero;


	private void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		var seq = new SequenceNode<FlyerAI>(resetStateTimer, idle, resetStateTimer, new AttackBehavior());
		var root = new LoopNode<FlyerAI>(seq);
		behaviorTree = new BehaviorTree<FlyerAI>(root);
		behaviorTree.Init(this);
	}

	private void Update() {
		behaviorTree.Update(this);
	}

	private void FixedUpdate() {
		var targetVelocity = movementDirection * speed;
		var diff = rigidBody.velocity.Delta(targetVelocity, speed);
		rigidBody.velocity += diff;
	}

	private readonly ActionDelegate<FlyerAI> resetStateTimer = (ctx) => {
		ctx.data.stateElapsed = 0;
		return BehaviorNodeStatus.Success;
	};

	private readonly ActionDelegate<FlyerAI> idle = (ctx) => {
		ctx.data.movementDirection = Vector3.zero;
		ctx.data.stateElapsed += Time.deltaTime;
		if (ctx.data.stateElapsed > ctx.data.idleDuration) {
			return BehaviorNodeStatus.Success;
		}
		return BehaviorNodeStatus.Running;
	};

	class AttackBehavior : BehaviorNode<FlyerAI> {
		public override void Init(BehaviorContext<FlyerAI> ctx) {
			ctx.data.attackTarget = Player.inst.transform.position;
		}

		public override BehaviorNodeStatus Update(BehaviorContext<FlyerAI> ctx) {
			ctx.data.stateElapsed += Time.deltaTime;
			if (ctx.data.stateElapsed > ctx.data.dashDuration) {
				return BehaviorNodeStatus.Success;
			}
			var toTarget = ctx.data.attackTarget - ctx.data.transform.position;
			if (toTarget.magnitude < 0.5f) {
				return BehaviorNodeStatus.Success;
			}
			ctx.data.movementDirection = toTarget.normalized;
			return BehaviorNodeStatus.Running;
		}
	}

	private readonly ActionDelegate<FlyerAI> attack = (ctx) => {
		ctx.data.stateElapsed += Time.deltaTime;
		if (ctx.data.stateElapsed < 2) {
			return BehaviorNodeStatus.Running;
		}
		return BehaviorNodeStatus.Success;
	};
}

