using Godot;

namespace SuperMarioRehashed.Scripts.Managers;

public partial class SceneManager : Node2D
{

	[Export] private PackedScene _playerScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int index = 0;
		foreach (PlayerInfo playerInfo in Managers.GameManager.Players)
		{
			Scenes.Player currentPlayer = _playerScene.Instantiate<Scenes.Player>();
			currentPlayer.Name = playerInfo.Id.ToString();
			this.AddChild(currentPlayer);

			currentPlayer.GlobalPosition = new Vector2((int)(WorldManager.ChunkSize / 2) * index + 100, 200);
			index++;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
