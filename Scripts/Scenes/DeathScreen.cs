using Godot;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class DeathScreen : Node2D
{
	private static readonly Vector2 FinalPos = new Vector2(0.0f, 0.0f);
	private static readonly Vector2 FinalScale = new Vector2(1f, 1f);
	private const float Speed = 8.0f; 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Scale = new Vector2(10.0f, 10.0f);
		this.Position = new Vector2(0.0f, -100.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (this.Position.Y < FinalPos.Y)
		{
			this.Position += new Vector2(0.0f, 2.0f);
		}

		Vector2 scale = this.Scale;
		if (scale.X > FinalScale.X)
		{
			scale.X += (-Speed * (float)delta);
		}

		if (scale.Y > FinalScale.Y)
		{
			scale.Y += (-Speed * (float)delta);
		}

		this.Scale = scale < FinalScale ? FinalScale : scale;
	}
}
