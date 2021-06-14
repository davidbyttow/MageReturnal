using UnityEngine;
using System.Collections;


public class SkeletonAI : MonoBehaviour {

	public float idleDuration = 4;
	public float speed = 10;
	public Projectile projectilePrefab;

	private Rigidbody2D rigidBody;
	private Animator animator;
	private Character character;

	private BehaviorTree<SkeletonAI> behaviorTree;
	private float stateElapsed = 0;

	private Vector3 attackTarget;
	private Vector2 movementDirection = Vector2.zero;

	private void Awake() {
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody2D>();
		character = GetComponent<Character>();

		var seq = new SequenceNode<SkeletonAI>(resetStateTimer, new IdleBehavior(), resetStateTimer, new AttackBehavior());
		var root = new LoopNode<SkeletonAI>(seq);
		behaviorTree = new BehaviorTree<SkeletonAI>(root);
		behaviorTree.Init(this);
	}

	private void Update() {
		behaviorTree.Update(this);
	}

	public void Anim_ThrowSkull() {
		var toTarget = attackTarget - transform.position;
		var proj = character.FireProjectile(projectilePrefab, toTarget.normalized);
		proj.rigidBody.angularVelocity = 720f;
	}

	private void FixedUpdate() {
		var targetVelocity = movementDirection * speed;
		var diff = rigidBody.velocity.Delta(targetVelocity, speed);
		rigidBody.velocity += diff;
	}

	private readonly ActionDelegate<SkeletonAI> resetStateTimer = (ctx) => {
		ctx.data.stateElapsed = 0;
		return BehaviorNodeStatus.Success;
	};

	class IdleBehavior : BehaviorNode<SkeletonAI> {
		public override void Init(BehaviorContext<SkeletonAI> ctx) {}

		public override BehaviorNodeStatus Update(BehaviorContext<SkeletonAI> ctx) {
			ctx.data.movementDirection = Vector3.zero;
			ctx.data.stateElapsed += Time.deltaTime;
			if (ctx.data.stateElapsed > ctx.data.idleDuration) {
				return BehaviorNodeStatus.Success;
			}
			return BehaviorNodeStatus.Running;
		}
	}


	class AttackBehavior : BehaviorNode<SkeletonAI> {
		public override void Init(BehaviorContext<SkeletonAI> ctx) {
			ctx.data.attackTarget = Player.inst.transform.position;
			ctx.data.animator.SetTrigger("Fire");
		}

		public override BehaviorNodeStatus Update(BehaviorContext<SkeletonAI> ctx) {
			ctx.data.stateElapsed += Time.deltaTime;

			if (ctx.data.stateElapsed > ctx.data.idleDuration) {
				return BehaviorNodeStatus.Success;
			}

			return BehaviorNodeStatus.Running;
		}
	}
}
