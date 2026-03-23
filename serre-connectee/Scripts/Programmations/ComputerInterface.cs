using Godot;
using System;

public partial class ComputerInterface : Node2D
{

	/// <summary>
	/// Called when entering the OrdinateurInterface scene. Loads the computer data and refreshes the display.
	/// </summary>
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		LoadComputer();
		RefreshDisplay();
		if (GetParent().Name != "PanneauAmelioration")
		{
			PopupManager.ShowPopup("Dans l'ordinateur, vous pouvez programmer différentes actions qui s'executeront tous les jours. Essayez de programmer l'arrosage d'un pot !");
		}
	}


	/// <summary>
	/// Refreshes the display of the OrdinateurInterface scene with the current values of the glass house.
	/// </summary>
	public void RefreshDisplay()
	{
		if (Global.SaveInteract["Calendrier"].SendData() is null)
		{
			DateControlScript Date = new DateControlScript();
			Date.LoadWeather();
			Date.ExportData();
		}

		if (Global.SaveInteract["Calendrier"].SendData() is not null)
		{
			int LumLamps = CalculateLampsLuminosity();
			int LumShutters = CalculateBlindLuminosity();
			GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer/Lampes").Text = LumLamps.ToString() + " lux";
			GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer/Volets").Text = LumShutters.ToString() + " lux";
			if (LumLamps > LumShutters)
			{
				Global.GlassHouseLuminosity = LumLamps;
			}
			else
			{
				Global.GlassHouseLuminosity = LumShutters;
			}
		}
		for (int i = 1; i <= Global.POTSNUMBER; i++)
		{
			Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> PotData = Global.SaveInteract["Plan de Terre " + i].SendData();
			if (PotData is null)
			{
				GridControlScript Pot = new GridControlScript();
			}
			double Humi = CalculatePotHumidity(i);
			double[] ProgrammedHumi = GetNode<PotsProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationPot" + i.ToString()).GetValuesProg();
			if (i < 4)
			{
				if (ProgrammedHumi is null)
				{
					GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer3/Label" + i).Text = "[] %";
				}
				else
				{

					if (Humi == -1)
					{
						GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer3/Label" + i).Text = ProgrammedHumi[0].ToString() + " %";
					}
					else
					{
						GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer3/Label" + i).Text = Humi.ToString() + " %";
					}
				}

			}
			else
			{
				if (ProgrammedHumi is null)
				{
					GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer4/Label" + i).Text = "[] %";
				}
				else
				{

					if (Humi == -1)
					{
						GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer4/Label" + i).Text = ProgrammedHumi[0].ToString() + " %";
					}
					else
					{
						GetNode<Label>("ControlPanneauPrincipal/PanneauPrincipal/VBoxContainer4/Label" + i).Text = Humi.ToString() + " %";
					}
				}
			}
		}
	}

	/// <summary>
	/// Loads the data saved in the "Programmation" savegame
	/// and loads it into the PanneauProgrammation children.
	/// </summary>
	public void LoadComputer()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Programmation"].SendData();
		if (Data != null)
		{
			GetNode<LampProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationLampes").Loading(Data["Lampes"]);

			GetNode<ShuttersProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationVolets").Loading(Data["Volets"]);

			for (int i = 1; i <= Global.POTSNUMBER; i++)
			{
				GetNode<PotsProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationPot" + i.ToString()).Loading(Data["Pot" + i.ToString()]);
			}

		}
	}

	/// <summary>
	/// Calculates the luminosity provided by the lamps based on the current
	/// shutter luminosity and programmed lamp luminosity values.
	/// This function retrieves the shutter luminosity using the CalculateBlindLuminosity method 
	/// and compares it against the programmed threshold values for the lamps. 
	/// Depending on the result of this comparison, it returns the appropriate lamp luminosity value.
	/// </summary>
	/// <returns>
	/// The calculated luminosity of the lamps if the programmed values are available, 
	/// otherwise returns 0 if the lamp values are not available.
	/// </returns>
	public int CalculateLampsLuminosity()
	{
		int LumShutters = CalculateBlindLuminosity();
		double[] LumLamps = GetNode<LampProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationLampes").GetValuesProg();
		int Luminosity = 0;
		if (LumLamps is not null)
		{
			if (LumShutters < LumLamps[1])
			{
				Luminosity = (int)LumLamps[0];
			}
			else
			{
				Luminosity = (int)LumLamps[2];
			}
			return Luminosity;
		}
		return 0;
	}


	/// <summary>
	/// Calculates the luminosity provided by the shutters based on the current
	/// weather luminosity and programmed shutter luminosity values.
	/// This function retrieves the weather luminosity using the Global.SaveInteract["Calendrier"].SendData() method 
	/// and compares it against the programmed values for the shutters. 
	/// Depending on the result of this comparison, it returns the appropriate shutter luminosity value.
	/// </summary>
	/// <returns>
	/// The calculated luminosity of the shutters if the programmed values are available, 
	/// otherwise returns 0 if the shutter values are not available.
	/// </returns>
	public int CalculateBlindLuminosity()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> WeatherData = Global.SaveInteract["Calendrier"].SendData();
		if (WeatherData is not null)
		{
			int LumWeather = Convert.ToInt32(WeatherData["Weather"][3]);
			double[] LumShutters = GetNode<ShuttersProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationVolets").GetValuesProg();
			int Luminosity = 0;
			if (LumShutters is not null)
			{
				if (LumWeather < LumShutters[1])
				{
					Luminosity = (int)LumShutters[0];
				}
				else
				{
					Luminosity = (int)LumShutters[2];
				}
				return Luminosity;
			}
			return LumWeather;
		}
		return 0;
	}

	/// <summary>
	/// Calculates the average humidity of a specific pot based on the programmed humidity values
	/// and updates the pot data accordingly.
	/// </summary>
	/// <param name="Index">The index of the pot for which the humidity is to be calculated.</param>
	/// <returns>
	/// The average humidity value of the pot if the programmed values are available and pot data is valid,
	/// otherwise returns -1 if the pot data is not available or programmed values are missing.
	/// </returns>
	public double CalculatePotHumidity(int Index)
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> PotData = Global.SaveInteract["Plan de Terre " + Index].SendData();
		if (PotData is not null)
		{
			double[] HumidityPot = GetNode<PotsProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationPot" + Index).GetValuesProg();
			if (HumidityPot is not null)
			{
				double HumidityMoyenne = 0;
				Godot.Collections.Array<string> CellsNamesArray = new Godot.Collections.Array<string> { "a1", "a2", "a3", "b1", "b2", "b3", "c1", "c2", "c3" };
				for (int i = 0; i < Global.POTSNUMBER; i++)
				{
					if (Convert.ToDouble(PotData[CellsNamesArray[i]][0]) < ((int)HumidityPot[1]))
					{
						PotData[CellsNamesArray[i]][0] = Mathf.Snapped((HumidityPot[0] % 101), 0.01).ToString();
						HumidityMoyenne += HumidityPot[0] % 101;
					}
					else
					{
						PotData[CellsNamesArray[i]][0] = Mathf.Snapped((HumidityPot[2] % 101), 0.01).ToString();
						HumidityMoyenne += HumidityPot[2] % 101;
					}
				}
				Global.SaveInteract["Plan de Terre " + Index].KeepData(PotData);
				return Mathf.Snapped((HumidityMoyenne / Global.POTSNUMBER), 0.01);
			}
		}
		return -1;

	}

	/// <summary>
	/// Handles the input of the user in the OrdinateurInterface scene.
	/// If the escape key is pressed or the interact action is pressed, the data is exported and the scene is changed to the Gameplay scene.
	/// The mouse mode is set to captured.
	/// </summary>
	/// <param name="@event">The input event.</param>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Pressed && (EventKey.IsActionPressed("Interact") || EventKey.Keycode == Key.Escape))
			{
				ExportData();
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
	}

	/// <summary>
	/// Exports the current state of the OrdinateurInterface's UI components to a dictionary,
	/// which is then stored in the global save interaction under the "Programmation" section.
	/// This function collects data from the PanneauProgrammationLampes, PanneauProgrammationVolets,
	/// and multiple PanneauProgrammationPots nodes.
	/// </summary>
	public void ExportData()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();

		Data["Lampes"] = GetNode<LampProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationLampes").ExportData();
		Data["Volets"] = GetNode<ShuttersProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationVolets").ExportData();

		for (int i = 1; i <= Global.POTSNUMBER; i++)
		{
			Data["Pot" + i.ToString()] = GetNode<PotsProgrammingPanel>("ControlPanneauxSecondaires/PanneauProgrammationPot" + i.ToString()).ExportData();
		}
		Global.SaveInteract["Programmation"].KeepData(Data);
	}
}