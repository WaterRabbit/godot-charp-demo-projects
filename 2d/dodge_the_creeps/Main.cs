using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private Timer _mobTimer;
    private Timer _scoreTimer;
    private Timer _startTimer;
    private Hud _hud;
    private AudioStreamPlayer _music;
    private AudioStreamPlayer _deathSound;
    private Player _player;
    private Marker2D _startPosition;

    private int _score;

    public override void _Ready()
    {
        GD.Randomize();

        _mobTimer = GetNode<Timer>("MobTimer");
        _scoreTimer = GetNode<Timer>("ScoreTimer");
        _startTimer = GetNode<Timer>("StartTimer");
        _hud = GetNode<Hud>("HUD");
        _music = GetNode<AudioStreamPlayer>("Music");
        _deathSound = GetNode<AudioStreamPlayer>("DeathSound");
        _player = GetNode<Player>("Player");
        _startPosition = GetNode<Marker2D>("StartPosition");
    }

    public void GameOver()
    {
        _scoreTimer.Stop();
        _mobTimer.Stop();
        _hud.ShowGameOver();
        _music.Stop();
        _deathSound.Play();
    }

    public void NewGame()
    {
        GetTree().CallGroup("mobs", "queue_free");
        _score = 0;
        _player.Start(_startPosition.Position);
        _startTimer.Start();
        _hud.UpdateScore(_score);
        _hud.ShowMessage("Get Ready");
        _music.Play();
    }

    public void OnMobTimerTimeout()
    {
        // Create a new instance of the Mob scene.
        var mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");

        mobSpawnLocation.Progress = GD.Randi();

        // Set the mob's direction perpendicular to the path direction.
        var direction = mobSpawnLocation.Rotation + MathF.PI / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Math.PI / 4, Math.PI / 4);

        mob.Rotation = direction;

        // Choose the velocity for the mob.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0.0f);

        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }

    public void OnScoreTimerTimeout()
    {
        _score++;
        _hud.UpdateScore(_score);
    }

    public void OnStartTimerTimeout()
    {
        _mobTimer.Start();
        _scoreTimer.Start();
    }
}
