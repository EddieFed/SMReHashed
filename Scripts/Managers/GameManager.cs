using System.Collections.Generic;
using Godot;

namespace SuperMarioRehashed.Scripts.Managers;

public partial class GameManager : Node
{
	public static readonly List<PlayerInfo> Players = new List<PlayerInfo>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}