using Godot;
using System;
using SuperMarioRehashed.Scripts;


public partial class OBJ : Area2D
{
	int max_collisions = 6;
	private void _on_body_entered(Player p)
	{
		p.AddItem("Coin");
		QueueFree();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}






