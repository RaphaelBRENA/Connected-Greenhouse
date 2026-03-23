using Godot;
using System;

public partial class PanelScript : Panel
{

	//READY ___________________________________________________________________________________________
	
	public override void _Ready(){
		DisplaySeedsNodes(false);
	}

	//SIGNAUX _________________________________________________________________________________________

	public void OnMenuPressed(){
		//Lorsque le bouton "Menu" est appuyé, le menu (Panel Panneau) apparaît ou disparaît.
		var Panneau = GetNode<Panel>("../Panneau");
		if(!Panneau.IsVisible()){
			Panneau.Show();
			GetNode<CameraScript>("../../../Camera2D").Centre(this);
			if(GetParent().Name=="Control"){
				PopupManager.ShowPopup("Commencez par déposer une graine dans une case. Ensuite vous pourrez l'arroser pour la faire pousser !");
				DisplaySeedsNodes(true);
			}else{
				PopupManager.ShowPopup("C'est ici que vous pouvez constater l'état de vos plantes. Mais attention, il faudra peut-être avoir placé un capteur au préalable !");
				DisplaySeedsNodes(false);
			}
		}else{
			Panneau.Hide();
			GetNode<CameraScript>("../../../Camera2D").Centre(GetNode<GridControlScript>("../../../ControlCarre"));
			DisplaySeedsNodes(false);
		}
	}

	//METHODE _________________________________________________________________________________________

	public void DisplaySeedsNodes(bool B)
	{
		//Affiche les noeuds des graines et capteurs si B vaut vrai, et les cache sinon
		var nodes = GetTree().GetNodesInGroup("Graines");
		var nodes2 = GetTree().GetNodesInGroup("PetitesCases");
		var nodes3 = GetTree().GetNodesInGroup("Capteurs");
		foreach(Node2D Node in nodes)
		{
			Node.Visible = B;
		}
		foreach(Node2D Node in nodes2)
		{
			Node.Visible = B;
		}
		foreach(Node2D Node in nodes3)
		{
			Node.Visible = B;
		}
	}

}
