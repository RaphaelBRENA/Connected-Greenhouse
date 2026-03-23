using Godot;
using System;

public partial class EncyclopediaInterface : Node2D
{
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Pressed && EventKey.Keycode == Key.Escape)
			{
				GetTree().ChangeSceneToFile("Scenes/Gameplay.tscn");
			}
		}
	}
}
