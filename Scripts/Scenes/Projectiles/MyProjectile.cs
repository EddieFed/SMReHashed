using Godot;

namespace SuperMarioRehashed.Scripts.Scenes.Projectiles;

public abstract partial class MyProjectile : CharacterBody2D
{
    public int Direction = 1;
    public float Speed = 300.0f;
    public float Gravity = 22.0f;
    public float Bounce = -350f;

    public int ProjectileOwner { get; set; }
    
    // By default, bounce and move. If a projectile does not follow this, override this!
    public override void _PhysicsProcess(double delta)
    {
        this.RotationDegrees += 20.0f * this.Direction;
		
        // Y component //
        Vector2 velocity = this.Velocity;
        velocity.Y += this.Gravity;

        if (IsOnWall())
        {
            this.Direction *= -1;
        }
		
        if (IsOnFloor())
        {
            velocity.Y += this.Bounce;
        }
		
        // X component //
        velocity.X = this.Direction * this.Speed;

        // //
        this.Velocity = velocity;
        MoveAndSlide();
    }
}