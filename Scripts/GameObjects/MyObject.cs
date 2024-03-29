using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public enum ObjectType
{
    None,
    Coin,
    Mushroom,
    FireFlower,
    IceFlower,
    Boomerang,
    QuestionBlock
} 

public abstract partial class MyObject : Area2D
{
	public Vector2 Speed = new Vector2(0, 0);
	
	public override void _Ready()
	{
		
		this.Monitoring = true;
		this.BodyEntered += (other) => OnCollide(other);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void DestroyObject()
	{
		this.QueueFree();
	}
	
	abstract public void OnCollide(Node2D other);

	/*public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = this.Velocity;
		velocity.Y += this.Gravity;
	}*/
}
