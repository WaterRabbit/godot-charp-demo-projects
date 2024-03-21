using Godot;
using System;

public partial class Mob : RigidBody2D
{
    private AnimatedSprite2D _sprite;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite.Play();
        var mobTypes = _sprite.SpriteFrames.GetAnimationNames();
        _sprite.Animation = mobTypes[GD.Randi() % mobTypes.Length];
    }

    private void OnVisibilityNotifier2DScreenExited()
    {
        QueueFree();
    }
}
