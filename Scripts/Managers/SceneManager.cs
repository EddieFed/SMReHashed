using Godot;

namespace SuperMarioRehashed.Scripts.Managers;

public partial class SceneManager : Node2D
{

	[Export] private PackedScene _playerScene;
	private static SceneManager _sceneManager = null; 
	
	public SceneManager()
	{
		_sceneManager = this;
	}
	
	// We want a singleton scene manager for each game!
	public static SceneManager GetSceneManager()
	{
		if (_sceneManager == null)
		{
			_sceneManager = new SceneManager();
			
		}
		
		return _sceneManager;
	}
	public void LoadPlayer(int id)
	{
		int index = 0;
		foreach (PlayerInfo playerInfo in Managers.GameManager.Players)
		{
			Scenes.Player currentPlayer = _playerScene.Instantiate<Scenes.Player>();
			currentPlayer.Name = playerInfo.Id.ToString();
			this.AddChild(currentPlayer);

			currentPlayer.GlobalPosition = new Vector2((int)(WorldManager.ChunkSize) * 3 * index + 200, 200);
			index++;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}