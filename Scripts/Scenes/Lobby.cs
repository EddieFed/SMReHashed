using System.Linq;
using Godot;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class Lobby : Control
{

	[Export] private int _port = 7777;
	[Export] private string _address = "127.0.0.1";

	private ENetMultiplayerPeer _peer;
	
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
		}

	}

	private void _hostGame()
	{
		_peer = new ENetMultiplayerPeer();
		Error err = _peer.CreateServer(_port, 100);

		if (err != Error.Ok)
		{
			GD.Print($"Error, cannot host! {err.ToString()}");
			return;
		}

		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Waiting for players!");
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
		RpcId(1, nameof(TransmitPlayerInformation), GetNode<LineEdit>("Username").Text, Multiplayer.GetUniqueId());
	}

	// Runs when player disconnects, runs on ALL peers
	private void PeerDisconnected(long id)
	{
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
		GD.Print($"Player Connected: {id.ToString()}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void _on_host_button_down()
	{
		_hostGame();
		TransmitPlayerInformation(GetNode<LineEdit>("Username").Text, 1);

	}

	public void _on_join_button_down()
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(_address, _port);
		
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Joining game...");

	}

	public void _on_start_button_down()
	{
		Rpc(nameof(StartGame));
	}

	// https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html#initializing-the-network:~:text=The%20parameters%20and%20their%20functions%20are%20as%20follows%3A
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		foreach (PlayerInfo playerInfo in Managers.GameManager.Players)
		{
			GD.Print($"{playerInfo.Name} is Playing");
		}
		
		
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Scenes/World.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);

		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void TransmitPlayerInformation(string name, int id)
	{
		PlayerInfo playerInfo = new PlayerInfo()
		{
			Name = name,
			Id = id
		};

		if (!Managers.GameManager.Players.Contains(playerInfo))
		{
			Managers.GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer())
		{
			foreach (PlayerInfo info in Managers.GameManager.Players)
			{
				Rpc(nameof(TransmitPlayerInformation), info.Name, info.Id);
			}
		}
	}
	
}