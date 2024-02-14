using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using SuperMarioRehashed.Scripts.Managers;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.Scenes;


public partial class LobbyLoading : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private ItemList _playerList;
	private int _playerId;
	private bool _updateList;
	
	private Texture2D _checkIcon = GD.Load<Texture2D>("res://Assets/Icons/CheckIcon.svg");
	private Texture2D _crossIcon = GD.Load<Texture2D>("res://Assets/Icons/CrossIcon.svg");
	
	public override void _Ready()
	{
		_playerList = this.GetChild<ItemList>(0);
		_playerId = Multiplayer.GetUniqueId();
		_updateList = false;
		
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
		if (_playerList.ItemCount != count  || _updateList)
		{
			_updateList = false;
			GD.Print("updating list");
			_playerList.Clear();
			foreach (PlayerInfo player in Managers.GameManager.Players)
			{
				_playerList.AddItem(player.Name == "" ? "{TEMP_NAME}" : player.Name, player.Ready ? _checkIcon : _crossIcon);
			}
		}
		
	}

	public void _on_check_button_toggled(bool status)
	{
		GD.Print("toggle");
		Rpc(nameof(TransmitPlayerStatus), _playerId, status);

	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void TransmitPlayerStatus(int id, bool status)
	{
		_updateList = true;
		GD.Print($"I for a RPC on id {_playerId}");
		List<PlayerInfo> players = Managers.GameManager.Players;
		int playerIndex = players.FindIndex(player => player.Id == id);
		if (playerIndex != -1)
		{
			players[playerIndex] = new PlayerInfo()
			{
				Id = players[playerIndex].Id,
				Name = players[playerIndex].Name,
				Ready = status,
			};
		}
	}
}
