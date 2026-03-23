using Godot;
using Godot.NativeInterop;
using System;
using Godot.Collections;
using Newtonsoft.Json;

public partial class SaveChoiceMenu : Control
{
	// VARIABLES _______________________________________________________________________________________

	ConfigFile SaveFile1, SaveFile2, SaveFile3;
	private int SaveToDelete;

	// READY ___________________________________________________________________________________________

	public override void _Ready()
	{
		SaveToDelete = 0;
		LoadHUD();
		InitSaveInteract();
		InitWeather();
		Global.Inventory.Initialisation();
	}
	
	
	public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey EventKey)
			if (EventKey.Pressed && EventKey.Keycode == Key.Escape)
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Lancement/StartMenu.tscn");
    }
	public static void InitSaveInteract(){
		//Fonction d'initialisation du dictionnaire de gestionnaires de sauvegarde des informations de chaque interface
		Global.SaveInteract = new Godot.Collections.Dictionary<string, SaveManager>{
			{ "Calendrier", new SaveManager() },
			{ "Coffre", new SaveManager() },
			{ "Boutique", new SaveManager() },
			{ "Programmation", new SaveManager() },
			{ "Thermostat", new SaveManager() },
			{ "Cout Energetique", new SaveManager() }
		};
		for (int i=1; i<=Global.POTSNUMBER;i++){
			Global.SaveInteract["Plan de Terre "+i.ToString()] = new SaveManager();
		}
	}
	
	public static void InitWeather(){
		//Fonction d'initialisation de la météo
		if(Global.SaveInteract["Calendrier"].SendData() is null){
			DateControlScript Date = new DateControlScript();
			Date.LoadWeather();
			Date.ExportData();
			Global.GlassHouseLuminosity = Convert.ToInt32(Global.SaveInteract["Calendrier"].SendData()["Weather"][3]);
			Global.GlassHouseTemperature = Convert.ToInt32(Global.SaveInteract["Calendrier"].SendData()["Weather"][1]);
		}
	}
	// SIGNAUX _________________________________________________________________________________________

	public void OnSave1Pressed()
	{
		Global.Coordinates = new Vector3(0, 0, 0);
		Global.RotationCamera = new Vector3(0, 0, 0);
		Global.RotationPlayer = new Vector3(0, 0, 0);
		Global.CurrentSaveName = "save1";
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
	}

	public void OnSave2Pressed()
	{
		Global.Coordinates = new Vector3(0, 0, 0);
		Global.RotationCamera = new Vector3(0, 0, 0);
		Global.RotationPlayer = new Vector3(0, 0, 0);
		Global.CurrentSaveName = "save2";
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
	}

	public void OnSave3Pressed()
	{
		Global.Coordinates = new Vector3(0, 0, 0);
		Global.RotationCamera = new Vector3(0, 0, 0);
		Global.RotationPlayer = new Vector3(0, 0, 0);
		Global.CurrentSaveName = "save3";
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
	}

	private void LoadHUD() 
	{
		string DirectoryPath = "user://Save";
		var Dir = DirAccess.Open(DirectoryPath);
		if(Dir==null){
			DirAccess.MakeDirAbsolute(DirectoryPath);
		}

		for (int i = 1; i <= 3; i++) 
		{
			GetNode<Label>($"Colonne1/Save {i}/Label").Hide();
			GetNode<Label>($"Colonne1/Save {i}/Label2").Hide();

			String SaveFilePath = $"user://Save/save{i}.json";
			bool DoesSaveFileExists = FileAccess.FileExists(SaveFilePath);
			if(!DoesSaveFileExists){
				var TempFile = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
				TempFile.Close();
			}
			var SaveFile = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read);

			Array<Dictionary<string, Array<string>>> Data = JsonConvert.DeserializeObject<Array<Dictionary<string, Array<string>>>>(SaveFile.GetLine());
			if (Data != null)
			{
				if (Data[0].Count != 0 && Data[1].Count != 0) 
				{
					GetNode<Label>($"Colonne1/Save {i}/Label").Text
					= Data[0]["Generaux"][1].ToString();

					string CurrentText = "Saison : " + Data[1]["Date"][1].ToString() + "  Date : " 
					+ Data[1]["Date"][2].ToString() + "-" + Data[1]["Date"][0].ToString();

					GetNode<Label>($"Colonne1/Save {i}/Label2").Text = CurrentText;

					GetNode<Label>($"Colonne1/Save {i}/Label").Show();
					GetNode<Label>($"Colonne1/Save {i}/Label2").Show();
				}
			}
			else
			{
				GetNode<Button>($"Colonne1/Save {i}").Text = "Nouvelle Partie";
			}
		}
	}

	public void OnDelete1Pressed()
	{
		ShowPopup("1ère");
		SaveToDelete = 1;
	}

	public void OnDelete2Pressed()
	{
		ShowPopup("2ème");
		SaveToDelete = 2;
	}

	public void OnDelete3Pressed()
	{
		ShowPopup("3ème");
		SaveToDelete = 3;
	}

	private void ShowPopup(string PopupNumber){
		GetNode<ColorRect>("ColorRect").Show();
		Panel Popup = GetNode<Panel>("Popup");
		Popup.GetChild<Label>(4).SetText("Voulez-vous vraiment supprimer la " + PopupNumber + " sauvegarde ?");
		Popup.Show();
	}

	public void OnAcceptPressed()
	{
		if (SaveToDelete>0 && SaveToDelete<=3) {
			var SaveFile = FileAccess.Open("user://Save/save"+SaveToDelete+".json", FileAccess.ModeFlags.Write);
			SaveFile.StoreLine(null);
			LoadHUD();
			OnClosePressed();
		} 
	}

	public void OnClosePressed()
	{
		GetNode<ColorRect>("ColorRect").Hide();
		GetNode<Panel>("Popup").Hide();
		SaveToDelete = 0;
	}
}
