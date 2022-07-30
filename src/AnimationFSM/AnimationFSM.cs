using Godot;
using System.Collections;
using System.Collections.Generic;

namespace AnimationFSM {
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

	public abstract class State {
		public string animationName;
		public uint animationNameHash;

		protected State(string animationName) {
			this.animationName = animationName;
			animationNameHash = animationName.Hash();
		}

		public abstract bool IsMatchingConditions(in Conditions conditions);
		protected bool isPlaying(in Conditions conditions) {
			return (conditions.currentState == this && !conditions.isAnimationFinished);
		}
	}

	public class FSM {
		public Conditions conditions;
		public State currentState;
		List<State> states = new List<State>();

		public void AddState(State state) {
			states.Add(state);
		}

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
