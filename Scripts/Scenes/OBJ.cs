using Godot;
using System;

public partial class OBJ : Area2D
{
	int max_collisions = 6;
	private void _on_body_entered(CharacterBody2D body)
	{	
		
		//body.addItem();
		GD.Print("Collected Block");
		//QueueFree();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void _PhysicsProcess(double delta) {
		var collision_count = 0;
		var collision = MoveAndCollide(Velocity * delta);
		
		 while(collision && collision_count < max_collisions) {
			var collider = collision.get_collider();
		}
	}
}






