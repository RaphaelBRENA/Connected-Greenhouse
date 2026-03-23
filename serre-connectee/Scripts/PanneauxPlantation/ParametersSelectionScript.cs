using Godot;
using System;

public partial class ParametersSelectionScript : Control
{
	//ATTRIBUTS _______________________________________________________________________________________

	GridControlScript ControlCarre ;
	

	//READY ___________________________________________________________________________________________

	public override void _Ready(){
		//Fonction d'initialisation
		ControlCarre = GetNode<GridControlScript>("../../ControlCarre");
	}


	//PROCESS _________________________________________________________________________________________


	public override void _Process(double delta){
			//Fonction appelée en boucle pour mettre à jour l'affichage

			CellControlScript n = (CellControlScript)ControlCarre.GetSelectedCell(); //On récupère le noeud sélectionné par le joueur
			if(n is not null){
				int tauxHumidity = (int) n.GetHumidity(); //On arrondit à l'unité l'humidité
				int tauxPesticide = (int) n.GetPesticide(); //On arrondit à l'unité les pesticide
				if(n.HasHygrometer()){
					GetNode<Label>("Panneau/HBoxContainer/Humidite").SetText(tauxHumidity + "%");  //On l'affiche dans le label
				}else{
					GetNode<Label>("Panneau/HBoxContainer/Humidite").SetText("Ø   ");  
				}
				if(n.HasThermometer()){
					GetNode<Label>("Panneau/HBoxContainer/Temperature").SetText(Global.GlassHouseTemperature.ToString() + "°C");  
				}else{
					GetNode<Label>("Panneau/HBoxContainer/Temperature").SetText("Ø   ");  
				}
				if(n.HasLuxmeter()){
					GetNode<Label>("Panneau/HBoxContainer/Luminosite").SetText(Global.GlassHouseLuminosity.ToString() + " lux");  
				}else{
					GetNode<Label>("Panneau/HBoxContainer/Luminosite").SetText("Ø   ");  
				}
				GetNode<Label>("Panneau/VBoxContainer/Compost").SetText("A du compost : "+n.HasCompost());
				GetNode<Label>("Panneau/VBoxContainer/Pesticide").SetText("Taux Pesticide : "+tauxPesticide +"%");
				GetNode<Label>("Panneau/VBoxContainer/Traitement").SetText("A encore de la bouillie bordelaise : "+n.HasTreatment());
				GetNode<Label>("Panneau/VBoxContainer/Maladie").SetText("Maladie : "+n.GetDisease());
				Plant P = n.GetPlant();
				if(P is null){
					GetNode<Label>("Panneau/Nom").SetText("Aucune plante n'est plantée ici");
				}else{
					GetNode<Label>("Panneau/Nom").SetText(P.GetName());
				}
			
			}
	
	}
}
