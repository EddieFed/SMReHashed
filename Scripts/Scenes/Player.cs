using System;
using Godot;
using SuperMarioRehashed.Scripts.Util;
using System.Collections;
using SuperMarioRehashed.Scripts.GameObjects;
using SuperMarioRehashed.Scripts.Scenes.Projectiles;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class Player : CharacterBody2D
{
	// ----- Variables ----- //
	
	// Preloaded nodes
	private static readonly PackedScene Fireball = GD.Load<PackedScene>("res://Scenes/Prefabs/Fireball.tscn");
	private static readonly PackedScene Iceball = GD.Load<PackedScene>("res://Scenes/Prefabs/Iceball.tscn");
	
	// Child nodes
	private Area2D _area2D;
	private Sprite2D _sprite2D;
	private ProgressBar _healthBar;
	
	// Stats/properties
	private float _speed = 300.0f;
	private float _jumpSpeed = -400.0f;
	private float _health = 100.0f;
	private int _direction = 1;
	private bool _frozen = false;
	private ObjectType _powerup = ObjectType.None;
	private readonly ArrayList _items = new();
	
	// World management
	private int _currentChunkIndex = 0;
	private Managers.WorldManager _worldManager;
	
	
	// Multiplayer authority??? Not really sure wtf this does, but it locks you out of effecting other players
	private int _authority;

	// Provides a faster movement across the network!
	private Vector2 _syncPos = new Vector2(0, 0);
	
	// Get the gravity from the project settings so you can sync with rigid body nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	// ----- Functions ----- //
	
	// From the singleton world manager render the chunk(s) requested
	private void RenderChunk(int i)
	{
		Node2D currentChunk = this._worldManager.GetChunk(i);
		if (currentChunk == null)
		{
			return;
		}
		
		if (!GetTree().Root.GetChildren().Contains(currentChunk))
		{
			GetTree().Root.AddChild(currentChunk);
			currentChunk.GlobalTranslate(new Vector2(Managers.WorldManager.ChunkSize * (i), 0));
		}
			
		// GD.Print($"Loaded Chunk #{i}");
	}

	public override void _Ready()
	{
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(this.Name));
		_authority = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority();
		if (_authority == Multiplayer.GetUniqueId())
		{
			GetNode<Camera2D>("Camera2D").MakeCurrent();
		}
		
		// Init child nodes
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.Monitoring = true;
		_area2D.BodyEntered += GotHit;
		_healthBar = GetNode<ProgressBar>("ProgressBar");
	}

	// TODO: generalize this to make it work for ANY object hit maybe using a switch statement?
	private void GotHit(Node2D node2D)
	{
		// Ignore if not a projectile OR if hit myself
		if (node2D is not MyProjectile myProjectile)
		{
			return;
		}
			
		if (myProjectile is Fireball)
		{ 
			_health -= 10.0f;
			_sprite2D.Modulate = Colors.Red;
			GetTree().CreateTimer(0.1).Timeout += () =>
			{
				_sprite2D.Modulate = Colors.White;
			};
		}

		if (myProjectile is Iceball && !_frozen)
		{
			_frozen = true;
			_health -= 2.0f;
			_speed = 20.0f;
			_sprite2D.Modulate = Colors.Blue;
			GetTree().CreateTimer(5).Timeout += () =>
			{
				_sprite2D.Modulate = Colors.White;
				_frozen = false;
				_speed = 300.0f;
			};
		}
	}

	public void setHealthBar() {
		_healthBar.value = _health;
	}

	public override void _Process(double delta)
	{
		// UGH there has to be a better place to put this call!!!
		this._worldManager = Managers.WorldManager.GetWorldManager();
		
		// GD.Print($"Character Pos: <X={this.Position.X}, Y={this.Position.Y}>");
		// GD.Print($"Chunk Index {_currentChunkIndex}");
		
		// We want to increment the chunk index IF the player has passed the edge of the chunk boundary
		if (this.Position.X > (Managers.WorldManager.ChunkSize * (this._currentChunkIndex + 1)))
		{
			this._currentChunkIndex = Tools.Mod(this._currentChunkIndex + 1, Managers.WorldManager.NumChunks);
			
			// If they have passed the global boundary, teleport to other side!
			if (this.Position.X > Managers.WorldManager.ChunkSize * (Managers.WorldManager.NumChunks))
			{
				this.GlobalTranslate(new Vector2(Managers.WorldManager.ChunkSize * (Managers.WorldManager.NumChunks) * -1, 0));
			}
		}
		else if (this.Position.X < (Managers.WorldManager.ChunkSize * (this._currentChunkIndex)))
		{
			this._currentChunkIndex = Tools.Mod(this._currentChunkIndex - 1, Managers.WorldManager.NumChunks);
			
			// If they have passed the global boundary, teleport to other side!
			if (this.Position.X < 0)
			{
				this.GlobalTranslate(new Vector2(Managers.WorldManager.ChunkSize * (Managers.WorldManager.NumChunks), 0));
			}
		}
		
		RenderChunk(this._currentChunkIndex);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_authority == Multiplayer.GetUniqueId())
		{
			Vector2 velocity = Velocity;

			// Add the gravity.
			velocity.Y += _gravity * (float)delta;

			// Handle jump.
			if (Input.IsActionJustPressed("action_jump") && IsOnFloor())
			{
				velocity.Y = _jumpSpeed;
			}

			// Get the input direction.
			_direction = (int) Input.GetAxis("move_left", "move_right");
			velocity.X = _direction * _speed;
			this.Velocity = velocity;

			MoveAndSlide();
			_syncPos = GlobalPosition;
		}
		else
		{
			// Commenting this out cases lag
			GlobalPosition = GlobalPosition.Lerp(_syncPos, 0.9f);
			// GlobalPosition = _syncPos;
		}
		
		if (_direction != 0)
		{
			_sprite2D.FlipH = !(_direction > 0); // Flip if NOT moving right
		}
		
	}
	
	private void ThrowProjectile(MyProjectile myProjectile)
	{
		myProjectile.Direction = (_sprite2D.FlipH) ? -1 : 1; // LOL
		myProjectile.GlobalPosition = new Vector2(this.GlobalPosition.X + 50 * myProjectile.Direction, this.GlobalPosition.Y);

		// Create timer to destroy!
		this.GetTree().CreateTimer(0.7).Timeout += myProjectile.QueueFree;

		this.GetTree().Root.AddChild(myProjectile);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void Attack(string effect)
	{

		switch (effect)
		{
			case "FireFlower":
				ThrowProjectile(Fireball.Instantiate<MyProjectile>());
				break;
			case "IceFlower":
				MyProjectile iceball = Iceball.Instantiate<MyProjectile>();
				iceball.Modulate = new Color(0, 1, 1);
				ThrowProjectile(iceball);
				break;
			case "Boomerang":
				// ThrowProjectile(Fireball.Instantiate<MyProjectile>());
				break;
		}
	}
	
	//Handles Keybindings
	public override void _Input(InputEvent @event)
	{
		// Only allow key presses for local player!
		if (_authority != Multiplayer.GetUniqueId()) return;
		
		// Do we need this? Should items be picked up by default or manually?
		/*//Pickup Item (e)
		if (@event.IsActionPressed("action_grab"))
		{
			this.AddItem(ObjectType.Mushroom);
		}*/
		
		// Attack (j)
		if (@event.IsActionPressed("action_attack"))
		{
			Rpc(nameof(Attack), Enum.GetName(typeof(ObjectType), _powerup));
		}

		// Print Inventory (p)
		if (@event.IsActionPressed("display_inventory"))
		{
			this.DisplayInventory();
		}
	}

	public void GivePowerup(ObjectType objectType)
	{
		this._powerup = objectType;
	}

	public void RemovePowerUp(ObjectType objectType)
	{
		if (this._powerup == objectType)
		{
			this._powerup = ObjectType.None;
		}
	}
	
	public void AddItem(ObjectType objectType) {
		this._items.Add(objectType);
	}

	public void RemoveItemType(ObjectType objectType)
	{
		this._items.Remove(objectType);
	}
	
	private void DisplayInventory()
	{
		GD.Print($"Active Powerup:{Enum.GetName(typeof(ObjectType), _powerup)}");
		
		string s = "Inventory: [";
		foreach (ObjectType objectType in _items)
		{
			s += objectType switch
			{
				ObjectType.Coin => "Coin ",
				ObjectType.Mushroom => "Mushroom ",
				ObjectType.FireFlower => "FireFlower ",
				ObjectType.IceFlower => "IceFlower ",
				_ => "Illegal Item!"
			};
		}

		s += "]";
		GD.Print(s);
	}
	
}
