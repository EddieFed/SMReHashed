using Godot;
using System;
using System.Collections.Generic;
using SuperMarioRehashed.Scripts.Managers;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.Scenes;


public partial class LobbyLoading : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private ItemList _playerList;
	

	private int _playerCount;
	
	public override void _Ready()
	{
		_playerList = this.GetChild<ItemList>(0);
		
		GD.Print($"Using chunk #{1}");
		Node2D chunk = (Node2D)GD.Load<PackedScene>($"res://Scenes/Chunks/Chunk{1}.tscn").Instantiate();
		this.AddChild(chunk);

		chunk.GlobalTranslate(new Vector2(0, 0));
		PlayerLocal currentPlayer = (PlayerLocal)GD.Load<PackedScene>("res://Scenes/PlayerLocal.tscn").Instantiate();
		this.AddChild(currentPlayer);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var count = Managers.GameManager.Players.Count;
		if (_playerCount != count)
		{
			GD.Print("updating list");
			_playerCount = count;
			_playerList.Clear();
			foreach (PlayerInfo player in Managers.GameManager.Players)
			{
				string name = player.Name == "" ? "{TEMP_NAME}" : player.Name;
				_playerList.AddItem($"{name}");
			}
		}
		
	}
}
