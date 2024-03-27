using System;
using System.Linq;
using Godot;
using SuperMarioRehashed.Scripts.Managers;

namespace SuperMarioRehashed.Scripts;

public partial class Lobby : Control
{

	[Export] private int _port = 7777;
	[Export] private string _address = "127.0.0.1";

	private ENetMultiplayerPeer _peer;
	
	private int _numPlayers = 0;
	private const int MaxPlayers = 4;
	private bool _inGame = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		
		if (OS.GetCmdlineArgs().Contains("--server"))
		{
			_hostGame();
			Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LobbyLoading.tscn").Instantiate<Node2D>();
			// We need to use CallDeferred since there might be timing issues with "--server"
			CallDeferred("AddLobbyLoading", scene);
		}

	}

	private void _hostGame()
	{
		_peer = new ENetMultiplayerPeer();
		Error err = _peer.CreateServer(_port, MaxPlayers);

		if (err != Error.Ok)
		{
			GD.Print($"Error, cannot host! {err.ToString()}");
			return;
		}

		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Waiting for players!");
		
	}

	private void AddLobbyLoading(Node2D scene)
	{
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	// Runs on a failed connection, ONLY runs on client
	private void ConnectionFailed()
	{
		GD.Print("CONNECTION FAILED");
	}

	// Runs when server connection is made, ONLY runs on client
	private void ConnectedToServer()
	{
		GD.Print("CONNECTION SUCCEEDED");
	}

	// Runs when player disconnects, runs on ALL peers
	private void PeerDisconnected(long id)
	{
		_numPlayers--;
		GD.Print($"Player Disconnected: {id.ToString()}");
		Managers.GameManager.Players.Remove(Managers.GameManager.Players.First(i => i.Id == id));
		foreach (Node player in GetTree().GetNodesInGroup("Players"))
		{
			if (player.Name == id.ToString())
			{
				player.QueueFree();
			}
		}
	}

	// Runs when a player connects, runs on ALL peers
	private void PeerConnected(long id)
	{
		if (Multiplayer.IsServer())
		{
			String connected = (!_inGame) ? "You're good Bro" : "Get off my dick";
			RpcId(id, "CanConnect", connected);
			// kick player if the game is running
			if (_inGame) Multiplayer.MultiplayerPeer.DisconnectPeer((int)id, true);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void CanConnect(String message)
	{
		if (message == "You're good Bro")
		{
			RpcId(1, nameof(TransmitPlayerInformation), GetNode<LineEdit>("Username").Text, Multiplayer.GetUniqueId(), false);
			Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LobbyLoading.tscn").Instantiate<Node2D>(); 
			GetTree().Root.AddChild(scene);
			this.Hide();
		}
		else
		{
			_peer.Close();
			Multiplayer.MultiplayerPeer.Dispose();
		}
		GD.Print($"{message}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool allReady = Managers.GameManager.Players.FindIndex(player => player.Ready == false) == -1;
		GameManager.GameStatuses status = Managers.GameManager.GameStatus;
		if (status == GameManager.GameStatuses.InLobby && Multiplayer.GetUniqueId() == 1 && (_numPlayers == MaxPlayers || allReady && _numPlayers >= 2))
		{
			Rpc(nameof(StartTimer));
		} else if (Multiplayer.GetUniqueId() == 1 && Managers.GameManager.GameStatus == GameManager.GameStatuses.StartingGame)
		{
			this.SetProcess(false);
			int seed = Random.Shared.Next(1000);
			Rpc(nameof(StartGame), seed);
		}
	}

	public void _on_host_button_down()
	{
		_hostGame();
		
		TransmitPlayerInformation(GetNode<LineEdit>("Username").Text, 1, false);
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LobbyLoading.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	public void _on_join_button_down()
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(_address, _port);
		
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
	}

	public void _on_start_button_down()
	{
		// only let the Host manually  
		if (Multiplayer.GetUniqueId() != 1)
		{
			return;
		}
		SetProcess(false);	
		int seed = Random.Shared.Next(1000);
		Rpc(nameof(StartGame), seed);
	}

	// https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html#initializing-the-network:~:text=The%20parameters%20and%20their%20functions%20are%20as%20follows%3A
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame(int seed)
	{
		if (Multiplayer.IsServer())
		{
			_inGame = true;
			GD.Print($"Server is no longer taking orders");
		}
		Managers.GameManager.GameStatus = GameManager.GameStatuses.InGame;
		foreach (PlayerInfo playerInfo in Managers.GameManager.Players)
		{
			GD.Print($"{playerInfo.Name} is Playing");
		}
		
		// Load the world
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/World.tscn").Instantiate<Node2D>();
		Managers.WorldManager.GetWorldManager().GenerateWorld(new Random(seed));
		Managers.SceneManager.GetSceneManager().LoadPlayer(Multiplayer.GetUniqueId());
		GetTree().Root.AddChild(scene);

		// remove the lobbyLoading Screen if it exists
		var node = GetTree().Root.GetNode<LobbyLoading>("LobbyLoading");
		if (node != null)
		{
			GetTree().Root.RemoveChild(node);
		}
		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void TransmitPlayerInformation(string name, int id, bool ready)
	{
		PlayerInfo playerInfo = new PlayerInfo()
		{
			Name = name,
			Id = id,
			Ready = ready,
		};

		if (!Managers.GameManager.Players.Contains(playerInfo))
		{
			Managers.GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer())
		{
			_numPlayers++;
			foreach (PlayerInfo info in Managers.GameManager.Players)
			{
				Rpc(nameof(TransmitPlayerInformation), info.Name, info.Id, info.Ready);
			}
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartTimer()
	{
		Managers.GameManager.GameStatus = GameManager.GameStatuses.StartTimer;
	}
}
