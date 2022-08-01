using Godot;
using System;

public class Player : KinematicBody2D {
	const int gravity = 1500;
	// Run speed.
	const int speed = 250;
	const int jumpSpeed = 500;
	// Clamp falling speed.
	const int maxFallSpeed = 750;
	// True when character is standing on floor.
	bool isOnFloor;
	// True when character can double jump.
	bool isDoubleJumpAvailable;
	// Current body velocity.
	Vector2 velocity = Vector2.Zero;
	// Character sprite.
	AnimatedSprite sprite;
	// State machine to switch sprite animations.
	readonly AnimationFSM.FSM fsm = new AnimationFSM.FSM();
	// True when the current sprite animation has finished playing.
	bool isAnimationFinished;
	// Character health.
	int health = 10;
	// Prevent player control for a short time after being attacked.
	const float hurtDuration = 0.2f;
	float hurtTimeout;
	// Left -1 or right 1.
	int headingDirection = 1;

	public override void _Ready() {
		sprite = GetNode("AnimatedSprite") as AnimatedSprite;
		sprite.Connect("animation_finished", this, "OnAnimationFinished");

		// States added first have higher priority.
		fsm.AddStates(new AnimationFSM.State[] {
			new AnimationFSM.DeathState(),
			new AnimationFSM.HurtState(),
			new AnimationFSM.AttackState(),
			new AnimationFSM.DefenseState(),
			new AnimationFSM.JumpState(),
			new AnimationFSM.FallState(),
			new AnimationFSM.RunState(),
			new AnimationFSM.CrouchState(),
			new AnimationFSM.IdleState()
		});
	}

	void OnAnimationFinished() {
		isAnimationFinished = true;
	}

	// The character is attacked by another entity.
	public void Hurt() {
		if (health > 0) {
			health -= 1;
			hurtTimeout = hurtDuration;
		}
	}

	public override void _PhysicsProcess(float delta) {
		// Was the character attacked?
		var isHurt = hurtTimeout > 0;
		// Disable input if being hurt or dead.
		var isInputEnabled = !isHurt && health > 0;
		// Gravity is asjusted when jumping or falling.
		var gravityScale = 1;
		// Player movement input, e.g. WASD or d-pad.
		var inputDirection = new Vector2i();
		if (isInputEnabled) {
			// Read movement input.
			inputDirection.x = (Input.IsActionPressed("left") ? -1 : 0) + (Input.IsActionPressed("right") ? 1 : 0);
			inputDirection.y = Input.IsActionPressed("down") ? 1 : 0;

			// Heading can't be 0.
			if (inputDirection.x != 0) {
				headingDirection = inputDirection.x;
			}

			// Set the current x velocity based on the input.
			velocity.x = speed * inputDirection.x;

			// Reset double jump ability when on floor.
			if (isOnFloor) {
				isDoubleJumpAvailable = true;
			}

			// Jump when key is pressed and the character is on floor or when double jump is available.
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
			// Stop X movement when input is disabled.
			velocity.x = 0;
		}

		// Knockback the character when being hurt.
		// Move in the opposite of heading direction and up a little.
		if (isHurt) {
			velocity.x = -0.5f * speed * headingDirection;
			// When was just hit.
			if (hurtTimeout == hurtDuration) {
				velocity.y = -1f * speed;
			}
			// Countdown the hurt timer.
			hurtTimeout -= delta;
		}

		// When falling or jumping increase the gravity. That way jumping feels faster and more precise.
		// Also controls the height of the jump when the jump button is released.
		if (!(isInputEnabled && Input.IsActionPressed("jump")) && !isOnFloor) {
			gravityScale = 2;
		}

		// Apply gravity and clamp falling speed.
		if (!isOnFloor) {
			velocity.y += gravityScale * gravity * delta;
			velocity.y = Mathf.Min(velocity.y, maxFallSpeed);
		}

		// Move Godot's KinematicBody.
		velocity = MoveAndSlide(velocity, Vector2.Up);
		// IsOnFloor() is available only after MoveAndSlide().
		isOnFloor = IsOnFloor();

		// Flip the sprite according to the heading.
		sprite.FlipH = headingDirection == -1;

		// Set animation FSM conditions.
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
		// Determine which is the current animation state.
		fsm.Update();

		// Play new animation when animation state is changed.
		var currentState = fsm.currentState;
		if (sprite.Animation.Hash() != currentState.animationNameHash) {
			sprite.Play(currentState.animationName);
			isAnimationFinished = false;
		}
	}
}
