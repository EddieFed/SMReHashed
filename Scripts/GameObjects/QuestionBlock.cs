using Godot;
using System;
using SuperMarioRehashed.Scripts.Scenes;
namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class QuestionBlock : MyObject
{
	private enum State { Unbumped, Bumped }
	private State state = State.Unbumped;
	private Vector2 originalPosition;

	public override void _Ready()
	{
		base._Ready(); // Call the base class _Ready, if it's defined and necessary
		originalPosition = Position;
	}

	public override void OnCollide(Node2D other)
	{
		// Check if the colliding object is a Player and if the state is Unbumped
		if (other is Player && state == State.Unbumped)
		{
			BumpBlock();
		}
	}


	private void BumpBlock()
	{
		state = State.Bumped;

		GetNode<Sprite2D>("Sprite2D").Frame = 1;

		BumpUpwards();
		
		var timer = GetTree().CreateTimer(0.2f);
		timer.Connect("timeout", new Callable(this, nameof(ReturnToOriginalPosition)));

	}

	private void BumpUpwards()
	{
		Position += new Vector2(0, -10);
	}

	private void ReturnToOriginalPosition()
	{
		Position = originalPosition;
	}
}





