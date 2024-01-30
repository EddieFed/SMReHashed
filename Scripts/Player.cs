using Godot;

namespace SuperMarioRehashed.Scripts;

public partial class Player : CharacterBody2D
{
    private float _speed = 300.0f;
    private float _jumpSpeed = -400.0f;

    // Get the gravity from the project settings so you can sync with rigid body nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

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
        var direction = Input.GetAxis("move_left", "move_right");
        velocity.X = direction * _speed;
        this.Velocity = velocity;

        // Get child Image Node
        var sprite2D = GetNode<Sprite2D>("Sprite2D");
        if (velocity.X != 0)
        {
            sprite2D.FlipH = !(velocity.X > 0); // Flip if NOT moving right
        }

        MoveAndSlide();
    }
}