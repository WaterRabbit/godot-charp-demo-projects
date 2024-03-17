using Godot;
using System;

public partial class Player : Area2D
{
    private AnimatedSprite2D _animatedSprite2D;

    private int _touching;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        BodyShapeEntered += OnBodyShapeEntered;
        BodyShapeExited += OnBodyShapeExited;

        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            Position = eventMouseMotion.Position - new Vector2(0, 16);
        }
    }

    private void OnBodyShapeEntered(Rid bodyRid, Node2D body, long bodyShapeIndex, long localShapeIndex)
    {
        _touching++;
        _animatedSprite2D.Frame = 1;
    }

    private void OnBodyShapeExited(Rid bodyRid, Node2D body, long bodyShapeIndex, long localShapeIndex)
    {
        _touching--;
        if (_touching == 0)
        {
            _animatedSprite2D.Frame = 0;
        }
    }
}
