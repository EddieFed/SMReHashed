using System.Collections.Generic;
using Godot;
using SuperMarioRehashed.Scripts.Util;

namespace SuperMarioRehashed.Scripts.Managers;

public partial class GameManager : Node
{
	public enum GameStatuses
	{
		GameSelect,
		InLobby,
		StartTimer,
		StartingGame,
		InGame,
	};
	
	public static readonly List<PlayerInfo> Players = new List<PlayerInfo>();
	
	public static GameStatuses GameStatus = GameStatuses.GameSelect;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}