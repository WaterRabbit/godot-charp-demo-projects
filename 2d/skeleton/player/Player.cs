using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public static class States
{
    public static string Idle = "idle";
    public static string Walk = "walk";
    public static string Run = "run";
    public static string Fly = "fly";
    public static string Fall = "fall";
}

public partial class Player : CharacterBody2D
{
    public const float WalkSpeed = 200.0f;
    public const float AccelerationSpeed = WalkSpeed * 6.0f;
    public const float JumpVelocity = -400.0f;
    // Maximum speed at which the player can fall.
    public const float TerminalVelocity = 400.0f;

    private bool _fallingSlow;
    private bool _fallingFast;
    private double _noMoveHorizontalTime = 0.0f;

    private float _gravity;
    private Node2D _sprite;
    private float _spriteScale;

    private AnimationTree _animationTree;

    public override void _Ready()
    {
        _sprite = GetNode<Node2D>("Sprite2D");
        _animationTree = GetNode<AnimationTree>("AnimationTree");

        _gravity = (float)ProjectSettings.Singleton.GetSetting("physics/2d/default_gravity", 0.0f).AsDouble();
        _spriteScale = _sprite.Scale.X;
        _animationTree.Active = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        bool isJumping = false;
        if (Input.IsActionJustPressed("jump"))
        {
            isJumping = TryJump();
        } else if (Input.IsActionJustReleased("jump") && Velocity.Y < 0.0)
        {
            // The player let go of jump early, reduce vertical momentum.
            Velocity = Velocity with { Y = Velocity.Y * 0.6f };
        }
        // Fail.
        Velocity = Velocity with { Y = MathF.Min(TerminalVelocity, Velocity.Y + _gravity * (float)delta) };

        var direction = Input.GetAxis("move_left", "move_right") * WalkSpeed;
        Velocity = Velocity with { X = Mathf.MoveToward(Velocity.X, direction, AccelerationSpeed * (float)delta) };

        if (_noMoveHorizontalTime > 0) 
        {
            // After doing a hard fall, don't move for a short time.
            Velocity = Velocity with { X = 0 };
            _noMoveHorizontalTime -= delta;
        }

        if (!Mathf.IsZeroApprox(Velocity.X))
        {
            if (Velocity.X > 0)
            {
                _sprite.Scale = _sprite.Scale with { X = _spriteScale };
            } else 
            {
                _sprite.Scale = _sprite.Scale with { X = -_spriteScale };
            }
        }

        MoveAndSlide();

        // After applying our motion, update our animation to match.

        // Calculate falling speed for animation purposes.
        if (Velocity.Y >= TerminalVelocity)
        {
            _fallingFast = true;
            _fallingSlow = false;
        } else if (Velocity.Y > 300)
        {
            _fallingSlow = true;
        }

        if (isJumping) 
        {
            _animationTree.Set("parameters/jump/request", (int)AnimationNodeOneShot.OneShotRequest.Fire); 
        }

        if (IsOnFloor())
        {
// Most animations change when we run, land, or take off.
            if (_fallingFast) 
            {
                _animationTree.Set("parameters/land_hard/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
                _noMoveHorizontalTime = 0.4f;
            } else if (_fallingSlow)
            {
                _animationTree.Set("parameters/land/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }

            if (Math.Abs(Velocity.X) > 50)
            {
                _animationTree.Set("parameters/state/transition_request", States.Run);
                _animationTree.Set("parameters/run_timescale/scale", Math.Abs(Velocity.X) / 60);
            }
            else if (Velocity.X > 0)
            {
                _animationTree.Set("parameters/state/transition_request", States.Walk);
                _animationTree.Set("parameters/run_timescale/scale", Math.Abs(Velocity.X) / 12);
            }
            else
            {
                _animationTree.Set("parameters/state/transition_request", States.Idle);
            }

            _fallingFast = false;
            _fallingSlow = false;
        } else 
        {
            if (Velocity.Y > 0) 
            {
                _animationTree.Set("parameters/state/transition_request", States.Fall);
            }
            else 
            {
                _animationTree.Set("parameters/state/transition_request", States.Fly);
            }
        }
    }

    public bool TryJump() 
    {
        if (IsOnFloor())
        {
            Velocity = Velocity with { Y = JumpVelocity };
            return true;
        }
        return false;
    }
}
