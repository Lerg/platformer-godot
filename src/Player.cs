using Godot;
using System;

public class Player : KinematicBody2D {
	private const int _gravity = 1500;
	// Run speed.
	private const int _speed = 250;
	private const int _jumpSpeed = 500;
	// Clamp falling speed.
	private const int _maxFallSpeed = 750;
	// True when character is standing on floor.
	private bool _isOnFloor;
	// True when character can double jump.
	private bool _isDoubleJumpAvailable;
	// Current body velocity.
	private Vector2 _velocity = Vector2.Zero;
	// Character health.
	private int _health = 10;
	// Prevent player control for a short time after being attacked.
	private const float _hurtDuration = 0.2f;
	private float _hurtTimeout;
	// Left -1 or right 1.
	private int _headingDirection = 1;
	// Character sprite.
	private AnimatedSprite _sprite;
	// True when the current sprite animation has finished playing.
	private bool _isAnimationFinished;
	// State machine to switch sprite animations.
	private readonly AnimationFSM.FSM _fsm = new AnimationFSM.FSM();

	public override void _Ready() {
		_sprite = GetNode("AnimatedSprite") as AnimatedSprite;
		_sprite.Connect("animation_finished", this, "OnAnimationFinished");

		// States added first have higher priority.
		_fsm.AddStates(new AnimationFSM.State[] {
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

	private void OnAnimationFinished() {
		_isAnimationFinished = true;
	}

	// The character is attacked by another entity.
	public void Hurt() {
		if (_health > 0) {
			_health -= 1;
			_hurtTimeout = _hurtDuration;
		}
	}

	public override void _PhysicsProcess(float delta) {
		// Was the character attacked?
		var isHurt = _hurtTimeout > 0;
		// Disable input if being hurt or dead.
		var isInputEnabled = !isHurt && _health > 0;
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
				_headingDirection = inputDirection.x;
			}

			// Set the current x velocity based on the input.
			_velocity.x = _speed * inputDirection.x;

			// Reset double jump ability when on floor.
			if (_isOnFloor) {
				_isDoubleJumpAvailable = true;
			}

			// Jump when key is pressed and the character is on floor or when double jump is available.
			if (Input.IsActionJustPressed("jump")) {
				var canJump = _isOnFloor;
				if (!canJump && _isDoubleJumpAvailable) {
					canJump = true;
					_isDoubleJumpAvailable = false;
				}
				if (canJump) {
					_velocity.y = -_jumpSpeed;
				}
			}
		} else {
			// Stop X movement when input is disabled.
			_velocity.x = 0;
		}

		// Knockback the character when being hurt.
		// Move in the opposite of heading direction and up a little.
		if (isHurt) {
			_velocity.x = -0.5f * _speed * _headingDirection;
			// When was just hit.
			if (_hurtTimeout == _hurtDuration) {
				_velocity.y = -1f * _speed;
			}
			// Countdown the hurt timer.
			_hurtTimeout -= delta;
		}

		// When falling or jumping increase the gravity. That way jumping feels faster and more precise.
		// Also controls the height of the jump when the jump button is released.
		if (!(isInputEnabled && Input.IsActionPressed("jump")) && !_isOnFloor) {
			gravityScale = 2;
		}

		// Apply gravity and clamp falling speed.
		if (!_isOnFloor) {
			_velocity.y += gravityScale * _gravity * delta;
			_velocity.y = Mathf.Min(_velocity.y, _maxFallSpeed);
		}

		// Move Godot's KinematicBody.
		_velocity = MoveAndSlide(_velocity, Vector2.Up);
		// IsOnFloor() is available only after MoveAndSlide().
		_isOnFloor = IsOnFloor();

		// Flip the sprite according to the heading.
		_sprite.FlipH = _headingDirection == -1;

		// Set animation FSM conditions.
		ref var conditions = ref _fsm.conditions;
		conditions.inputDirection = inputDirection;
		conditions.moveDirection.x = Math.Sign(_velocity.x);
		conditions.moveDirection.y = Math.Sign(_velocity.y);
		conditions.isAttacking = Input.IsActionJustPressed("attack");
		conditions.isDefending = Input.IsActionPressed("defense");
		conditions.isOnFloor = _isOnFloor;
		conditions.health = _health;
		conditions.currentState = _fsm.currentState;
		conditions.isAnimationFinished = _isAnimationFinished;
		// Determine which is the current animation state.
		_fsm.Update();

		// Play new animation when animation state is changed.
		var currentState = _fsm.currentState;
		if (_sprite.Animation.Hash() != currentState.animationNameHash) {
			_sprite.Play(currentState.animationName);
			_isAnimationFinished = false;
		}
	}
}
