using System;
using Godot;

public class Player : KinematicBody2D
{
    public override void _Ready()
    {
    }

    public override void _Process(float delta)
    {
        MoveAndSlide(new Vector2(10, 0));
    }
}
