using Godot;
using System;

public partial class TileMap : Godot.TileMap
{
    private int _secretLayer; // You can have multiple layers if you make this an array.
    private bool _playerInSecret;
    private double _layerAlpha = 1.0;

    public TileMap()
    {
        for (int i = 0; i < GetLayersCount(); i++)
        {
            if (GetLayerName(i) == "Secret")
            {
                _secretLayer = i;
                break;
            }
        }
    }

    public override void _Ready()
    {
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        if (_playerInSecret)
        {
            if (_layerAlpha > 0.3)
            {
                _layerAlpha = Mathf.MoveToward(_layerAlpha, 0.3, delta); // Animate the layer transparency.
                SetLayerModulate(_secretLayer, new Color(1, 1, 1, (float)_layerAlpha));
            }
            else
            {
                SetProcess(false);
            }
        }
        else
        {
            if (_layerAlpha < 1.0)
            {
                _layerAlpha = Mathf.MoveToward(_layerAlpha, 1, delta);
                SetLayerModulate(_secretLayer, new Color(1, 1, 1, (float)_layerAlpha));
            }
            else
            {
                SetProcess(false);
            }
        }
    }

    public override bool _UseTileDataRuntimeUpdate(int layer, Vector2I coords)
    {
        if (layer == _secretLayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void _TileDataRuntimeUpdate(int layer, Vector2I coords, TileData tileData)
    {
        tileData.SetCollisionPolygonsCount(0, 0); // Remove collision for secret layer.
    }

    public void OnSecretDetectorBodyEntered(Node2D body)
    {
        if (body is not CharacterBody2D) return; // Detect player only

        _playerInSecret = true;
        SetProcess(true);
    }

    public void OnSecretDetectorBodyExited(Node2D body)
    {
        if (body is not CharacterBody2D) return;

        _playerInSecret = false;
        SetProcess(true);
    }
}