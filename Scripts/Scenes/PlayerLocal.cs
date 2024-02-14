using System.Collections.Generic;
using Godot;
using SuperMarioRehashed.Scripts.Util;

namespace SuperMarioRehashed.Scripts.Scenes;

public partial class PlayerLocal : CharacterBody2D
{
    private float _speed = 300.0f;
    private float _jumpSpeed = -400.0f;
    private float _direction = 0.0f;
    
    // Multiplayer authority??? Not really sure wtf this does, but it locks you out of effecting other players

    // Provides a faster movement across the network!
    private Vector2 _syncPos = new Vector2(0, 0);
    
    // Get the gravity from the project settings so you can sync with rigid body nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private float _baseWidth;
    // From the singleton world manager render the chunk(s) requested

    public override void _Ready()
    {
        _baseWidth = GetWindow().Size.X;
    }

    public override void _Process(double delta)
    {
        if (this.Position.X > _baseWidth)
        {
            GD.Print("Hit the right side");
            this.GlobalTranslate(new Vector2(-_baseWidth, 0));
        } else if (this.Position.X < 0)
        {
            GD.Print("Hit the left side");
            this.GlobalTranslate(new Vector2(_baseWidth, 0));
        }
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
            _direction = Input.GetAxis("move_left", "move_right");
            velocity.X = _direction * _speed;
            this.Velocity = velocity;

            MoveAndSlide();
            _syncPos = GlobalPosition;
        
        // Get child Image Node
        Sprite2D sprite2D = GetNode<Sprite2D>("Sprite2D");
        if (_direction != 0)
        {
            sprite2D.FlipH = !(_direction > 0); // Flip if NOT moving right
        }
        
    }
}