using Godot;
using System;

public partial class Player : CharacterBody2D
{
    const int WalkForce = 600;
    const int WalkSpeed = 200;
    const int StopForce = 1300;
    const int JumpSpeed = 200;

    private float _gravity;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.Singleton.GetSetting("physics/2d/default_gravity", 0.0f).AsDouble();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Horizontal movement code. First, get the player's input.
        var walk = WalkForce * (Input.GetAxis("move_left", "move_right"));
        // Slow down the player if they're not trying to move.
        if (Math.Abs(walk) < WalkForce * 0.2)
        {
            // The velocity, slowed down a bit, and then reassigned.
            Velocity = Velocity with { X = Mathf.MoveToward(Velocity.X, 0, StopForce * (float)delta) };
        }
        else
        {
            Velocity = Velocity with { X = Velocity.X + walk * (float)delta };
        }
        // Clamp to the maximum horizontal movement speed.
        Velocity = Velocity with { X = Math.Clamp(Velocity.X, -WalkSpeed, WalkSpeed) };

        // Vertical movement code. Apply gravity.
        Velocity = Velocity with { Y = Velocity.Y + _gravity * (float)delta };


        // Move based on the velocity and snap to the ground.
        // TODO: This information should be set to the CharacterBody properties instead of arguments: snap, Vector2.DOWN, Vector2.UP
        // TODO: Rename velocity to linear_velocity in the rest of the script.
        MoveAndSlide();

        // Check for jumping. is_on_floor() must be called after movement code.
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            Velocity = Velocity with { Y = -JumpSpeed };
        }
    }
}
