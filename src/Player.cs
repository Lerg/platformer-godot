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
	public override void _Ready() {
		sprite = GetNode("AnimatedSprite") as AnimatedSprite;
	}

	public override void _PhysicsProcess(float delta) {
		var inputDirection = new Vector2i(
			(Input.IsActionPressed("left") ? -1 : 0) + (Input.IsActionPressed("right") ? 1 : 0),
			Input.IsActionPressed("down") ? 1 : 0
		);

		velocity.x = speed * inputDirection.x;
		var gravityScale = 1;
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
		} else if (!Input.IsActionPressed("jump") && !isOnFloor) {
			gravityScale = 2;
		}
		if (!isOnFloor) {
			velocity.y += gravityScale * gravity * delta;
			velocity.y = Mathf.Min(velocity.y, maxFallSpeed);
		}
		velocity = MoveAndSlide(velocity, Vector2.Up);
		isOnFloor = IsOnFloor();

		if (inputDirection.x != 0) {
			sprite.FlipH = inputDirection.x == -1;
		}
	}
}
