using Godot;
using System;
using System.Collections.Generic;
using static Bullets;

public partial class Bullets : Node2D
{
    public const int BulletCount = 500;
    public const int SpeedMin = 20;
    public const int SpeedMax = 80;

    private Texture2D _bulletImage;

    private List<Bullet> _bullets = new List<Bullet>();
    private Rid _shape;

    public class Bullet
    {
        public Vector2 Position { get; set; }
        public float Speed { get; set; } = 1.0f;
        public Rid body;
    }

    public override void _Ready()
    {
        _bulletImage = (Texture2D)GD.Load("res://bullet.png");

        _shape = PhysicsServer2D.CircleShapeCreate();
        PhysicsServer2D.ShapeSetData(_shape, 8);

        for (int i = 0; i < BulletCount; i++)
        {
            var bullet = new Bullet();
            bullet.Speed = GD.RandRange(SpeedMin, SpeedMax);
            bullet.body = PhysicsServer2D.BodyCreate();

            PhysicsServer2D.BodySetSpace(bullet.body, GetWorld2D().Space);
            PhysicsServer2D.BodyAddShape(bullet.body, _shape);
            PhysicsServer2D.BodySetCollisionMask(bullet.body, 0);

            bullet.Position = new Vector2(
            (float)(GD.RandRange(0, GetViewportRect().Size.X) + GetViewportRect().Size.X),
            (float)GD.RandRange(0, GetViewportRect().Size.Y));
            var transform2d = new Transform2D();
            transform2d.Origin = bullet.Position;
            PhysicsServer2D.BodySetState(bullet.body, PhysicsServer2D.BodyState.Transform, transform2d);

            _bullets.Add(bullet);
        }
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        var transform2d = new Transform2D();

        var offset = GetViewportRect().Size.X + 16;

        foreach (var bullet in _bullets)
        {
            bullet.Position = bullet.Position with { X = bullet.Position.X - bullet.Speed * (float)delta };

            if (bullet.Position.X < -16)
            {
                bullet.Position = bullet.Position with { X = offset };
            }

            transform2d.Origin = bullet.Position;
            PhysicsServer2D.BodySetState(bullet.body, PhysicsServer2D.BodyState.Transform, transform2d);
        }
    }

    public override void _Draw()
    {
        var offset = -_bulletImage.GetSize() * 0.5f;

        foreach (var bullet in _bullets)
        {
            DrawTexture(_bulletImage, bullet.Position + offset);
        }
    }

    public override void _ExitTree()
    {
        foreach (var bullet in _bullets)
        {
            PhysicsServer2D.FreeRid(bullet.body);
        }

        PhysicsServer2D.FreeRid(_shape);
    }
}
