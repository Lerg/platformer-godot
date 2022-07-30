using Godot;
using System;

public class Player : KinematicBody2D {
	const int gravity = 1500;
	const int speed = 250;
	const int jumpSpeed = 500;
	const int maxFallSpeed = 750;
	bool isOnFloor;
	bool isDoubleJumpAvailable;
	Vector2 velocity = Vector2.Zero;
	AnimatedSprite sprite;
	AnimationFSM.FSM fsm = new AnimationFSM.FSM();
	bool isAnimationFinished = false;
	int health = 10;
	float hurtDuration = 0.2f;
	float hurtTimeout;
	int headingDirection = 1;

	public override void _Ready() {
		sprite = GetNode("AnimatedSprite") as AnimatedSprite;
		sprite.Connect("animation_finished", this, "onAnimationFinished");

		fsm.AddState(new AnimationFSM.DeathState());
		fsm.AddState(new AnimationFSM.HurtState());
		fsm.AddState(new AnimationFSM.AttackState());
		fsm.AddState(new AnimationFSM.DefenseState());
		fsm.AddState(new AnimationFSM.JumpState());
		fsm.AddState(new AnimationFSM.FallState());
		fsm.AddState(new AnimationFSM.RunState());
		fsm.AddState(new AnimationFSM.CrouchState());
		fsm.AddState(new AnimationFSM.IdleState());
	}

	void onAnimationFinished() {
		isAnimationFinished = true;
	}

	public void Hurt() {
		if (health > 0) {
			health -= 1;
			hurtTimeout = hurtDuration;
		}
	}

	public override void _PhysicsProcess(float delta) {
		var isHurt = hurtTimeout > 0;
		var isInputEnabled = !isHurt && health > 0;
		var gravityScale = 1;
		var inputDirection = new Vector2i();
		if (isInputEnabled) {
			inputDirection.x = (Input.IsActionPressed("left") ? -1 : 0) + (Input.IsActionPressed("right") ? 1 : 0);
			inputDirection.y = Input.IsActionPressed("down") ? 1 : 0;

			if (inputDirection.x != 0) {
				headingDirection = inputDirection.x;
			}
			velocity.x = speed * inputDirection.x;

			if (isOnFloor) {
				isDoubleJumpAvailable = true;
			}
			if (Input.IsActionJustPressed("jump")) {
				var canJump = isOnFloor;
				if (!canJump && isDoubleJumpAvailable) {
					canJump = true;
					isDoubleJumpAvailable = false;
				}
				if (canJump) {
					velocity.y = -jumpSpeed;
				}
			}
		} else {
			velocity.x = 0;
		}

		if (isHurt) {
			velocity.x = -0.5f * speed * headingDirection;
			if (hurtTimeout == hurtDuration) {
				velocity.y = -1f * speed;
			}
			hurtTimeout -= delta;
		}

		if (!(isInputEnabled && Input.IsActionPressed("jump")) && !isOnFloor) {
			gravityScale = 2;
		}
		if (!isOnFloor) {
			velocity.y += gravityScale * gravity * delta;
			velocity.y = Mathf.Min(velocity.y, maxFallSpeed);
		}
		velocity = MoveAndSlide(velocity, Vector2.Up);
		isOnFloor = IsOnFloor();

		sprite.FlipH = headingDirection == -1;

		ref var conditions = ref fsm.conditions;
		conditions.inputDirection = inputDirection;
		conditions.moveDirection.x = Math.Sign(velocity.x);
		conditions.moveDirection.y = Math.Sign(velocity.y);
		conditions.currentState = fsm.currentState;
		conditions.isAnimationFinished = isAnimationFinished;
		conditions.isAttacking = Input.IsActionJustPressed("attack");
		conditions.isDefending = Input.IsActionPressed("defense");
		conditions.isOnFloor = isOnFloor;
		conditions.health = health;
		fsm.Update();

		var currentState = fsm.currentState;
		if (sprite.Animation.Hash() != currentState.animationNameHash) {
			sprite.Play(currentState.animationName);
			isAnimationFinished = false;
		}
	}
}
