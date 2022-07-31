using Godot;
using System;

public class Knight : Area2D {
	public override void _Ready() {
		Connect("body_entered", this, "onBodyEntered");
	}

	void onBodyEntered(Node body) {
		if (body is Player player) {
			player.Hurt();
		}
	}
}
