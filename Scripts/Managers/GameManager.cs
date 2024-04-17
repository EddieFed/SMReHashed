using System;
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
	public static int LocalId = Int32.MinValue;
	
	public static GameStatuses GameStatus = GameStatuses.GameSelect;

	private static int NumPlayersDead = 0;

	public static int PlayerDeath(int playerId)
	{
		NumPlayersDead++;
		GD.Print($"player died {playerId}");
		GD.Print($"total players dead {NumPlayersDead}");
		int playerIndex = Players.FindIndex(player => player.Id == playerId);
		Players[playerIndex] = new PlayerInfo()
		{
			Id = Players[playerIndex].Id,
			Name = Players[playerIndex].Name,
			Ready = Players[playerIndex].Ready,
			IsDead = true,
		};
		if (NumPlayersDead == (Players.Count - 1)) return AlertWinner();
		return Int32.MinValue;
	}

	private static int AlertWinner()
	{
		// find the player that Won
		foreach (PlayerInfo player in Players)
		{
			if (!player.IsDead && LocalId == player.Id)
			{
				return player.Id;
			}
		}
		return Int32.MinValue;
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