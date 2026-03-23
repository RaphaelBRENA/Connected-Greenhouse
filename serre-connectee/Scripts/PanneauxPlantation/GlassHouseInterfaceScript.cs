using Godot;
using System;

public partial class GlassHouseInterfaceScript: Node2D
{

	//INPUT __________________________________________________________________________________________

	public override void _Ready(){
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _Input(InputEvent @event)
    {
		//Fonction de détection des KeyboardKeys appuyées pour sortir de l'interface
		if (@event is InputEventKey EventKey) {
			if (EventKey.Pressed && (EventKey.Keycode == Key.Escape || EventKey.IsActionPressed("Interact"))) {
				GetChild<GridControlScript>(1).ExportData();
				Input.SetCustomMouseCursor(null);
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn"); 
				Input.MouseMode = Input.MouseModeEnum.Visible;			
			}
		}
    }
}
