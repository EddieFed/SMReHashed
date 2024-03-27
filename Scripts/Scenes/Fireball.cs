using System.Threading;
using Godot;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class Fireball : CharacterBody2D
{
	public const float Speed = 300.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float Gravity = 22.0f;
	public float Bounce = -350f;

	public int Direction = -1;

	public override void _Ready()
	{
		
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void KillMe()
	{
		this.QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		this.RotationDegrees += 25.0f * Direction;
		
		Vector2 velocity = Velocity;
		velocity.Y += Gravity;

		if (IsOnWall())
		{
			Direction *= -1;
		}

		// Add the gravity.
		if (IsOnFloor())
		{
			velocity.Y += Bounce;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		velocity.X = Direction * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}
}