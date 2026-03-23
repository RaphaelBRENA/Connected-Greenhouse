using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradePanel : Panel
{
	Dictionary<string, Button> DictionaryButton;
	Dictionary<string, PopupMenu> DictionaryPopup;
	Dictionary<string, string> DictionarySave;
	Dictionary<string, Label> DictionaryLabel;
	ComputerInterface ComputerInterface;
	Node2D ThermoInterface;
	double GeneratorConstant;
	double AirConditionerConstant;
	double LampsCost;
	int PotsNumber;
	double PotsCost;
	double AirConditionerCost = 0;
	int AirConditionerReliability;
	int AirConditionerConsumption;
	double TotalDailyCost;
	double CostSinceBeginingYear;
	List<string> LastDate;
	List<string> CurrentDate;


	/// <summary>
	/// Called when the node enters the AmeliorationInterface scene.
	/// Sets up the panel by initializing all the buttons, labels, and popups,
	/// loading the save, and updating the cost and expenses.
	/// </summary>
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		InitButtons();
		InitLabels();
		LoadSave();
		InitPots();
		InitAirConditioner();
		InitPopups();
		InitComputer();
		UpdateConstants();
		DisplayCost();
		DisplayExpenses();
		ChangeDate();
	}

	/// <summary>
	/// Called when the user clicks on the "Ameliorations" or "Depenses" button.
	/// Shows the appropriate panel and hides the other one.
	/// </summary>
	/// <param name="Name">The name of the button that was clicked.</param>
	public void OnChangePanelPressed(string Name)
	{
		if (Name == "Ameliorations")
		{
			DisplayExpenses();
			this.Hide();
			GetParent().GetNode<Panel>("PanneauDepense").Show();
		}
		else if (Name == "Depenses")
		{
			GetParent().GetNode<Panel>("PanneauDepense").Hide();
			this.Show();
		}
	}

	/// <summary>
	/// Displays the popup menu associated with the given name and resets its size to zero.
	/// </summary>
	/// <param name="Name">The name of the popup menu to display.</param>
	public void OnButtonsPopupPressed(string Name)
	{
		if (DictionaryPopup.ContainsKey(Name))
		{
			DictionaryPopup[Name].Show();
			DictionaryPopup[Name].Size = new Vector2I(0, 0);
		}
	}

	/// <summary>
	/// Called when the user selects an item in a popup menu.
	/// Updates the button associated with the popup menu to show the selected item,
	/// updates the save, and refreshes the cost and expenses displays.
	/// </summary>
	/// <param name="Id">The ID of the selected item.</param>
	/// <param name="Name">The name of the popup menu.</param>
	public void OnPopupItemPressed(int Id, string Name)
	{
		if (Id >= 0 && DictionaryButton.ContainsKey(Name))
		{
			if (Name.Contains("Pot"))
			{
				PopupPots(Id, Name);
			}
			else if (Name.Contains("Clime"))
			{
				PopupAirConditioner(Id, Name);
			}
			DictionaryButton[Name].Icon = DictionaryPopup[Name].GetItemIcon(Id);
			DictionarySave[Name] = DictionaryPopup[Name].GetItemText(Id);
			UpdateConstants();
			DisplayCost();
			DisplayExpenses();
		}
	}

	/// <summary>
	/// Initializes the buttons dictionary with the buttons in the improvement panel.
	/// </summary>
	public void InitButtons()
	{
		DictionaryButton = new Dictionary<string, Button>();
		DictionaryButton["Generateur"] = GetNode<Button>("HBoxContainer/Colonne0/Partie0/Case0");
		DictionaryButton["Panneaux"] = GetNode<Button>("HBoxContainer/Colonne0/Partie1/Case1");
		DictionaryButton["Pot1"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot1");
		DictionaryButton["Pot2"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot2");
		DictionaryButton["Pot3"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot3");
		DictionaryButton["Pot4"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot4");
		DictionaryButton["Pot5"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot5");
		DictionaryButton["Pot6"] = GetNode<Button>("HBoxContainer/Colonne1/GrillePots/Pot6");
		DictionaryButton["Lampes"] = GetNode<Button>("HBoxContainer/Colonne2/Partie3/Case3");
		DictionaryButton["Clime1"] = GetNode<Button>("HBoxContainer/Colonne2/Partie4/HBoxContainer/Case4");
		DictionaryButton["Clime2"] = GetNode<Button>("HBoxContainer/Colonne2/Partie4/HBoxContainer/Case5");
		DictionaryButton["Depenses"] = GetNode<Button>("Depenses");
	}

	/// <summary>
	/// Initializes the labels dictionary with the labels in the improvement panel.
	/// </summary>
	public void InitLabels()
	{
		DictionaryLabel = new Dictionary<string, Label>();
		DictionaryLabel["Generateur"] = GetNode<Label>("HBoxContainer/Colonne0/Partie0/Label");
		DictionaryLabel["Panneaux"] = GetNode<Label>("HBoxContainer/Colonne0/Partie1/Label");
		DictionaryLabel["Pots"] = GetNode<Label>("HBoxContainer/Colonne1/Label");
		DictionaryLabel["Lampes"] = GetNode<Label>("HBoxContainer/Colonne2/Partie3/Label");
		DictionaryLabel["Clime"] = GetNode<Label>("HBoxContainer/Colonne2/Partie4/Label");
	}

	/// <summary>
	/// Initializes the computer interface by instantiating a new instance of it from its PackedScene,
	/// setting its visibility to false, and adding it as a child of the improvement panel.
	/// Also initializes the thermostat interface in the same way.
	/// </summary>
	public void InitComputer()
	{
		ComputerInterface = (ComputerInterface)((PackedScene)GD.Load("res://Scenes/Interface/OrdinateurInterface.tscn")).Instantiate();
		ComputerInterface.Visible = false;
		AddChild(ComputerInterface);
		ThermoInterface = (Node2D)((PackedScene)GD.Load("res://Scenes/Interface/ThermostatInterface.tscn")).Instantiate();
		ThermoInterface.Visible = false;
		AddChild(ThermoInterface);
	}

	/// <summary>
	/// Returns the list of items associated with the air conditioner.
	/// The list contains the two items associated with the two air conditioner slots.
	/// </summary>
	/// <returns>A list of strings, each representing the name of an item.</returns>
	public List<string> GetAirConditionerItems()
	{
		return new List<string> { DictionarySave["Clime1"], DictionarySave["Clime2"] };
	}

	/// <summary>
	/// Initializes the pots number by counting the number of "Pot haute qualité" in the inventory.
	/// Subtracts the number of "Pot haute qualité" already placed in the improvement panel.
	/// </summary>
	public void InitPots()
	{
		PotsNumber = 0;
		PotsNumber = Global.Inventory.GetProductQuantity("Pot haute qualité");
		foreach (KeyValuePair<string, string> Entry in DictionarySave)
		{
			if (Entry.Value == "Pot haute qualité")
			{
				PotsNumber--;
			}
		}
	}

	/// <summary>
	/// Initializes the air conditioner's reliability and consumption values based on the inventory.
	/// Sets the initial values from the inventory quantities of "Amélioration de la fiabilité de la clime"
	/// and "Réduction de la consommation de la clime". Adjusts these values by decrementing for each
	/// corresponding entry found in the saved dictionary.
	/// </summary>
	public void InitAirConditioner()
	{
		AirConditionerReliability = 0;
		AirConditionerConsumption = 0;
		AirConditionerReliability = Global.Inventory.GetProductQuantity("Amélioration de la fiabilité de la clime");
		AirConditionerConsumption = Global.Inventory.GetProductQuantity("Réduction de la consommation de la clime");
		foreach (KeyValuePair<string, string> Entry in DictionarySave)
		{
			if (Entry.Value == "Amélioration de la fiabilité de la clime")
			{
				AirConditionerReliability--;
			}
			else if (Entry.Value == "Réduction de la consommation de la clime")
			{
				AirConditionerConsumption--;
			}
		}
	}

	/// <summary>
	/// Initializes the popups dictionary with the popups in the improvement panel.
	/// For each popup, it sets up the items and their icons, and grays out the items that are not available in the inventory.
	/// </summary>
	public void InitPopups()
	{
		DictionaryPopup = new Dictionary<string, PopupMenu>();
		InitPopup("Generateur", "HBoxContainer/Colonne0/Partie0/PopupMenu", new List<string> { "Réduction de la consommation du générateur" });
		InitPopup("Panneaux", "HBoxContainer/Colonne0/Partie1/PopupMenu", new List<string> { "Panneaux solaires et batterie" });
		InitPopup("Lampes", "HBoxContainer/Colonne2/Partie3/PopupMenu", new List<string> { "LED basse consommation" });
		InitPopup("Clime1", "HBoxContainer/Colonne2/Partie4/PopupMenu", new List<string> { "Amélioration de la fiabilité de la clime", "Réduction de la consommation de la clime" });
		InitPopup("Clime2", "HBoxContainer/Colonne2/Partie4/PopupMenu2", new List<string> { "Amélioration de la fiabilité de la clime", "Réduction de la consommation de la clime" });
		InitPopup("Pot1", "HBoxContainer/Colonne1/GrillePots/PopupMenu", new List<string> { "Pot haute qualité" });
		InitPopup("Pot2", "HBoxContainer/Colonne1/GrillePots/PopupMenu2", new List<string> { "Pot haute qualité" });
		InitPopup("Pot3", "HBoxContainer/Colonne1/GrillePots/PopupMenu3", new List<string> { "Pot haute qualité" });
		InitPopup("Pot4", "HBoxContainer/Colonne1/GrillePots/PopupMenu4", new List<string> { "Pot haute qualité" });
		InitPopup("Pot5", "HBoxContainer/Colonne1/GrillePots/PopupMenu5", new List<string> { "Pot haute qualité" });
		InitPopup("Pot6", "HBoxContainer/Colonne1/GrillePots/PopupMenu6", new List<string> { "Pot haute qualité" });
	}

	/// <summary>
	/// Initializes a popup menu for a given material in the improvement panel.
	/// Sets the maximum width for item icons and grays out items that are not available 
	/// in the inventory or have certain conditions, such as being already selected 
	/// in another related popup or having insufficient quantity in the inventory.
	/// </summary>
	/// <param name="Material">The identifier for the popup menu to initialize.</param>
	/// <param name="Path">The node path to the popup menu in the scene.</param>
	/// <param name="Names">A list of item names to be included in the popup menu.</param>
	public void InitPopup(string Material, string Path, List<string> Names)
	{

		DictionaryPopup[Material] = GetNode<PopupMenu>(Path);
		for (int i = 0; i < DictionaryPopup[Material].GetItemCount(); i++)
		{
			DictionaryPopup[Material].SetItemIconMaxWidth(i, 80);
		}
		for (int j = 0; j < Names.Count; j++)
		{
			if (!Global.Inventory.ContainsProduct(Names[j]))
			{
				GrayIcon(Material, j);
			}
			if (Names[j] == "Amélioration de la fiabilité de la clime" || Names[j] == "Réduction de la consommation de la clime")
			{
				if (Material == "Clime1" && DictionarySave["Clime2"] == Names[j] && Global.Inventory.GetProductQuantity(Names[j]) < 2)
				{
					GrayIcon(Material, j);
				}
				else if (Material == "Clime2" && DictionarySave["Clime1"] == Names[j] && Global.Inventory.GetProductQuantity(Names[j]) < 2)
				{
					GrayIcon(Material, j);
				}
			}
			else if (Material.Contains("Pot") && DictionarySave[Material] != "Pot haute qualité" && PotsNumber == 0)
			{
				GrayIcon(Material, j);
			}
		}
	}

	/// <summary>
	/// Handles the logic for updating the state of pot items in the popup menu
	/// based on the selected item ID. If a "Pot haute qualité" is selected,
	/// it increments the available pots number and updates related popup items.
	/// If "Ne rien sélectionner" is chosen, it decrements the pots number and
	/// potentially disables related items if no pots are available.
	/// </summary>
	/// <param name="id">The ID of the selected item.</param>
	/// <param name="Name">The name of the popup menu for the pot.</param>
	public void PopupPots(int id, string Name)
	{
		if (id < 0) return;
		if (id == 0 && DictionarySave[Name] == "Pot haute qualité")
		{
			PotsNumber++;
			foreach (KeyValuePair<string, string> Entry in DictionarySave)
			{
				if (Entry.Key.Contains("Pot"))
				{
					DictionaryPopup[Entry.Key].SetItemIcon(1, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Pot haute qualité.png"));
					DictionaryPopup[Entry.Key].SetItemDisabled(1, false);
				}
			}
		}
		else
		{
			if (id == 1 && DictionarySave[Name] == "Ne rien sélectionner")
			{
				PotsNumber--;
				if (PotsNumber == 0)
				{
					foreach (KeyValuePair<string, string> Entry in DictionarySave)
					{
						if (Entry.Key.Contains("Pot") && Entry.Value != "Pot haute qualité" && Entry.Key != Name)
						{
							GrayIcon(Entry.Key, 0);
							DictionaryPopup[Entry.Key].SetItemDisabled(1, true);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Handles the logic for updating the state of air conditioner items in the popup menu
	/// based on the selected item ID. If a "Amélioration de la fiabilité de la clime" or
	/// "Réduction de la consommation de la clime" is selected, it increments the available
	/// air conditioners number and updates related popup items. If "Ne rien sélectionner"
	/// is chosen, it decrements the air conditioners number and potentially disables
	/// related items if no air conditioners are available.
	/// </summary>
	/// <param name="id">The ID of the selected item.</param>
	/// <param name="Name">The name of the popup menu for the air conditioner.</param>
	public void PopupAirConditioner(int id, string Name)
	{
		if (id < 0) return;
		int PreviousId = 0; if (DictionarySave[Name] == "Ne rien sélectionner") PreviousId = 0; else if (DictionarySave[Name] == "Amélioration de la fiabilité de la clime") PreviousId = 1; else PreviousId = 2;
		if (id == 0 && DictionarySave[Name] == "Amélioration de la fiabilité de la clime")
		{
			AirConditionerReliability++;
			foreach (KeyValuePair<string, string> Entry in DictionarySave)
			{
				if (Entry.Key.Contains("Clime") && PreviousId != 0)
				{
					DictionaryPopup[Entry.Key].SetItemIcon(PreviousId, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Amélioration de la fiabilité de la clime.png"));
					DictionaryPopup[Entry.Key].SetItemDisabled(PreviousId, false);
				}
			}
		}
		else if (id == 1 && DictionarySave[Name] == "Ne rien sélectionner")
		{
			AirConditionerReliability--;
			if (AirConditionerReliability == 0)
			{
				foreach (KeyValuePair<string, string> Entry in DictionarySave)
				{
					if (Entry.Key.Contains("Clime") && Entry.Value != "Amélioration de la fiabilité de la clime" && Entry.Key != Name)
					{
						GrayIcon(Entry.Key, 0);
						DictionaryPopup[Entry.Key].SetItemDisabled(1, true);
					}
				}
			}
		}
		else if (id == 1 && DictionarySave[Name] == "Réduction de la consommation de la clime")
		{
			AirConditionerReliability--;
			AirConditionerConsumption++;
			if (AirConditionerReliability == 0)
			{
				foreach (KeyValuePair<string, string> Entry in DictionarySave)
				{
					if (Entry.Key.Contains("Clime") && Entry.Value != "Amélioration de la fiabilité de la clime" && Entry.Key != Name)
					{
						GrayIcon(Entry.Key, 0);
						DictionaryPopup[Entry.Key].SetItemDisabled(1, true);
					}
				}
			}
			if (Name == "Clime1")
			{
				DictionaryPopup["Clime2"].SetItemIcon(2, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Réduction de la consommation de la clime.png"));
				DictionaryPopup["Clime2"].SetItemDisabled(2, false);
			}
			else if (Name == "Clime2")
			{
				DictionaryPopup["Clime1"].SetItemIcon(2, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Réduction de la consommation de la clime.png"));
				DictionaryPopup["Clime1"].SetItemDisabled(2, false);
			}
		}
		else if (id == 0 && DictionarySave[Name] == "Réduction de la consommation de la clime")
		{
			AirConditionerConsumption++;
			foreach (KeyValuePair<string, string> Entry in DictionarySave)
			{
				if (Entry.Key.Contains("Clime") && PreviousId != 0)
				{
					DictionaryPopup[Entry.Key].SetItemIcon(PreviousId, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Réduction de la consommation de la clime.png"));
					DictionaryPopup[Entry.Key].SetItemDisabled(PreviousId, false);
				}
			}
		}
		else if (id == 2 && DictionarySave[Name] == "Ne rien sélectionner")
		{
			AirConditionerConsumption--;
			if (AirConditionerConsumption == 0)
			{
				foreach (KeyValuePair<string, string> Entry in DictionarySave)
				{
					if (Entry.Key.Contains("Clime") && Entry.Value != "Réduction de la consommation de la clime" && Entry.Key != Name)
					{
						GrayIcon(Entry.Key, 1);
						DictionaryPopup[Entry.Key].SetItemDisabled(2, true);
					}
				}
			}
		}
		else if (id == 2 && DictionarySave[Name] == "Amélioration de la fiabilité de la clime")
		{
			AirConditionerReliability++;
			AirConditionerConsumption--;
			if (AirConditionerConsumption == 0)
			{
				foreach (KeyValuePair<string, string> Entry in DictionarySave)
				{
					if (Entry.Key.Contains("Clime") && Entry.Value != "Réduction de la consommation de la clime" && Entry.Key != Name)
					{
						GrayIcon(Entry.Key, 1);
						DictionaryPopup[Entry.Key].SetItemDisabled(2, true);
					}
				}
			}
			if (Name == "Clime1")
			{
				DictionaryPopup["Clime2"].SetItemIcon(1, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Amélioration de la fiabilité de la clime.png"));
				DictionaryPopup["Clime2"].SetItemDisabled(1, false);
			}
			else if (Name == "Clime2")
			{
				DictionaryPopup["Clime1"].SetItemIcon(1, (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/Amélioration de la fiabilité de la clime.png"));
				DictionaryPopup["Clime1"].SetItemDisabled(1, false);
			}
		}
	}

	/// <summary>
	/// Grays out the icon for a specified item in the popup menu.
	/// Disables the item and converts its icon to grayscale by calculating
	/// the luminance of each pixel and updating the image accordingly.
	/// </summary>
	/// <param name="Material">The identifier for the popup menu containing the item.</param>
	/// <param name="j">The index of the item in the popup menu to gray out.</param>
	public void GrayIcon(string Material, int j)
	{
		DictionaryPopup[Material].SetItemDisabled(j + 1, true);
		Texture2D OriginalTexture = DictionaryPopup[Material].GetItemIcon(j + 1);
		Image Image = OriginalTexture.GetImage();
		for (int y = 0; y < Image.GetHeight(); y++)
		{
			for (int x = 0; x < Image.GetWidth(); x++)
			{
				Color CurrentColor = Image.GetPixel(x, y);
				float Gray = CurrentColor.R * 0.299f + CurrentColor.G * 0.587f + CurrentColor.B * 0.114f;
				Image.SetPixel(x, y, new Color(Gray, Gray, Gray, CurrentColor.A));
			}
		}
		ImageTexture NewTexture = ImageTexture.CreateFromImage(Image);
		DictionaryPopup[Material].SetItemIcon(j + 1, NewTexture);
	}

	/// <summary>
	/// Loads data from the save system to update the current date and selected items.
	/// Updates the cost since the beginning of the year and the last date.
	/// Checks if the selected items are available in the inventory and updates the buttons' icons accordingly.
	/// </summary>
	public void LoadSave()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> CalendarData = Global.SaveInteract["Calendrier"].SendData();
		CurrentDate = new List<string>();
		for (int i = 0; i < 3; i++)
		{
			CurrentDate.Add(CalendarData["Date"][i]);
		}
		DictionarySave = new Dictionary<string, string>();
		DictionarySave["Generateur"] = "Ne rien sélectionner";
		DictionarySave["Panneaux"] = "Ne rien sélectionner";
		DictionarySave["Pot1"] = "Ne rien sélectionner";
		DictionarySave["Pot2"] = "Ne rien sélectionner";
		DictionarySave["Pot3"] = "Ne rien sélectionner";
		DictionarySave["Pot4"] = "Ne rien sélectionner";
		DictionarySave["Pot5"] = "Ne rien sélectionner";
		DictionarySave["Pot6"] = "Ne rien sélectionner";
		DictionarySave["Lampes"] = "Ne rien sélectionner";
		DictionarySave["Clime1"] = "Ne rien sélectionner";
		DictionarySave["Clime2"] = "Ne rien sélectionner";
		CostSinceBeginingYear = 0;
		TotalDailyCost = 0;
		LastDate = new List<string>() { "1", "Printemps", "1" };
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Dictionary = Global.SaveInteract["Cout Energetique"].SendData();
		if (Dictionary == null) { return; }
		foreach (KeyValuePair<string, Godot.Collections.Array<string>> Entry in Dictionary)
		{
			if (Global.Inventory.ContainsProduct(Entry.Value[0]))
			{
				DictionarySave[Entry.Key] = Entry.Value[0];
			}
		}
		foreach (KeyValuePair<string, string> Entry in DictionarySave)
		{
			if (Entry.Value != "Ne rien sélectionner" && Global.Inventory.ContainsProduct(Entry.Value))
			{
				DictionaryButton[Entry.Key].Icon = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + Entry.Value + ".png");
			}
		}
		CostSinceBeginingYear = Double.Parse(Dictionary["CoutDepuisDebutAnnee"][0]);
		LastDate = new List<string>();
		for (int i = 0; i < 3; i++)
		{
			LastDate.Add(Dictionary["Date"][i]);
		}
		TotalDailyCost = Double.Parse(Dictionary["CoutJournalier"][0]);
	}

	/// <summary>
	/// Updates the constants used to calculate the cost of the energy expenses.
	/// This function is called when the state of the energy saving items is changed.
	/// The constants are used in the <see cref="DailyExpenses"/> function to calculate the cost of the energy expenses.
	/// </summary>
	public void UpdateConstants()
	{
		GeneratorConstant = 0.0261;
		if (DictionarySave["Generateur"] == "Réduction de la consommation du générateur")
		{
			GeneratorConstant -= 0.0053;
		}
		if (DictionarySave["Panneaux"] == "Panneaux solaires et batterie")
		{
			GeneratorConstant -= 0.0042;
		}
		if (ComputerInterface.CalculateLampsLuminosity() != 0)
		{
			LampsCost = Math.Log(ComputerInterface.CalculateLampsLuminosity()) * GeneratorConstant * 30;
		}
		else
		{
			LampsCost = 0;
		}
		if (DictionarySave["Lampes"] == "LED basse consommation")
		{
			LampsCost *= 0.7;
		}
		PotsCost = 0;
		int Counter = 1;
		foreach (KeyValuePair<string, string> Entry in DictionarySave)
		{
			if (Entry.Key.Contains("Pot") && ComputerInterface.CalculatePotHumidity(Counter) != -1 && ComputerInterface.CalculatePotHumidity(Counter) != 0)
			{
				Counter++;
				if (DictionarySave[Entry.Key] == "Pot haute qualité")
				{
					PotsCost += 0.96;
				}
				else
				{
					PotsCost += 1.45;
				}
			}
		}
		AirConditionerConstant = 150;
		if (DictionarySave["Clime1"] == "Réduction de la consommation de la clime")
		{
			AirConditionerConstant -= 22;
		}
		if (DictionarySave["Clime2"] == "Réduction de la consommation de la clime")
		{
			AirConditionerConstant -= 22;
		}
		AirConditionerCost = 0;
		if (((ThermostatProgrammingPanel)ThermoInterface.GetNode<Panel>("ControlPanneau2/PanneauProgrammation")).EffectiveProgrammedValue() != -1)
		{
			AirConditionerCost = Math.Log(Math.Abs(Global.GlassHouseTemperature - double.Parse(Global.SaveInteract["Calendrier"].SendData()["Weather"][1]))) * 2 * GeneratorConstant * AirConditionerConstant;
		}
	}

	/// <summary>
	/// Updates the labels with the current cost of the energy expenses.
	/// This function is called when the state of the energy saving items is changed.
	/// The labels are updated with the current cost of the energy expenses.
	/// </summary>
	public void DisplayCost()
	{
		DictionaryLabel["Generateur"].Text = "Constante consommation : " + (1000 * GeneratorConstant).ToString("0");
		if (DictionarySave["Panneaux"] == "Panneaux solaires et batterie")
		{
			DictionaryLabel["Panneaux"].Text = "Réduction consommation";
		}
		else
		{
			DictionaryLabel["Panneaux"].Text = "";
		}
		DictionaryLabel["Lampes"].Text = LampsCost.ToString("0.00") + "€/j";
		DictionaryLabel["Pots"].Text = PotsCost.ToString("0.00") + "€/j";
		DictionaryLabel["Clime"].Text = AirConditionerCost.ToString("0.00") + "€/j";
	}

	/// <summary>
	/// Updates the labels in the "Depenses" panel with the current daily cost of the energy expenses,
	/// the total cost of the energy expenses since the beginning of the year, and the predicted cost
	/// of the energy expenses for the whole year based on the current daily cost.
	/// </summary>
	public void DisplayExpenses()
	{
		DailyExpenses();
		ExpensesSinceBeginingYear();
		ExpensesForWholeYear();
	}

	/// <summary>
	/// Updates the daily expenses label with the current total daily cost of the energy expenses.
	/// </summary>
	public void DailyExpenses()
	{
		TotalDailyCost = PotsCost + LampsCost + AirConditionerCost;
		GetParent().GetNode<Label>("PanneauDepense/VBoxContainer/DepensesJournalieres").Text = ("Dépenses à la fin de cette journée : " + TotalDailyCost.ToString("0.00") + "€");
	}

	/// <summary>
	/// Updates the expenses since the beginning of the year label with the current total expenses since the beginning of the year.
	/// </summary>
	public void ExpensesSinceBeginingYear()
	{
		GetParent().GetNode<Label>("PanneauDepense/VBoxContainer/DepensesDepuisDebutAnnee").Text = ("Dépenses depuis le début de l'année : " + (CostSinceBeginingYear).ToString("0.00") + "€");
	}

	/// <summary>
	/// Updates the expenses for the whole year label with the current total expenses for the whole year,
	/// based on the current daily cost of the energy expenses.
	/// The total expenses for the whole year is calculated by adding the cost of the energy expenses since
	/// the beginning of the year to the cost of the energy expenses for the remaining days in the year.
	/// The number of remaining days is calculated as the number of days until the end of the year minus the
	/// number of days that have passed so far this year.
	/// </summary>
	public void ExpensesForWholeYear()
	{
		GetParent().GetNode<Label>("PanneauDepense/VBoxContainer/PrevisionDepenses").Text = ("Prévision des dépenses pour cette année : " + (CostSinceBeginingYear + TotalDailyCost * ((56 - (int.Parse(CurrentDate[0]) % 56)) % 56)).ToString("0.00") + "€");
	}

	/// <summary>
	/// Calculates the difference in days between the current date and the last connexion date of the user in the PanneauAmeliorations scene.
	/// The difference is calculated as the day of the current date minus the day of the last date.
	/// </summary>
	/// <param name="LastDate">The last connexion date of the user in the PanneauAmeliorations scene.</param>
	/// <param name="CurrentDate">The current date.</param>
	/// <returns>The difference in days between the current date and the last date.</returns>
	public int DaysDifference(List<string> LastDate, List<string> CurrentDate)
	{
		return int.Parse(CurrentDate[0]) - int.Parse(LastDate[0]);
	}

	/// <summary>
	/// Converts the given season to an integer.
	/// </summary>
	/// <param name="Season">The season to convert.</param>
	/// <returns>An integer representing the season. 1 for Printemps, 2 for Eté, 3 for Automne, and 4 for Hiver.</returns>
	public int SeasonToInt(string Season)
	{
		if (Season == "Printemps")
		{
			return 1;
		}
		else if (Season == "Eté")
		{
			return 2;
		}
		else if (Season == "Automne")
		{
			return 3;
		}
		else
		{
			return 4;
		}
	}

	/// <summary>
	/// Handles the input of the user in the PanneauAmelioration scene.
	/// If the user presses the "Interact" action or the escape key, the data is exported and the scene is changed to the Gameplay scene.
	/// The mouse mode is set to captured.
	/// </summary>
	/// <param name="event">The input event.</param>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && (eventKey.IsActionPressed("Interact") || eventKey.Keycode == Key.Escape))
			{
				ExportData();
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
	}

	/// <summary>
	/// Updates the cost since the beginning of the year by adding the total daily cost of the energy expenses.
	/// Exports the data to the save system.
	/// If the year has changed, subtracts the cost since the beginning of the year from the player's money and resets it to 0.
	/// </summary>
	public void ChangeDate()
	{
		if (GetParent().GetParent().Name == "ControlDate")
		{
			CostSinceBeginingYear += TotalDailyCost / 2;
			ExportData();
			if (LastDate[2] != CurrentDate[2])
			{
				Global.Inventory.ModifyMoney(-CostSinceBeginingYear);
				CostSinceBeginingYear = 0;
			}
		}
	}

	/// <summary>
	/// Exports the current energy cost data to the save system.
	/// This includes the selected items from the dictionary, the cost since the beginning of the year,
	/// the total daily cost, and the current date. The data is stored in the "Cout Energetique" section
	/// of the global save interaction.
	/// </summary>
	public void ExportData()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Dictionary = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
		foreach (KeyValuePair<string, string> Entry in DictionarySave)
		{
			Dictionary[Entry.Key] = new Godot.Collections.Array<string> { Entry.Value };
		}
		Dictionary["CoutDepuisDebutAnnee"] = new Godot.Collections.Array<string> { CostSinceBeginingYear.ToString() };
		Dictionary["CoutJournalier"] = new Godot.Collections.Array<string> { TotalDailyCost.ToString() };
		Dictionary["Date"] = new Godot.Collections.Array<string> { CurrentDate[0].ToString(), CurrentDate[1].ToString(), CurrentDate[2].ToString() };
		Global.SaveInteract["Cout Energetique"].KeepData(Dictionary);
	}
}
