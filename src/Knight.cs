using Godot;

public class Knight : Area2D {
	public override void _Ready() {
		Connect("body_entered", this, "OnBodyEntered");
	}

	private void OnBodyEntered(Node body) {
		if (body is Player player) {
			player.Hurt();
		}
	}
}
