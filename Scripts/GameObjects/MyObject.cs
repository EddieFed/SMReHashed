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
	public override void _Ready()
	{
		
		this.Monitoring = true;
		this.BodyEntered += (other) => OnCollide(other);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void DestroyObject()
	{
		this.QueueFree();
	}
	
	abstract public void OnCollide(Node2D other);

}
