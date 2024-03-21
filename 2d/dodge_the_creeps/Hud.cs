using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    private Label _messageLabel;
    private Label _scoreLabel;
    private Button _startButton;
    private Timer _messageTimer;

    public override void _Ready()
    {
        _messageLabel = GetNode<Label>("MessageLabel");
        _scoreLabel = GetNode<Label>("ScoreLabel");
        _startButton = GetNode<Button>("StartButton");
        _messageTimer = GetNode<Timer>("MessageTimer");
    }

    public void ShowMessage(string text)
    {
        _messageLabel.Text = text;
        _messageLabel.Show();
        _messageTimer.Start();
    }

    public async void ShowGameOver()
    {
        ShowMessage("Game Over");
        await ToSignal(_messageTimer, Timer.SignalName.Timeout);
        _messageLabel.Text = "Dodge the\ncreeps";
        _messageLabel.Show();
        await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);
        _startButton.Show();
    }

    public void UpdateScore(int score)
    {
        _scoreLabel.Text = score.ToString();
    }

    public void OnStartButtonPressed()
    {
        _startButton.Hide();
        EmitSignal(Hud.SignalName.StartGame);
    }

    public void OnMessageTimerTimeout()
    {
        _messageLabel.Hide();
    }
}
