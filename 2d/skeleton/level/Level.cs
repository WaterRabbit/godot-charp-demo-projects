using Godot;
using System;

public partial class Level : Node2D
{
    public override void _Ready()
    {
        var camera = GetNode<Camera2D>("SkeletalPlayer/Camera2D");
        var minPosition = GetNode<Marker2D>("CameraLimit_min").GlobalPosition;
        var maxPosition = GetNode<Marker2D>("CameraLimit_max").GlobalPosition;
        camera.LimitLeft = (int)minPosition.X;
        camera.LimitTop = (int)minPosition.Y;
        camera.LimitRight = (int)maxPosition.X;
        camera.LimitBottom = (int)maxPosition.Y;

    }
}
