using System.Collections.Generic;
using Godot;
using SuperMarioRehashed.Scripts.Util;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class Player : CharacterBody2D
{
    private float _speed = 300.0f;
    private float _jumpSpeed = -400.0f;
    private Managers.WorldManager _worldManager;
    private int _currentChunkIndex = 0;
    private LinkedList<Node2D> _activeChunks = new LinkedList<Node2D>();
    
    private float _direction = 0.0f;
    
    // Multiplayer authority??? Not really sure wtf this does, but it locks you out of effecting other players
    private int _authority;

    // Provides a faster movement across the network!
    private Vector2 _syncPos = new Vector2(0, 0);
    
    // Get the gravity from the project settings so you can sync with rigid body nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    
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
            _direction = Input.GetAxis("move_left", "move_right");
            velocity.X = _direction * _speed;
            this.Velocity = velocity;

            MoveAndSlide();
            _syncPos = GlobalPosition;
        }
        else
        {
            GlobalPosition = GlobalPosition.Lerp(_syncPos, 0.5f);
        }
        
        // Get child Image Node
        Sprite2D sprite2D = GetNode<Sprite2D>("Sprite2D");
        if (_direction != 0)
        {
            sprite2D.FlipH = !(_direction > 0); // Flip if NOT moving right
        }
        
    }
}