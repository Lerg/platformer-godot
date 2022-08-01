using Godot;
using System.Collections.Generic;

namespace AnimationFSM {
	// Conditions that all states are checking if they should be active.
	public struct Conditions {
		public State currentState;
		public bool isAnimationFinished;
		public bool isOnFloor;
		public bool isAttacking;
		public bool isDefending;
		public int health;
		public Vector2i inputDirection;
		public Vector2i moveDirection;
	}

	// Base class for animation state.
	public abstract class State {
		// Name of the sprite animation.
		public readonly string animationName;
		// Hash of the name for quick string comparasion.
		public readonly uint animationNameHash;

		protected State(string animationName) {
			this.animationName = animationName;
			animationNameHash = animationName.Hash();
		}

		// Each state should provide it's own implemention.
		public abstract bool IsMatchingConditions(in Conditions conditions);

		// Check if this state is currently active and its animation is still playing.
		protected bool isPlaying(in Conditions conditions) {
			return (conditions.currentState == this && !conditions.isAnimationFinished);
		}
	}

	// Animation Finite State Machine.
	public class FSM {
		public Conditions conditions;
		public State currentState;
		List<State> states = new List<State>();

		public void AddState(State state) {
			states.Add(state);
		}

		public void AddStates(IEnumerable<State> states) {
			this.states.AddRange(states);
		}

		// Determine which state should be active.
		public void Update() {
			foreach (var state in states) {
				if (state.IsMatchingConditions(in conditions)) {
					currentState = state;
					break;
				}
			}
		}
	}
}
