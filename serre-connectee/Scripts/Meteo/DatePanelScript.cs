using Godot;
using System;

public partial class DatePanelScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________


	private DateControlScript NodeDate ;
	private const int NBLIGNESCALENDRIER = 2;


	//READY  __________________________________________________________________________________________

	public override void _Ready(){
		//Fonction d'initialisation 
		PopupManager.ShowPopup("Quand vous avez fini vos actions de la journée, venez ici pour passer au jour suivant. Vous pouvez aussi y constater la météo du jour !");
		NodeDate = GetNode<DateControlScript>("../ControlDate");
		NodeDate.LoadWeather();
		DisplayDate();
	}


	//SIGNAUX ________________________________________________________________________________________

	public void OnNextDayPressed(){
		Global.Coordinates = new Vector3(0, 0, 0);
		Global.RotationCamera = new Vector3(0, 0, 0);
		Global.RotationPlayer = new Vector3(0, 0, 0);

		NodeDate.ChangeDay();
		NodeDate.CostEvolution();
		NodeDate.ExportData();
		

		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
	}


	//METHODES _______________________________________________________________________________________


	public void DisplayDate(){
		
		Label Year = GetNode<Label>("Panneau/VBoxContainer/Annee");
		Label Overview = GetNode<Label>("Panneau/VBoxContainer/Intitule");
		Label Temperature = GetNode<Label>("Panneau/VBoxContainer/ParametresMeteo/Temperature");
		Label Humidity = GetNode<Label>("Panneau/VBoxContainer/ParametresMeteo/Humidite");
		Label Luminosity = GetNode<Label>("Panneau/VBoxContainer/ParametresMeteo/Luminosite");

		
		Year.SetText("Année : " + DateControlScript.Year.ToString());
		DisplaySeason();
		RestoreDefaultDay();
		DisplayDay();
		DisplayOverview();
		Overview.SetText((string)DateControlScript.DayWeather["Aperçu"]);
		Temperature.SetText((string)DateControlScript.DayWeather["Température"] + "°C");
		Humidity.SetText((string)DateControlScript.DayWeather["Humidité"] + "%");
		Luminosity.SetText((string)DateControlScript.DayWeather["Luminosité"] + " lux");

	}


	private void DisplaySeason(){
		
		Sprite2D SpriteSaison = GetNode<Sprite2D>("Panneau/VBoxContainer/Panel/Saison");
		string Season = DateControlScript.Season;
		if(Season=="Printemps"){
			Resource Image = ResourceLoader.Load("../Assets/Images/ImagesMeteo/TitrePrintemps.png");
			SpriteSaison.Texture = (Texture2D)Image;
		}
		else if(Season=="Eté"){
			Resource Image = ResourceLoader.Load("../Assets/Images/ImagesMeteo/TitreEte.png");
			SpriteSaison.Texture = (Texture2D)Image;
		}
		else if(Season=="Automne"){
			Resource Image = ResourceLoader.Load("../Assets/Images/ImagesMeteo/TitreAutomne.png");
			SpriteSaison.Texture = (Texture2D)Image;
		}
		else if(Season=="Hiver"){
			Resource Image = ResourceLoader.Load("../Assets/Images/ImagesMeteo/TitreHiver.png");
			SpriteSaison.Texture = (Texture2D)Image;
		}
	}

	private void DisplayDay(){
		Label LabelJour = GetNode<Label>("Panneau/VBoxContainer/Jour");
		int NumeroJour =DateControlScript.DayNumber;
		LabelJour.SetText("Jour : " + NumeroJour.ToString());
		
		//Récupération du fond de case correspondant à la saison
		string Season = DateControlScript.Season;
		Resource ImageCase = null;
		
		if(Season=="Printemps"){
			ImageCase = ResourceLoader.Load("../Assets/Images/ImagesMeteo/CasePrintemps.png");
		}
		else if(Season=="Eté"){
			ImageCase = ResourceLoader.Load("../Assets/Images/ImagesMeteo/CaseEte.png");
		}
		else if(Season=="Automne"){
			ImageCase = ResourceLoader.Load("../Assets/Images/ImagesMeteo/CaseAutomne.png");
		}
		else if(Season=="Hiver"){
			ImageCase = ResourceLoader.Load("../Assets/Images/ImagesMeteo/CaseHiver.png");
		}

		//Changement du fond et de la couleur de CurrentText des cases
		string Chemin = "Panneau/VBoxContainer/Panel/Ligne";

		int JourDuMois = NumeroJour%14;
		if(JourDuMois==0){JourDuMois=14;}
		if(JourDuMois<=7){
			Chemin+="1/Jour"+(JourDuMois).ToString();
			Label Day = GetNode<Label>(Chemin);
			Day.Set("custom_colors/font_color",new Color("#FFFFFF"));
			Day.GetChild<TextureRect>(0).Texture = (Texture2D)ImageCase;
		}else{
			Chemin+="2/Jour"+(JourDuMois).ToString();
			Label Day = GetNode<Label>(Chemin);
			Day.Set("custom_colors/font_color",new Color("#FFFFFF"));
			Day.GetChild<TextureRect>(0).Texture = (Texture2D)ImageCase;
		}

	}

	private void RestoreDefaultDay(){
		for(int j=1; j<=(DateControlScript.DAYSPERSEASON)/NBLIGNESCALENDRIER;j++){
			Label Day = GetNode<Label>("Panneau/VBoxContainer/Panel/Ligne1/Jour"+j.ToString());
			Day.Set("custom_colors/font_color",new Color("#605442"));
			Day.GetChild<TextureRect>(0).Texture = null;
		}
		for(int j=((DateControlScript.DAYSPERSEASON)/NBLIGNESCALENDRIER)+1;j<=DateControlScript.DAYSPERSEASON;j++){
			Label Day = GetNode<Label>("Panneau/VBoxContainer/Panel/Ligne2/Jour"+j.ToString());
			Day.Set("custom_colors/font_color",new Color("#605442"));
			Day.GetChild<TextureRect>(0).Texture = null;
		}
	}

	public void DisplayOverview(){
		
		string Overview = (string)DateControlScript.DayWeather["Aperçu"];
		TextureRect logoHumi = GetNode<TextureRect>("Panneau/LogoHumidite");
		TextureRect logoLumi = GetNode<TextureRect>("Panneau/LogoLuminosite");
		TextureRect logoTemp = GetNode<TextureRect>("Panneau/LogoTemperature");

		bool CurrentHumidity = false;
		bool CurrentLuminosity = false;
		int CurrentTemperature = 0;

		if(Overview.Find("Pluie")!=(-1) || Overview.Find("Risque de pluie")!=(-1)){
			CurrentHumidity = true;
		}
		if(Overview.Find("Soleil")!=(-1)){
			CurrentLuminosity = true;
		}
		if(Overview.Find("Canicule")!=(-1)){
			CurrentTemperature = 1;
		}
		if(Overview.Find("Neige et Gel")!=(-1)){
			CurrentTemperature = 2;
		}
		if(CurrentHumidity){
			logoHumi.Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesMeteo/Pluie.png");;
		}else{
			logoHumi.Texture = null;
		}
		if(CurrentLuminosity){
			logoLumi.Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesMeteo/Soleil.png");
		}else{
			logoLumi.Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesMeteo/Nuageux.png");
		}
		if(CurrentTemperature==1){
			logoTemp.Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesMeteo/Canicule.png");
		}else if(CurrentTemperature==2){
			logoTemp.Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesMeteo/Neige.png");
		}else{
			logoTemp.Texture = null;
		}

		

	}

}
