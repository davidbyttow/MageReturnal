using UnityEngine;
using System;
using System.Collections;

public enum BehaviorNodeStatus {
	Running,
	Success,
	Failed,
}

public struct BehaviorContext<T> {
	public T data;

	public static BehaviorContext<T> create(T data) {
		var ctx = new BehaviorContext<T>();
		ctx.data = data;
		return ctx;
	}
}

public abstract class BehaviorNode<T> {
	public abstract void Init(BehaviorContext<T> ctx);
	public abstract BehaviorNodeStatus Update(BehaviorContext<T> ctx);

	public static implicit operator BehaviorNode<T>(ActionDelegate<T> fn) => new ActionNode<T>(fn);
}

public class BehaviorTree<T> {
	private BehaviorNode<T> root;

	public BehaviorTree(BehaviorNode<T> root) {
		this.root = root;
	}

	public void Init(T data) {
		root.Init(BehaviorContext<T>.create(data));
	}

	public void Update(T data) {
		root.Update(BehaviorContext<T>.create(data));
	}
}

public delegate BehaviorNodeStatus ActionDelegate<T>(BehaviorContext<T> ctx);

public class ActionNode<T> : BehaviorNode<T> {
	private readonly ActionDelegate<T> fn;

	public ActionNode(ActionDelegate<T> fn) {
		this.fn = fn;
	}

	public static implicit operator ActionNode<T>(ActionDelegate<T> fn) => new ActionNode<T>(fn);

	public override void Init(BehaviorContext<T> ctx) { }

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		return fn(ctx);
	}
}

public delegate BehaviorNodeStatus DecoratorDelegate<T>(BehaviorNodeStatus status, BehaviorContext<T> ctx);

public class DecoratorNode<T> : BehaviorNode<T> {

	private readonly DecoratorDelegate<T> fn;
	private readonly BehaviorNode<T> child;

	public DecoratorNode(DecoratorDelegate<T> fn, BehaviorNode<T> child) {
		this.fn = fn;
		this.child = child;
	}

	public override void Init(BehaviorContext<T> ctx) {
		child.Init(ctx);
	}

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		return fn(child.Update(ctx), ctx);
	}
}

public class InverterNode<T> : DecoratorNode<T> {

	private static readonly DecoratorDelegate<T> invert = (status, ctx) => {
		switch (status) {
			case BehaviorNodeStatus.Success:
				return BehaviorNodeStatus.Failed;
			case BehaviorNodeStatus.Failed:
				return BehaviorNodeStatus.Success;
			default:
				return status;
		}
	};

	public InverterNode(BehaviorNode<T> child) : base(invert, child) { }
}

public class LoopNode<T> : BehaviorNode<T> {
	private readonly BehaviorNode<T> child;
	private readonly int count;

	private int iteration = 0;

	public LoopNode(BehaviorNode<T> child) : this(child, 0) { }

	public LoopNode(BehaviorNode<T> child, int count) {
		this.child = child;
		this.count = count;
	}

	public override void Init(BehaviorContext<T> ctx) {
		iteration = 0;
		child.Init(ctx);
	}

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		var status = child.Update(ctx);

		if (status != BehaviorNodeStatus.Running) {
			++iteration;

			if (count > 0 && iteration >= count) {
				return BehaviorNodeStatus.Success;
			}

			child.Init(ctx);
		}
		return BehaviorNodeStatus.Running;
	}
}

public class LoopUntilFailNode<T> : BehaviorNode<T> {
	private readonly BehaviorNode<T> child;

	private readonly int count;

	private int iteration = 0;

	public LoopUntilFailNode(BehaviorNode<T> child) : this(child, 0) { }

	public LoopUntilFailNode(BehaviorNode<T> child, int count) {
		this.child = child;
		this.count = count;
	}

	public override void Init(BehaviorContext<T> ctx) {
		iteration = 0;
		child.Init(ctx);
	}

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		var status = child.Update(ctx);

		if (status == BehaviorNodeStatus.Success) {
			++iteration;

			if (count > 0 && iteration >= count) {
				return BehaviorNodeStatus.Success;
			}

			child.Init(ctx);
			status = BehaviorNodeStatus.Running;
		}

		return status;
	}
}

public abstract class CompositeNode<T> : BehaviorNode<T> {
	protected readonly BehaviorNode<T>[] nodes;

	public CompositeNode(params BehaviorNode<T>[] nodes) {
		this.nodes = nodes;
	}
}

public class SequenceNode<T> : CompositeNode<T> {

	private BehaviorNode<T> runningNode;

	public SequenceNode(params BehaviorNode<T>[] nodes) : base(nodes) { }

	public override void Init(BehaviorContext<T> ctx) {
		runningNode = null;
	}

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		var result = BehaviorNodeStatus.Success;

		foreach (var node in nodes) {
			if (runningNode == null) {
				runningNode = node;
				runningNode.Init(ctx);
			}
			if (node == runningNode) {
				result = runningNode.Update(ctx);
				if (result == BehaviorNodeStatus.Running) {
					break;
				}

				runningNode = null;
				if (result == BehaviorNodeStatus.Failed) {
					break;
				}
			}
		}

		return result;
	}
}

public class SelectorNode<T> : CompositeNode<T> {

	private BehaviorNode<T> runningNode;

	public SelectorNode(params BehaviorNode<T>[] nodes) : base(nodes) { }

	public override void Init(BehaviorContext<T> ctx) {
		runningNode = null;
	}

	public override BehaviorNodeStatus Update(BehaviorContext<T> ctx) {
		var result = BehaviorNodeStatus.Failed;

		foreach (var node in nodes) {
			if (runningNode == null) {
				runningNode = node;
				runningNode.Init(ctx);
			}

			if (node == runningNode) {
				result = runningNode.Update(ctx);
				if (result == BehaviorNodeStatus.Running) {
					break;
				}

				runningNode = null;
				if (result == BehaviorNodeStatus.Success) {
					break;
				}
			}
		}

		return result;
	}
}