using System.Runtime.CompilerServices;

namespace AnimationFSM {
	public class AttackRunState : State {
		public AttackRunState(string animationName = "attack_run") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.isAttacking && conditions.isOnFloor && conditions.inputDirection.x != 0;
		}
	}

	public class AttackState : State {
		public AttackState(string animationName = "attack") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.isAttacking || isPlaying(in conditions);
		}
	}

	public class AttackUpState : State {
		public AttackUpState(string animationName = "attack_up") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.isAttacking && conditions.inputDirection.y == 1;
		}
	}

	public class CrouchState : State {
		public CrouchState(string animationName = "crouch") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.isOnFloor && conditions.inputDirection.y == 1;
		}
	}

	public class DeathState : State {
		public DeathState(string animationName = "death") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.health == 0;
		}
	}

	public class DefenseState : State {
		public DefenseState(string animationName = "defense") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.isDefending;
		}
	}

	public class FallState : State {
		public FallState(string animationName = "fall") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return !conditions.isOnFloor && conditions.moveDirection.y == 1;
		}
	}

	public class HurtState : State {
		private int _health;
		public HurtState(string animationName = "hurt") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			var isMatching = conditions.health < _health || isPlaying(in conditions);
			_health = conditions.health;
			return isMatching;
		}
	}

	public class IdleState : State {
		public IdleState(string animationName = "idle") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return true;
		}
	}

	public class JumpState : State {
		public JumpState(string animationName = "jump") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return !conditions.isOnFloor && conditions.moveDirection.y == -1;
		}
	}

	public class RunState : State {
		public RunState(string animationName = "run") : base(animationName) {}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsMatchingConditions(in Conditions conditions) {
			return conditions.inputDirection.x != 0;
		}
	}
}