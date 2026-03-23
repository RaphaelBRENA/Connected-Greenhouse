using Godot;
using System;
using System.Collections;

public partial class Crosshair : CenterContainer
{
	//READY _______________________________________________________________________________________________
	public override void _Ready()
	{
		QueueRedraw();
	}

	//DRAW _______________________________________________________________________________________________

	public override void _Draw()
	{
		//Fonction qui dessine le point de la visée
		DrawCircle(new Vector2(0, 0), 2.0f, Colors.Black);
	}
}
