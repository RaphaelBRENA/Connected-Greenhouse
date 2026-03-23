using Godot;
using System;

public partial class GridControlScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________
	private static string ColliderName = "";
	private Node SelectedCell; //Case sélectionnée par le joueur
	private int CellsNumber; //Nombre de cases du carré de potager
	private readonly string[] CellsNamesArray
	= {"a1","a2","a3","b1","b2","b3","c1","c2","c3"}; //Liste des noms des boutons de cases
	Godot.Collections.Dictionary CellsArray; //Liste des Nodes de contrôleurs de case.
	private string PotName;

	//READY ___________________________________________________________________________________________

	
	public override void _Ready(){
		//Fonction d'initialisation de l'interface centrale de Pot
		PopupManager.ShowPopup("Vous êtes entré dans l'interface d'un pot. C'est ici que vous plantez vos graines, placez vos capteurs et veillez à la bonne croissance de vos plantes !");
		CellsNumber = 9;
		CellsArray  = new Godot.Collections.Dictionary();
		SelectedCell = null;
		
		//On remplit CellsArray avec les Nodes des contrôleurs de case:
		for(int i=0; i<CellsNumber; i++){
			CellsArray.Add(CellsNamesArray[i],GetNode<Control>("ControlCase"+i)); // +"/"+CellsNamesArray[i]) pour accéder au bouton
		}

		//On charge les données de l'interface
		LoadPot();
	}


	//METHODES ________________________________________________________________________________________

	public static void SetColliderName(string name)
	{
		ColliderName = name;
	}
	public void SetSelectedCell(Node SelectedNode){ //Utilisé par les contrôleurs de boutons lorsqu'ils sont cliqués avec le curseur normal
		SelectedCell = SelectedNode;
	}

	public Node GetSelectedCell(){
		return SelectedCell;
	}


	//METHODES DE SAUVEGARDE ___________________________________________________________________________

	private void LoadPot(){
		//Fonction de chargement des données du Pot avec lequel le joueur a intéragi.
		if(ColliderName!=""){
			PotName = ColliderName.Substring(0,15);
			Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data;
			SaveManager PotData = (SaveManager)Global.SaveInteract[PotName];
			Data = PotData.SendData();
			if(Data is not null){
				foreach(string NomCase in CellsNamesArray){
					((CellControlScript)CellsArray[NomCase]).LoadCell(Data[NomCase]);
				}
			}
		}
	}

	public void ExportData()
	//Fonction d'envoi des données de plantations contenues dans ce Pot
    {
		SaveManager PotData = (SaveManager)Global.SaveInteract[PotName];
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Export = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
		for (int i = 0; i < CellsNumber; i++){
			Export.Add((string)CellsNamesArray[i], ((CellControlScript) CellsArray[CellsNamesArray[i]]).GetData());
		}
        PotData.KeepData(Export);
    } 
}
