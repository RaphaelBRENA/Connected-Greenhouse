using Godot;
using System;

public partial class StartMenu : Control
{
	// VARIABLES  _______________________________________________________________________________________

	private bool Clicked = false;

	// READY  ___________________________________________________________________________________________

	public override void _Ready()
	{
	}

	// SIGNAUX __________________________________________________________________________________________

	public void OnStartPressed()
	{
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Lancement/SaveChoiceMenu.tscn"); 
	}

	public void OnOptionsPressed()
	{
		Global.ComesFromOptionsMenu = false;
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Options/OptionsMenu.tscn"); 
	}

	public void OnExitPressed() 
	{
		GetTree().Quit();
	}
}
