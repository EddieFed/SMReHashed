using Godot;

namespace SuperMarioRehashed.Scripts.Scenes.Projectiles;

public partial class Fireball : MyProjectile
{
	public override void _Ready()
	{
		// Override any properties here!
	}

	public override void _PhysicsProcess(double delta)
	{
		this.RotationDegrees += 20.0f * this.Direction;
		
		// Y component //
		Vector2 velocity = this.Velocity;
		velocity.Y += this.Gravity;

		if (IsOnWall())
		{
			this.Direction *= -1;
		}
		
		if (IsOnFloor())
		{
			velocity.Y += this.Bounce;
		}
		
		// X component //
		velocity.X = this.Direction * this.Speed;

		// //
		this.Velocity = velocity;
		MoveAndSlide();
	}
	
}