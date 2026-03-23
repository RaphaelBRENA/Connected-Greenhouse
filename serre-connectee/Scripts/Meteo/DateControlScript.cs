using Godot;
using System;

public partial class DateControlScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________

	public const int DAYSPERSEASON=14; //Nombre de jours par saison
	public static int DayNumber; //de jour 1 à 14
	public static string Season; //"Hiver","Printemps","Eté","Automne", saisons de 2 semaines chacune
	public static int Year; //de année 1 à pas de limite

	public static Godot.Collections.Dictionary DayWeather; //Paramètres du jour actuel

	public static Godot.Collections.Dictionary<string, int> WinterParameters; //Paramètres Min et max de l'hiver
	public static Godot.Collections.Dictionary<string, int> SpringParameters; //Paramètres Min et max du printemps
	public static Godot.Collections.Dictionary<string, int> SummerParameters; //Paramètres Min et max de l'été
	public static Godot.Collections.Dictionary<string, int> AutumnParameters; //Paramètres Min et max de l'automne

	//INITIALISATION ___________________________________________________________________________________

	public void Initialisation()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//Fonction d'initialisation de la météo
		WinterParameters = new Godot.Collections.Dictionary<string, int>{
			{"tempMax",15},
			{"tempMin",-5},
			{"humiMax",100},
			{"humiMin",0},
			{"lumiMax",3500},
			{"lumiMin",1000}
		};

		SpringParameters = new Godot.Collections.Dictionary<string,int>{
			{"tempMax",25},
			{"tempMin",8},
			{"humiMax",100},
			{"humiMin",0},
			{"lumiMax",90000},
			{"lumiMin",4500}
		};


		SummerParameters = new Godot.Collections.Dictionary<string,int>{
			{"tempMax",38},
			{"tempMin",24},
			{"humiMax",20},
			{"humiMin",0},
			{"lumiMax",100000},
			{"lumiMin",20000}
		};

		AutumnParameters = new Godot.Collections.Dictionary<string,int>{
			{"tempMax",22},
			{"tempMin",6},
			{"humiMax",100},
			{"humiMin",30},
			{"lumiMax",30000},
			{"lumiMin",4000},
		};

		DayNumber = 0;
		Year = 1;

	}

	public void ChangeDay(){
		if(DayNumber+1>(DAYSPERSEASON*4)){
			Year++;
			DayNumber = 1;
		}else{
			DayNumber++;
		}
		if(DayNumber<=DAYSPERSEASON){
			Season="Printemps";
		}
		if(DayNumber>DAYSPERSEASON && DayNumber<=(DAYSPERSEASON*2)){
			Season="Eté";
		}
		if(DayNumber>(DAYSPERSEASON*2) && DayNumber<=(DAYSPERSEASON*3)){
			Season="Automne";
		}
		if(DayNumber>(DAYSPERSEASON*3)){
			Season="Hiver";
		}

		ChangeWeather();
		PlantsEvolution();
		ShopEvolution();
	}

	public void PlantsEvolution(){
		//Fonction qui fait évoluer les plantes de la serre en fonction des paramètres de la serre
		/*GestionInterfacePot est constitué de
		/{"a1" : Humidity, Nitrogen, Compost,IsInfested,Chemicals, Disease, IsTreated, DaysSinceLastTreatment,
		ChemicalIntoxication, PlantName, PlantStage, TotalDaysNumber, TotalChemicalsDaysNumber, TotalDaysSinceLastStage,
		Capteur1, Capteur2, Capteur3}*/
		Godot.Collections.Dictionary<string,Godot.Collections.Array<string>> PotData;
		Godot.Collections.Array<string> CellData;
		Godot.Collections.Array<string> CellsArray = new Godot.Collections.Array<string>{
			"a1","a2","a3","b1","b2","b3","c1","c2","c3"
		};

		double CellHumidity = 0;
		int GlassHouseLuminosity = Global.GlassHouseLuminosity;
		int GlassHouseTemperature = Global.GlassHouseTemperature;

		for(int PotNumber = 1; PotNumber<=Global.POTSNUMBER; PotNumber++){

			PotData = Global.SaveInteract["Plan de Terre "+PotNumber].SendData();
			if(PotData is not null){
			foreach(string Cell in CellsArray){
				CellData = PotData[Cell];
				if(Convert.ToDouble(CellData[0])>CellHumidity){
					CellHumidity = Convert.ToDouble(CellData[0]);
				}
				//I - Evolution de la Plante
				if(CellData[9]!="0"){
					//1) Recréation de la plante
					Plant PlantTemp = new Plant(CellData[9]);
					PlantTemp.LoadPlant(Convert.ToInt32(CellData[10]),Convert.ToInt32(CellData[11]),Convert.ToInt32(CellData[12]),Convert.ToInt32(CellData[13]), Convert.ToBoolean(CellData[14]));
					//2) Appel de la fonction d'évolution
					PlantTemp.NewDay(CellHumidity, Convert.ToInt32(CellData[1]), Convert.ToDouble(GlassHouseTemperature), Convert.ToInt32(GlassHouseLuminosity),Convert.ToBoolean(CellData[8]));
					//3) Sauvegarde de la plante évoluée
					CellData[9] = PlantTemp.GetName();
			    	CellData[10] = PlantTemp.GetPlantStage().ToString();
        	    	CellData[11] = PlantTemp.GetTotalDaysNumber().ToString();
        	    	CellData[12] = PlantTemp.GetTotalChemicalDaysNumber().ToString();
        	    	CellData[13] = PlantTemp.GetTotalDaysSinceLastStage().ToString();
                    CellData[14] = PlantTemp.GetDead().ToString();
					if(PlantTemp.GetPlantStage()!=0){
						CellData[5] = (ChangeDisease(CellData[5],Convert.ToDouble(CellData[0]),Convert.ToBoolean(CellData[6]),PlantTemp.GetMaxHumidity()));
					}
				}
				//II - Rafraichissement des données de la case de plantation
				Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Programmation = Global.SaveInteract["Programmation"].SendData();
				if(Programmation is not null){
					if(!Convert.ToBoolean(Programmation["Pot"+PotNumber][5])){
						CellData[0]  = (ChangeHumidity(CellHumidity,GlassHouseTemperature)).ToString();
					}else{
						if(Convert.ToInt32(Programmation["Pot"+PotNumber][1])==0){
							if(Convert.ToDouble(CellData[0])<Convert.ToDouble(Programmation["Pot"+PotNumber][2])){
								CellData[0] = Programmation["Pot"+PotNumber][3];
							}
						}
						if(Convert.ToInt32(Programmation["Pot"+PotNumber][1])==1){
							if(Convert.ToDouble(CellData[0])>Convert.ToDouble(Programmation["Pot"+PotNumber][2])){
								CellData[0] = Programmation["Pot"+PotNumber][4];
							}
						}

					}
				}else{
					CellData[0]  = (ChangeHumidity(CellHumidity,GlassHouseTemperature)).ToString();
				}
				CellData[4] = (ChangeChemicals(Convert.ToDouble(CellData[4]))).ToString();
				if(Convert.ToBoolean(CellData[6])){
					CellData[6] = (ChangeTreatment(Convert.ToInt32(CellData[7]))).ToString();
					if(!Convert.ToBoolean(CellData[6])){
						CellData[7] = 0.ToString();
					}
				}
				PotData[Cell] = CellData;
			}
			Global.SaveInteract["Plan de Terre "+PotNumber].KeepData(PotData);
			}
		}
	}

	private double ChangeHumidity(double Humidity, double Temperature)
	{
		//Fonction de mise à jour de l'humidité
		Humidity = Humidity - (Mathf.Pow(2.71828,0.09*Temperature)-1);
		if(Humidity < 0)
		{
			Humidity = 0;
		}
		return Humidity;
	}

	private double ChangeChemicals(double Chemicals){
		//Fonction de mise à jour du niveau de pesticide
		Chemicals =  Chemicals-2;
		if(Chemicals>=0){
			return Chemicals;
		}
		return 0;
	}

	private bool ChangeTreatment(int DaysSinceLastTreatment){
		//Fonction de mise à jour du traitement à la bouillie bordelaise
		if(DaysSinceLastTreatment>=12){
			return false;
		}
		return true;
	}

	public string ChangeDisease(string Disease, double Humidity, bool IsTreated, int HumidityMax){
		//Fonction de mise à jour de la maladie de la plante
		if(IsTreated){
			Disease = "aucune";
		}else{
				string[] ListeMaladies = {"oïdium","rouille","cloque","mildiou"};
				int r = Random(0,3);
				if(Disease=="aucune" && r==1){
					if(Humidity>HumidityMax){
						int i = Random(0,3);
						Disease = ListeMaladies[i];
					}
				}
		}
		return Disease;
	}

	public void ShopEvolution(){
		//Fonction qui fait évoluer les prix et produits de la boutique
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data =  Global.SaveInteract["Boutique"].SendData();
		if(Data is not null){
			Data["MustChangeArticles"] = new Godot.Collections.Array<string>{true.ToString()};
			if(DayNumber==1 || DayNumber==8){
				Data["MustChangePrices"] = new Godot.Collections.Array<string>{true.ToString()};
			}
			Global.SaveInteract["Boutique"].KeepData(Data);
		}
	}


	//METHODES DE SAUVEGARDE __________________________________________________________________________


	public void LoadWeather(){
		//Fonction de chargement de l'interface CurrentWeather avec les données de sauvegarde
		Initialisation();
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Calendrier"].SendData();
		if(Data is not null){
			DayNumber = Convert.ToInt32(Data["Date"][0]);
			Season = Data["Date"][1];
			Year = Convert.ToInt32(Data["Date"][2]);
			DayWeather = new Godot.Collections.Dictionary{
				{"Aperçu",Data["Weather"][0]},
				{"Température",Convert.ToInt32(Data["Weather"][1])},
				{"Humidité",Convert.ToInt32(Data["Weather"][2])},
				{"Luminosité",Convert.ToInt32(Data["Weather"][3])}
			};
		}
		else{
			ChangeDay();
		}
	}

	public void ExportData(){
		//Fonction de sauvegarde des données de la météo et de la date
		Godot.Collections.Array<string> Date = new Godot.Collections.Array<string>{DayNumber.ToString(),Season,Year.ToString()};
		Godot.Collections.Array<string> Weather = new Godot.Collections.Array<string>{(string)DayWeather["Aperçu"],(string)DayWeather["Température"],(string)DayWeather["Humidité"],(string)DayWeather["Luminosité"]};
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>{
			{"Date",Date},
			{"Weather",Weather}
		};
		Global.SaveInteract["Calendrier"].KeepData(Data);
	}


	//METHODES ________________________________________________________________________________________


	public void ChangeWeather()
	{
		//Permet de changer les paramètres de météo, appelée lors du changement de jour
		int Temperature = 0;
		int Humidity = 0;
		int Luminosity = 0;
		string Overview = "";


		if (Season == "Printemps")
		{
			Temperature = Random(SpringParameters["tempMin"], SpringParameters["tempMax"]);
			Humidity = Random(SpringParameters["humiMin"], SpringParameters["humiMax"]);
			Luminosity = Random(SpringParameters["lumiMin"], SpringParameters["lumiMax"]);
		}
		if (Season == "Eté")
		{
			Temperature = Random(SummerParameters["tempMin"], SummerParameters["tempMax"]);
			Humidity = Random(SummerParameters["humiMin"], SummerParameters["humiMax"]);
			Luminosity = Random(SummerParameters["lumiMin"], SummerParameters["lumiMax"]);
		}
		if (Season == "Automne")
		{
			Temperature = Random(AutumnParameters["tempMin"], AutumnParameters["tempMax"]);
			Humidity = Random(AutumnParameters["humiMin"], AutumnParameters["humiMax"]);
			Luminosity = Random(AutumnParameters["lumiMin"], AutumnParameters["lumiMax"]);
		}
		if (Season == "Hiver")
		{
			Temperature = Random(WinterParameters["tempMin"], WinterParameters["tempMax"]);
			Humidity = Random(WinterParameters["humiMin"], WinterParameters["humiMax"]);
			Luminosity = Random(WinterParameters["lumiMin"], WinterParameters["lumiMax"]);
		}

		Overview = DefineOverview(Temperature, Humidity, Luminosity);

		// On applique les nouveaux paramètres
		DayWeather = new Godot.Collections.Dictionary{
				{"Aperçu",Overview},
				{"Température",Temperature},
				{"Humidité",Humidity},
				{"Luminosité",Luminosity}
		};
	}

	public int Random(int Min, int Max)
	{
		//Renvoie un int aléatoire entre Min et Max
		RandomNumberGenerator RandomGen = new RandomNumberGenerator();
		return (int)RandomGen.RandiRange(Min,Max);
	}


	public string DefineOverview(int CurrentTemperature, int CurrentHumidity, int CurrentLuminosity)
	{
		// Définie un intitulé météo (risque de pluie,etc...) en fonction des paramètres du jour

		Godot.Collections.Dictionary<string, int> Parameters = new Godot.Collections.Dictionary<string, int>();
		string Overview = "|";

		if (Season == "Printemps")
		{
			Parameters = SpringParameters;
		}
		if (Season == "Eté")
		{
			Parameters = SummerParameters;
		}
		if (Season == "Automne")
		{
			Parameters = AutumnParameters;
		}
		if (Season == "Hiver")
		{
			Parameters = WinterParameters;
		}

		// Humidité
		if (CurrentHumidity > 90)
		{
			Overview += " Pluie |";
		}
		else if (CurrentHumidity > ((Parameters["humiMin"] + Parameters["humiMax"]) / 2))
		{
			Overview += " Risque de pluie |";
		}

		// Luminosité
		if (CurrentLuminosity > ((Parameters["lumiMin"] + Parameters["lumiMax"]) / 2))
		{
			Overview += " Soleil |";
		}
		else
		{
			Overview += " Nuageux |";
		}

		// Températures
		if (CurrentTemperature > 35)
		{
			Overview += " Canicule |";
		}
		else if (CurrentTemperature < 0)
		{
			Overview += " Neige et Gel |";
		}
		return Overview;

	}


	public void CostEvolution()
	{
		Node2D ImprovementsInterface = (Node2D)((PackedScene)GD.Load("res://Scenes/Interface/AmeliorationInterface.tscn")).Instantiate();
		ImprovementsInterface.Visible = false;
		AddChild(ImprovementsInterface);
		UpgradePanel ImprovementsPanel = ImprovementsInterface.GetNode<UpgradePanel>("PanneauAmelioration");
		ImprovementsPanel._Ready();
	}


	public override void _Input(InputEvent @event)
    {
		//Fonction de détection des KeyboardKeys appuyées pour sortir de l'interface
        base._Input(@event);
		if (@event is InputEventKey EventKey) {
			if (EventKey.Pressed && (EventKey.Keycode == Key.Escape || EventKey.IsActionPressed("Interact"))) {
				ExportData();
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
    }
}
