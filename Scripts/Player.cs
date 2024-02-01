using System;
using System.Collections.Generic;
using Godot;

namespace SuperMarioRehashed.Scripts;

public partial class Player : CharacterBody2D
{
    private float _speed = 300.0f;
    private float _jumpSpeed = -400.0f;
    private WorldManager _worldManager;
    private int _currentChunkIndex = 0;
    private LinkedList<Node2D> _activeChunks = new LinkedList<Node2D>();
    
    // Get the gravity from the project settings so you can sync with rigid body nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public Player()
    {
        // Constructor
    }

    // We need to grab the auto loaded world manager!
    public override void _Ready()
    {

    }
    
    private void RenderChunk(int i)
    {
        
    }
    
    public override void _Process(double delta)
    {
        GD.Print($"Character Pos: <X={this.Position.X}, Y={this.Position.Y}>");
        GD.Print($"Chunk Index {_currentChunkIndex}");
        // From the singleton world manager, grab the current active chunks (current and adjacent chunks)
        WorldManager worldManager = WorldManager.GetWorldManager();
        
        float chunkSizeX = worldManager.GetChunkWidth();
        GD.Print($"Chunk Size in pixels: {chunkSizeX}");
        if (this.Position.X <= 0)
        {
            _currentChunkIndex = WorldManager.NumChunks - 1;
            this.GlobalTranslate(new Vector2(chunkSizeX * (WorldManager.NumChunks), 0));
        }
        else if (this.Position.X >= chunkSizeX * (WorldManager.NumChunks))
        {
            _currentChunkIndex = 0;
            this.GlobalTranslate(new Vector2(chunkSizeX * (WorldManager.NumChunks) * -1, 0));
        }
        else if (this.Position.X > (chunkSizeX * (_currentChunkIndex + 1)))
        {
            // We want to increment the chunk index IF the player has passed the edge of the chunk boundary
            _currentChunkIndex = (_currentChunkIndex + 1) % WorldManager.NumChunks;
        }
        
        
        Node2D currentChunk = worldManager.GetChunk(_currentChunkIndex);
        if (currentChunk == null)
        {
            return;
        }
        
        if (!GetTree().Root.GetChildren().Contains(currentChunk))
        {
            GetTree().Root.AddChild(currentChunk);
            currentChunk.GlobalTranslate(new Vector2(chunkSizeX * (_currentChunkIndex), 0));
        }
            
        GD.Print($"Loaded Chunk #{_currentChunkIndex}");
    }
    
    public override void _PhysicsProcess(double delta)
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
        float direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * _speed;
        this.Velocity = velocity;

        // Get child Image Node
        Sprite2D sprite2D = GetNode<Sprite2D>("Sprite2D");
        if (velocity.X != 0)
        {
            sprite2D.FlipH = !(velocity.X > 0); // Flip if NOT moving right
        }

        MoveAndSlide();
    }
}