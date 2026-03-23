using Godot;
using System;
using System.Collections.Generic;


public partial class CellControlScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________


	private double Humidity; //De 0% à 100%
	private double Nitrogen; // de 0% à 5%,  le composte doit augmenter son taux, et trop d'arrosage le diminuer
	private bool Compost; //true s'il y a du composte, false sinon. Ca revient à false après chaque ramassage de plante.

	private bool IsInfested; //true s'il y a des insectes (aléatoire), false sinon
	private double Chemicals; //De 0% à 100%, néfaste au-delà de 30%, diminue au fil du temps
	private string Disease; //"aucune" si pas de maladie, sinon (aléatoire) "oïdium","rouille","cloque","mildiou"
	private bool IsTreated; //true s'il y a application de bouillie bordelaise dans les 15 derniers jours, false sinon

	private int DaysSinceLastTreatment; //nombre de jours depuis le traitement à la bouillie bordelaise
	private bool ChemicalIntoxication; //true quand Chemicals>30, false sinon

	private Plant CurrentPlant; //Objet Plant contenant les informations de la plante plantée, égale à null si rien n'est planté

	private Node ControlledCell; //Noeud enfant
	private Node ActionsPanel; //Panneau droit avec les actions possibles à réaliser sur les cases

	//READY _______________________________________________________________________________________

	public override void _Ready()
	{
		//Fonction d'initialisation de CellControlScript
		Initialisation();
		ControlledCell = this.GetChild(0);
		ActionsPanel = GetNode<Control>("../../ControlGeneral/Control");

	}

	public void Initialisation()
	{
		//Sous-fonction d'initialisation de ControlCaseScipt
		CurrentPlant = null;

		Humidity = 0;
		Nitrogen = 5;
		Compost = false;

		IsInfested = false;
		Chemicals = 0.0;
		Disease = "aucune";
		DaysSinceLastTreatment = 0;
		IsTreated = false;
		ChemicalIntoxication = false;
	}

	//PROCESS _______________________________________________________________________________________

	public override void _Process(double delta)
	{
		//Fonction appelée en boucle pour détecter les actions et mettre à jour l'affichage
		RefreshValues();
		if (ControlledCell is CellScript)
		{
			if (((CellScript)ControlledCell).IsPressed)
			{ //Si le bouton enfant est appuyé. Doit être casté pour trouver la variable.

				/* En C#, la Node n'est pas égale à son script,c'est pourquoi ici
				le "if" réalise un cast implicite de la Node vers la classe de son script :*/
				if (ActionsPanel is ControlScript)
				{
					Watering();
					SpreadingCompost();
					Selecting();
					SprayingChemicals();
					Treating();
					Gathering();
				}
			}
			DisplayPlant();
		}
	}

	//GETTER ___________________________________________________________________________________________

	public double GetHumidity()
	{
		return Humidity;
	}

	public double GetPesticide()
	{
		return Chemicals;
	}

	//ACTIONS APPELEES PAR LE PROCESS __________________________________________________________________

	private void Gathering()
	{
		//On récolte si le curseur est normal et si la plante est récoltable
		if (ControlScript.ActionCursor == "normal")
		{
			if (CurrentPlant is not null)
			{
				string Etat = CurrentPlant.IsGatherable();
				if (Etat != "En Croissance")
				{
					if (Etat == "Morte")
					{
						CurrentPlant = null;
					}
					else
					{
						bool Ajout = Global.Inventory.ModifyProductQuantity(Etat, 1);
						if (!Ajout)
						{
							Ajout = Global.Inventory.AddProduct(Etat, 1);
							if (!Ajout)
							{
								Panel Popup = GetNode<Panel>("../../ControlPopup/Popup");
								Popup.GetChild<Label>(0).Text = "Votre inventaire est plein";
								Popup.Show();
							}
						}
						if (Ajout)
						{
							CurrentPlant = null;
						}
					}
				}
			}

		}
	}

	private void Watering()
	{
		//On modifie l'humidité si bouton appuyé, curseur en arrosoir et humidité augmentable
		if (ControlScript.ActionCursor == "arrosoir" && Humidity < 100)
		{
			Humidity += 0.05;
		}
	}

	private void SpreadingCompost()
	{
		//On modifie l'attribut compost si bouton appuyé et curseur en compost
		if (ControlScript.ActionCursor == "compost" && !Compost)
		{
			Compost = true;
			GetChild(0).GetNode<TextureRect>("TextureRect").Texture = (Texture2D)ResourceLoader.Load("../../Assets/Images/ImagesPots/TerreCompost.png");
		}
	}


	private void Selecting()
	{
		//Indique dans GridControlScript quelle case est sélectionnée
		Node ControlCarre = GetNode<Control>("../../ControlCarre");
		((GridControlScript)ControlCarre).SetSelectedCell(this);
	}

	private void SprayingChemicals()
	{
		//On modifie le pesticide si bouton appuyé, curseur en pesticide et pesticide augmentable
		if (ControlScript.ActionCursor == "pesticide" && Chemicals < 100)
		{
			Chemicals += 0.1;
		}
	}

	private void Treating()
	{
		//On modifie l'attribut EstTraité si bouton appuyé et curseur en traitement
		if (ControlScript.ActionCursor == "bouillie")
		{
			IsTreated = true;
			DaysSinceLastTreatment = 0;
		}
	}

	private void DisplayPlant()
	{
		//Fonction d'affichage de l'image de la plante sur la case
		if (CurrentPlant is null)
		{
			((TextureRect)GetChild(0).GetChild(1)).Texture = null;
		}
		else
		{
			((TextureRect)GetChild(0).GetChild(1)).Texture = CurrentPlant.GetImage();
		}
	}

	//METHODES _______________________________________________________________________________________

	public void SetPlant(string Seed)
	{
		CurrentPlant = new Plant(Seed);
	}

	public Plant GetPlant()
	{
		return CurrentPlant;
	}

	public bool HasHygrometer()
	{
		//Renvoie true si la case a un hygromètre et false sinon
		LargeCellsSlots GreatCollider = GetNode<LargeCellsSlots>("../../DragAndDrop/GrossesCase" + (this.Name.ToString()).Substring(11, 1));
		List<string> SensorsArray = GreatCollider.GetSensorsArray();
		if (SensorsArray.Contains("Hygromètre") || SensorsArray.Contains("Capteur d'humidité visuel"))
		{
			return true;
		}
		return false;
	}
	public bool HasLuxmeter()
	{
		//Renvoie true si la case a un luxmètre et false sinon
		LargeCellsSlots GreatCollider = GetNode<LargeCellsSlots>("../../DragAndDrop/GrossesCase" + (this.Name.ToString()).Substring(11, 1));
		List<string> SensorsArray = GreatCollider.GetSensorsArray();
		if (SensorsArray.Contains("Luxmètre"))
		{
			return true;
		}
		return false;
	}

	public bool HasThermometer()
	{
		//Renvoie true si la case a un thermomètre et false sinon
		LargeCellsSlots GreatCollider = GetNode<LargeCellsSlots>("../../DragAndDrop/GrossesCase" + (this.Name.ToString()).Substring(11, 1));
		List<string> SensorsArray = GreatCollider.GetSensorsArray();
		if (SensorsArray.Contains("Thermomètre") || SensorsArray.Contains("Capteur de température visuel"))
		{
			return true;
		}
		return false;
	}


	public void RefreshValues()
	{ //Chemicals et IsTreated doivent changer au fil du temps
	  //int Date =  GetNode<Control>("../../ControlDate");
	  //ICI : diminuer le pesticide et le traitement dans la fonction de passage de jour
		if (Chemicals > 30)
		{
			ChemicalIntoxication = true;
		}
		else
		{
			ChemicalIntoxication = false;
		}
		if (DaysSinceLastTreatment > 12)
		{
			IsTreated = false;
			DaysSinceLastTreatment = 0;
		}
	}

	public string HasCompost()
	{
		if (Compost)
		{
			return "oui";
		}
		else
		{
			return "non";
		}
	}

	public string HasTreatment()
	{
		if (IsTreated)
		{
			return "oui";
		}
		else
		{
			return "non";
		}
	}

	public string IsChemicallyIntoxicated()
	{
		if (Chemicals > 30)
		{
			return "oui";
		}
		else
		{
			return "non";
		}
	}

	public string GetDisease()
	{
		return Disease;
	}


	//METHODES DE SAUVEGARDE _____________________________________________________________________________________________________________


	public void LoadCell(Godot.Collections.Array<string> CurrentData)
	{
		//Fonction de chargement des données de la case de plantation

		//1) Données de la case de terre
		Humidity = Convert.ToDouble(CurrentData[0]);
		Nitrogen = Convert.ToDouble(CurrentData[1]);
		Compost = Convert.ToBoolean(CurrentData[2]);
		if (Compost)
		{
			GetChild(0).GetNode<TextureRect>("TextureRect").Texture = (Texture2D)ResourceLoader.Load("../../Assets/Images/ImagesPots/TerreCompost.png");
		}
		IsInfested = Convert.ToBoolean(CurrentData[3]);
		Chemicals = Convert.ToDouble(CurrentData[4]);
		Disease = CurrentData[5];
		IsTreated = Convert.ToBoolean(CurrentData[6]);
		DaysSinceLastTreatment = Convert.ToInt32(CurrentData[7]);
		ChemicalIntoxication = Convert.ToBoolean(CurrentData[8]);

		//2) Données de la plante
		if (CurrentData[9] != "0")
		{
			CurrentPlant = new Plant(CurrentData[9]);
			CurrentPlant.LoadPlant(Convert.ToInt32(CurrentData[10]), Convert.ToInt32(CurrentData[11]), Convert.ToInt32(CurrentData[12]), Convert.ToInt32(CurrentData[13]), Convert.ToBoolean(CurrentData[14]));
			if (Disease != "aucun")
			{
				CurrentPlant.BecomesSick(true);
			}
			else
			{
				if (!ChemicalIntoxication)
				{
					CurrentPlant.BecomesSick(false);
				}
			}
		}

		//3) Données des capteurs
		CallDeferred("Loading", CurrentData);
	}



	public void Loading(Godot.Collections.Array<string> CurrentData)
	{
		//Sous fonction de chargement des données de la case de plantation
		LargeCellsSlots GreatCollider = GetNode<LargeCellsSlots>("../../DragAndDrop/GrossesCase" + (this.Name.ToString()).Substring(11, 1));
		for (int i = 15; i < CurrentData.Count; i++)
		{
			GreatCollider.AddItem(CurrentData[i]);
		}
		GreatCollider.GetSensorsNodesArray();
		PanelScript ParentPanel = GetParent().GetParent().GetNode<PanelScript>("ControlGeneral/Control/Panneau");
		ParentPanel.DisplaySeedsNodes(false);
		//GreatCollider.DisplaySensors();
	}

	public Godot.Collections.Array<string> GetData()
	{
		//Fonction d'envoi des données de la case de plantation

		//1) Données de la plante
		string PlantName = "0";
		string PlantStage = "0";
		string TotalDaysNumber = "0";
		string TotalChemicalsDaysNumber = "0";
		string TotalDaysSinceLastStage = "0";
        string isDead = "false";

		if (CurrentPlant is not null)
		{
			PlantName = CurrentPlant.GetName();
			PlantStage = CurrentPlant.GetPlantStage().ToString();
			TotalDaysNumber = CurrentPlant.GetTotalDaysNumber().ToString();
			TotalChemicalsDaysNumber = CurrentPlant.GetTotalChemicalDaysNumber().ToString();
			TotalDaysSinceLastStage = CurrentPlant.GetTotalDaysSinceLastStage().ToString();
            isDead = CurrentPlant.GetDead().ToString();
		}

		//2) Données de la case de terre
		Godot.Collections.Array<string> CurrentData = new Godot.Collections.Array<string>{
			Mathf.Snapped(Humidity,0.01).ToString(), Mathf.Snapped(Nitrogen,0.01).ToString(), Compost.ToString(),
			IsInfested.ToString(), Mathf.Snapped(Chemicals,0.01).ToString(), Disease, IsTreated.ToString(),
			DaysSinceLastTreatment.ToString(), ChemicalIntoxication.ToString(), PlantName, PlantStage,
			TotalDaysNumber, TotalChemicalsDaysNumber, TotalDaysSinceLastStage, isDead
		};


		//3) données des capteurs
		List<string> SensorsArray = GetNode<LargeCellsSlots>("../../DragAndDrop/GrossesCase" + (this.Name.ToString()).Substring(11, 1)).GetSensorsArray();
		foreach (string Capteur in SensorsArray)
		{
			CurrentData.Add(Capteur);
		}
		return CurrentData;
	}

}
