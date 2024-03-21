using Godot;
using System;
using System.Runtime.InteropServices.ObjectiveC;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400;

    private AnimatedSprite2D _sprite;
    private GpuParticles2D _trail;
    private CollisionShape2D _collisionShape;

    private Vector2 _screenSize;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _trail = GetNode<GpuParticles2D>("Trail");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _screenSize = GetViewportRect().Size;
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }
        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }
        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }
        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            _sprite.Play();
        }
        else
        {
            _sprite.Stop();
        }

        Position += velocity * (float)delta;
        Position = Position.Clamp(Vector2.Zero, _screenSize);

        if (velocity.X != 0)
        {
            _sprite.Animation = "right";
            _sprite.FlipV = false;
            _trail.Rotation = 0;
            _sprite.FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            _sprite.Animation = "up";
            Rotation = velocity.Y > 0 ? (float)Math.PI : 0;
        }
    }

    public void Start(Vector2 pos)
    {
        Position = pos;
        Rotation = 0;
        Show();
        _collisionShape.Disabled = false;
    }

    public void OnPlayerBodyEntered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        _collisionShape.SetDeferred("disabled", true);
    }
}
