using Godot;
using System;

public partial class Hand : Control
{
	public TextureRect CurrentTexture;
	public Label Label;
	public bool Hover = false;
	public String Item = "";
	public int Quantity = 0;

    public override void _Ready()
    {
		CurrentTexture = GetNode<TextureRect>("TextureRect");
		Label = GetNode<Label>("Label");
    }

    public override void _PhysicsProcess(double delta)
    {
		GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
    }
}
