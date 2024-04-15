using Godot;
using System;
using SuperMarioRehashed.Scripts.Scenes;
namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class QuestionBlock : MyObject
{
	private enum State { Unbumped, Bumped }
	private State state = State.Unbumped;
	private Vector2 originalPosition;
	private int Item = 1;	// HOW TO SYNC???

	public override void _Ready()
	{
		base._Ready(); // Call the base class _Ready, if it's defined and necessary
		originalPosition = Position;
		Item = Managers.WorldManager.RandomGenerator.Next(0, 3);
	}

	public override void OnCollide(Node2D other)
	{
		// Check if the colliding object is a Player and if the state is Unbumped
		if (other is Player && state == State.Unbumped)
		{
			BumpBlock();
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	void SpawnItem()
	{
		if (Multiplayer.IsServer())
		{
			SceneTreeTimer timer = GetTree().CreateTimer(5);
			timer.Connect("timeout", new Callable(this, nameof(ResetBlock)));
		}
		string resourceName = Item switch
		{
			0 => "Coin",
			1 => "FireFlower",
			2 => "IceFlower",
			_ => "Illegal Item!"
		};
		GD.Print(resourceName);
		PackedScene packedScene = GD.Load<PackedScene>($"res://Scenes/Prefabs/{resourceName}.tscn");
		Node2D item = packedScene.Instantiate<Node2D>();
		item.GlobalPosition = this.Position + new Vector2(50.0f, 0.0f);
		
		this.GetParent().AddChild(item);
	}
	
	private void BumpBlock()
	{
		if (Multiplayer.IsServer())
		{
			SceneTreeTimer time = GetTree().CreateTimer(5);
			time.Connect("timeout", new Callable(this, nameof(ResetBlock)));
		}
		state = State.Bumped;

		GetNode<Sprite2D>("Sprite2D").Frame = 1;

		BumpUpwards();
		
		SceneTreeTimer timer = GetTree().CreateTimer(0.2f);
		timer.Connect("timeout", new Callable(this, nameof(ReturnToOriginalPosition)));
		
		Rpc(nameof(SpawnItem));
	}

	private void BumpUpwards()
	{
		Position += new Vector2(0, -10);
	}

	private void ReturnToOriginalPosition()
	{
		Position = originalPosition;
	}

	void ResetBlock()
	{
		Rpc(nameof(ResetBlockRPC));
		GetNode<Sprite2D>("Sprite2D").Frame = 0;
		this.state = State.Unbumped;
	}
	
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	void ResetBlockRPC()
	{
		GetNode<Sprite2D>("Sprite2D").Frame = 0;
		this.state = State.Unbumped;
	}

}





