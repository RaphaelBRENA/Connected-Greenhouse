using Godot;
using System;

public partial class CameraScript : Camera2D
{

	//ATTRIBUTS _______________________________________________________________________________________

	private Panel ActionsPanel; //Panneau droit de l'interface des pots
	private Panel SelectedCellParametersPanel; //Panneau gauche de l'interface des pots
	
	//READY ___________________________________________________________________________________________
	
	public override void _Ready(){
		//Fonction d'initialisation du script
		ActionsPanel = GetNode<Panel>("../ControlGeneral/Control/Panneau");
		SelectedCellParametersPanel = GetNode<Panel>("../ControlGeneral/ParametresSelection/Panneau");

	}

	//METHODE __________________________________________________________________________________________

	public void Centre(Node panneau){

		//Positions de la caméra
		Vector2 Left = new Vector2(300,300);
		Vector2 Right = new Vector2(800,300);
		Vector2 Centre = new Vector2(580,300);
		
		if(panneau == ActionsPanel)
		{ //Si le panneau ouvert est celui des actions
		
			this.SetPosition(Right); 

			SelectedCellParametersPanel.Hide();

			CellScript.Adjustment = new Vector2(220,1);

		}

		else if (panneau == SelectedCellParametersPanel)
		{ // Si le panneau ouvert est celui des Parameters de la case sélectionnée
			this.SetPosition(Left);

			ActionsPanel.Hide();
			
			CellScript.Adjustment = new Vector2(-265,1);
		
		}
		
		else
		{ //Si aucun panneau n'est ouvert, on Centre sur le carré de potager.

			this.SetPosition(Centre);
			CellScript.Adjustment = new Vector2(0,0);
		}
	}
}
