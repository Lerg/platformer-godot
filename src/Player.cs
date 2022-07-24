using Godot;
using System;

public class Player : RigidBody2D {
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	public override void _PhysicsProcess(float delta) {
		var inputDirection = new Vector2i(
			(Input.IsActionPressed("left") ? -1 : 0) + (Input.IsActionPressed("right") ? 1 : 0),
			Input.IsActionPressed("down") ? 1 : 0
		);
		//var inputDirection = ne
		ApplyCentralImpulse(new Vector2(inputDirection.x, 0));
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
