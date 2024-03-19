using Godot;
using System;
using System.Diagnostics;
using System.IO;

public partial class Main : Node
{
    private Sprite2D _icon;
    private Vector2 _iconStartPosition;

    private Label _countdownLabel;
    private Label _speedLabel;
    private Path2D _path;
    private TextureProgressBar _progress;
    private Slider _speedSlider;
    private Button _infinite;
    private Button _reset;
    private SpinBox _loops;
    private OptionButton _ease1;
    private OptionButton _trans1;
    private OptionButton _ease3;
    private OptionButton _trans3;
    private OptionButton _ease7;
    private OptionButton _trans7;

    private Tween _tween;
    private Tween _subTween;

    public override void _Ready()
    {
        _icon = GetNode<Sprite2D>("%Icon");
        _iconStartPosition = _icon.Position;

        _countdownLabel = GetNode<Label>("%CountdownLabel");
        _speedLabel = GetNode<Label>("%SpeedLabel");
        _path = GetNode<Path2D>("Path2D");
        _progress = GetNode<TextureProgressBar>("%Progress");
        _speedSlider = GetNode<Slider>("%SpeedSlider");
        _infinite = GetNode<Button>("%Infinite");
        _reset = GetNode<Button>("%Reset");
        _loops = GetNode<SpinBox>("%Loops");
        _ease1 = GetNode<OptionButton>("%Ease1");
        _trans1 = GetNode<OptionButton>("%Trans1");
        _ease3 = GetNode<OptionButton>("%Ease3");
        _trans3 = GetNode<OptionButton>("%Trans3");
        _ease7 = GetNode<OptionButton>("%Ease7");
        _trans7 = GetNode<OptionButton>("%Trans7");

    }

    public void StartAnimation()
    {
        // Reset the icon to original state.
        Reset();

        // Create the Tween. Also sets the initial animation speed.
        // All methods that modify Tween will return the Tween, so you can chain them.
        _tween = CreateTween().SetSpeedScale((float)_speedSlider.Value);

        // Sets the amount of loops. 1 loop = 1 animation cycle, so e.g. 2 loops will play animation twice.
        if (_infinite.ButtonPressed)
        {
            _tween.SetLoops();
        }
        else
        {
            _tween.SetLoops((int)_loops.Value);
        }

        // Step 1

        if (IsStepEnabled("MoveTo", 1.0f))
        {
            // Tween*() methods return a Tweener object. Its methods can also be chained, but
            //it's stored in a variable here for readability (chained lines tend to be long).
            // Note the usage of ^"NodePath". A regular "String" is accepted too, but it's very slightly slower.
            var tweener = _tween.TweenProperty(_icon, "position", new Vector2(400, 250), 1.0f);
            tweener.SetEase((Tween.EaseType)_ease1.Selected);
            tweener.SetTrans((Tween.TransitionType)_trans1.Selected);
        }

        // Step 2

        if (IsStepEnabled("ColorRed", 1.0f))
        {
            _tween.TweenProperty(_icon, "self_modulate", Colors.Red, 1.0f);
        }

        // Step 3

        if (IsStepEnabled("MoveRight", 1.0f))
        {
            // AsRelative() makes the value relative, so in this case it moves the icon
            // 200 pixels from the previous position.
            var tweener = _tween.TweenProperty(_icon, "position:x", 200.0, 1.0f).AsRelative();
            tweener.SetEase((Tween.EaseType)_ease3.Selected);
            tweener.SetTrans((Tween.TransitionType)_trans3.Selected);
        }
        if (IsStepEnabled("Roll", 0.0f))
        {
            // Parallel() makes the Tweener run in parallel to the previous one.
            var tweener = _tween.Parallel().TweenProperty(_icon, "rotation", Mathf.Tau, 1.0f);
            tweener.SetEase((Tween.EaseType)_ease3.Selected);
            tweener.SetTrans((Tween.TransitionType)_trans3.Selected);
        }

        // Step 4

        if (IsStepEnabled("MoveLeft", 1.0f))
        {
            _tween.TweenProperty(_icon, "position", Vector2.Left, 1.0f).AsRelative();
        }
        if (IsStepEnabled("Jump", 0.0f))
        {
            // Jump has 2 substeps, so to make it properly parallel, it can be done in a sub-Tween.
            // Here we are calling a lambda method that creates a sub-Tween.
            // Any number of Tweens can animate a single object in the same time.
            _tween.Parallel().TweenCallback(Callable.From(() =>
            {
                // Note that transition is set on Tween, but ease is set on Tweener.
                // Values set on Tween will affect all Tweeners (as defaults) and values
                // on Tweeners can override them.
                _subTween = CreateTween().SetSpeedScale((float)_speedSlider.Value).SetTrans(Tween.TransitionType.Sine);
                _subTween.TweenProperty(_icon, "position:y", -150.0, 0.5).AsRelative().SetEase(Tween.EaseType.Out);
                _subTween.TweenProperty(_icon, "position:y", 150.0, 0.5).AsRelative().SetEase(Tween.EaseType.In);
            }));
        }

        // Step 5


        if (IsStepEnabled("Blink", 2.0f))
        {
            // Loops are handy when creating some animations.
            for (var i = 0; i < 10; i++)
            {
                _tween.TweenCallback(Callable.From(_icon.Hide)).SetDelay(0.1);
                _tween.TweenCallback(Callable.From(_icon.Show)).SetDelay(0.1);
            }
        }

        // Step 6

        if (IsStepEnabled("Teleport", 0.5f))
        {
            // Tweening a value with 0 duration makes it change instantly.
            _tween.TweenProperty(_icon, "position", new Vector2(325, 325), 0);
            _tween.TweenInterval(0.5);
            // Binds can be used for advanced callbacks.
            _tween.TweenCallback(Callable.From(() => _icon.Position = new Vector2(680, 215)));
        }

        // Step 7


        if (IsStepEnabled("Curve", 3.5f))
        {
            // Method tweening is useful for animating values that can't be directly interpolated.
            // It can be used for remapping and some very advanced animations.
            // Here it's used for moving sprite along a path, using inline lambda function.
            var tweener = _tween.TweenMethod(Callable.From((float offset) => _icon.Position = _path.Position + _path.Curve.SampleBaked(offset)),
            0.0, _path.Curve.GetBakedLength(), 3.0).SetDelay(0.5);
            tweener.SetEase((Tween.EaseType)_ease7.Selected);
            tweener.SetTrans((Tween.TransitionType)_trans7.Selected);
        }

        // Step 8

        if (IsStepEnabled("Wait", 2.0f))
        {
            // ...
            _tween.TweenInterval(2);
        }

        // Step 9

        if (IsStepEnabled("Countdown", 3.0f))
        {
            _tween.TweenCallback(Callable.From(_countdownLabel.Show));

            _tween.TweenMethod(Callable.From<int>(DoCountdown), 4, 1, 3);

            _tween.TweenCallback(Callable.From(_countdownLabel.Hide));
        }

        // Step 10

        if (IsStepEnabled("Enlarge", 0.0f))
        {
            _tween.TweenProperty(_icon, "scale", Vector2.One * 5, 0.5).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
        }
        if (IsStepEnabled("Vanish", 1.0f))
        {
            _tween.Parallel().TweenProperty(_icon, "self_modulate:a", 0.0, 1.0);
        }

        if (_loops.Value > 1 && _infinite.ButtonPressed)
        {
            _tween.TweenCallback(Callable.From(_icon.Show));

            _tween.TweenCallback(Callable.From(() => _icon.SelfModulate = Colors.White));
        }

        // RESET step

        if (_reset.ButtonPressed)
        {
            _tween.TweenCallback(Callable.From(() => Reset(true)));
        }
    }

    public void DoCountdown(int v)
    {
        _countdownLabel.Text = v.ToString();
    }

    public void Reset(bool soft = false)
    {
        _icon.Position = _iconStartPosition;
        _icon.SelfModulate = Colors.White;
        _icon.Rotation = 0;
        _icon.Scale = Vector2.One;
        _icon.Show();
        _countdownLabel.Hide();

        if (soft)
        {
            return;
        }

        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        if (_subTween != null)
        {
            _subTween.Kill();
            _subTween = null;
        }

        _progress.MaxValue = 0;
    }

    private bool IsStepEnabled(string step, float expectedTime)
    {
        var enabled = GetNode<Button>("%" + step).ButtonPressed;
        if (enabled)
        {
            _progress.MaxValue += expectedTime;
        }

        return enabled;
    }

    public void PauseResume()
    {
        if (_tween != null && _tween.IsValid())
        {
            if (_tween.IsRunning())
            {
                _tween.Pause();
            }
            else
            {
                _tween.Play();
            }
        }

        if (_subTween != null && _subTween.IsValid())
        {
            if (_subTween.IsRunning())
            {
                _subTween.Pause();
            }
            else
            {
                _subTween.Play();
            }
        }
    }

    public void KillTween()
    {
        _tween?.Kill();
        _subTween?.Kill();
    }

    public void SpeedChanged(float value)
    {
        _tween?.SetSpeedScale(value);
        _subTween?.SetSpeedScale(value);
        _speedLabel.Text = $"{value}x";
    }

    public void InfiniteToggled(bool buttonPressed)
    {
        _loops.Editable = !buttonPressed;
    }

    public override void _Process(double delta)
    {
        if (_tween == null || !_tween.IsRunning())
        {
            return;
        }

        _progress.Value = _tween.GetTotalElapsedTime();
    }
}