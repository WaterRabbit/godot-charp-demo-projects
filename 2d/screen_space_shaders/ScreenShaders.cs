using Godot;
using System;

public partial class ScreenShaders : Control
{
    private OptionButton _effect;
    private Control _effects;
    private OptionButton _picture;
    private Control _pictures;

    public override void _Ready()
    {
        _effect = GetNode<OptionButton>("Effect");
        _effects = GetNode<Control>("Effects");
        _picture = GetNode<OptionButton>("Picture");
        _pictures = GetNode<Control>("Pictures");

        foreach (var node in _pictures.GetChildren())
        {
            _picture.AddItem("PIC: " + node.Name);
        }

        foreach (var node in _effects.GetChildren())
        {
            _effect.AddItem("FX: " + node.Name);
        }
    }

    public void OnPictureItemSelected(long index)
    {
        for (int i = 0; i < _pictures.GetChildCount(); i++)
        {
            if (index == i)
            {
                _pictures.GetChild<CanvasItem>(i).Show();
            }
            else
            {
                _pictures.GetChild<CanvasItem>(i).Hide();
            }
        }

        GD.Print("Hello");
    }
    public void OnEffectItemSelected(long index)
    {
        for (int i = 0; i < _effects.GetChildCount(); i++)
        {
            if (index == i)
            {
                _effects.GetChild<CanvasItem>(i).Show();
            }
            else
            {
                _effects.GetChild<CanvasItem>(i).Hide();
            }
        }
    }
}
