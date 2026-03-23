using Godot;
using System;
using System.Collections.Generic;

public partial class ThermostatProgrammingPanel : Panel
{
	private OptionButton OptionButtonIf;
	private SpinBox SpinBoxRepeat;
	private SpinBox SpinBoxIf;
	private SpinBox SpinBoxActionIf;
	private SpinBox SpinBoxActionIfNot;
	bool IsProgrammed = false;

	double GlassHouseTemp;
	double ProgrammationTemp;
	Dictionary<double, List<double>> DictionaryTemp;


	/// <summary>
	/// Called when entering the OrdinateurInterface scene. Set the references to the UI elements, and load the data from the save file.
	/// If the save file is not empty, it also calculates the temperature of the glasshouse and the current programmed value, and updates the UI accordingly.
	/// </summary>
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		if (GetParent().GetParent().GetParent().Name != "PanneauAmelioration")
		{
			PopupManager.ShowPopup("Vous pouvez régler la température de la serre grâce au thermostat. Essayez de le programmer !");
		}
		SpinBoxRepeat = GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox");
		OptionButtonIf = GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2");
		SpinBoxIf = GetNode<SpinBox>("ContainerBlocs/Si/SpinBox");
		SpinBoxActionIf = GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox");
		SpinBoxActionIfNot = GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox");
		LoadThermostat();
		DictionaryTemp = new Dictionary<double, List<double>>();
		if (Global.SaveInteract["Thermostat"].SendData() != null)
		{
			Godot.Collections.Array<string> Array = Global.SaveInteract["Thermostat"].SendData()["ValuesTemp"];
			for (int i = 0; i < Array.Count; i += 3)
			{
				DictionaryTemp[double.Parse(Array[i])] = new List<double>() { double.Parse(Array[i + 1]), double.Parse(Array[i + 2]) };
			}
		}
		if (Global.SaveInteract["Calendrier"].SendData() is not null)
		{
			GlassHouseTemp = CalculateGlassHouseTemperature();
			ProgrammationTemp = EffectiveProgrammedValue();
			GetNode<Label>("../../ControlPanneau1/PanneauThermostat/Température").Text = ((int)GlassHouseTemp).ToString("0.0");
			if (ProgrammationTemp != -1)
			{
				GetNode<Label>("../../ControlPanneau1/PanneauThermostat/ValeurJour").Text = ProgrammationTemp.ToString("0.0");
			}
			else
			{
				GetNode<Label>("../../ControlPanneau1/PanneauThermostat/ValeurJour").Text = "[]°C";
			}
			Global.GlassHouseTemperature = (int)GlassHouseTemp;
		}
	}


	/// <summary>
	/// Loads thermostat data from the saved file into the corresponding UI components.
	/// This function retrieves the saved values for the thermostat saved data, if available, and 
	/// assigns them to the associated SpinBoxes and OptionButton in the interface.
	/// </summary>
	public void LoadThermostat()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Thermostat"].SendData();
		if (Data != null)
		{
			GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value = double.Parse(Data["Thermostat"][0]);
			GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected = int.Parse(Data["Thermostat"][1]);
			GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value = double.Parse(Data["Thermostat"][2]);
			GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value = int.Parse(Data["Thermostat"][3]);
			GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value = int.Parse(Data["Thermostat"][4]);
			GetNode<CheckButton>("CheckButton").SetPressed(bool.Parse(Data["Thermostat"][5]));
		}
	}

	/// <summary>
	/// Called every frame. Loads the weather data from the saved file into the Weather data structure if the saved file does not exist.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	public override void _Process(double delta)
	{
		if (Global.SaveInteract["Calendrier"].SendData() is null)
		{
			DateControlScript Date = new DateControlScript();
			Date.LoadWeather();
			Date.ExportData();
		}
	}

	/// <summary>
	/// Determines whether the current selection in the 'Si' option button is valid.
	/// The function checks if a valid option is selected in the 'Si' option button by
	/// verifying that the selected ID is not -1.
	/// </summary>
	/// <returns>True if a valid selection is made; otherwise, false.</returns>
	public bool IsCorrect()
	{
		if (OptionButtonIf.GetSelectedId() == -1)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// Toggles the editability and enabled state of UI components based on the CheckButton's state.
	/// When active, enables editing for SpinBoxes and enables OptionButton. Otherwise, disables them.
	/// Also sets the IsProgrammed property accordingly.
	/// </summary>
	/// <param name="Actif">Indicates whether the CheckButton is active (true) or inactive (false).</param>
	public void OnCheckButtonToggle(bool Actif)
	{
		if (Actif)
		{
			SpinBoxRepeat.Editable = true;
			OptionButtonIf.Disabled = false;
			SpinBoxIf.Editable = true;
			SpinBoxActionIf.Editable = true;
			SpinBoxActionIfNot.Editable = true;
			IsProgrammed = true;
		}
		else
		{
			SpinBoxRepeat.Editable = false;
			OptionButtonIf.Disabled = true;
			SpinBoxIf.Editable = false;
			SpinBoxActionIf.Editable = false;
			SpinBoxActionIfNot.Editable = false;
			IsProgrammed = false;
		}
	}

	/// <summary>
	/// Checks if the panel is currently programmed and all required conditions are correctly set.
	/// </summary>
	/// <returns>True if the panel is programmed and all conditions are valid; otherwise, false.</returns>
	public bool GetIsProgrammed()
	{
		if (IsProgrammed && IsCorrect())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Retrieves an array of programmed values from the panel if it is currently programmed.
	/// </summary>
	/// <returns>
	/// A double array containing the following values:
	/// - Index 0: 'ActionSi' value or 0 based on the selected option in OptionButtonActionIf.
	/// - Index 1: 'Si' condition value from SpinBoxIf.
	/// - Index 2: 'ActionSinon' value or 0 based on the selected option in OptionButtonActionIfNot.
	/// - Returns null if the panel is not programmed or conditions are not met.
	/// </returns>
	public double[] GetValuesProg()
	{
		if (GetIsProgrammed())
		{
			double[] CurrentArray = new double[3];
			CurrentArray[1] = SpinBoxIf.Value;
			if (OptionButtonIf.GetSelectedId() == 0)
			{
				CurrentArray[0] = SpinBoxActionIf.Value;
				CurrentArray[2] = SpinBoxActionIfNot.Value;
			}
			else if (OptionButtonIf.GetSelectedId() == 1)
			{
				CurrentArray[2] = SpinBoxActionIf.Value;
				CurrentArray[0] = SpinBoxActionIfNot.Value;
			}
			return CurrentArray;
		}
		return null;
	}

	/// <summary>
	/// Calculates the temperature of the glasshouse based on the current weather and thermostat settings.
	/// </summary>
	/// <returns>
	/// The calculated glasshouse temperature as a double. If the thermostat is programmed and the weather
	/// temperature is below the ThermostatTemperature, it returns a random temperature near the 'ActionSi' value. Otherwise,
	/// it returns a random temperature near the 'ActionSinon' value. If no thermostat settings are available,
	/// it defaults to the current weather temperature.
	/// </returns>
	public double CalculateGlassHouseTemperature()
	{
		int WeatherTemperature = Convert.ToInt32(Global.SaveInteract["Calendrier"].SendData()["Weather"][1]);
		double[] ThermostatTemperature = GetValuesProg();
		double Temperature = 0;
		if (ThermostatTemperature is not null)
		{
			if (WeatherTemperature < ThermostatTemperature[1])
			{
				Temperature = GetRandomNear(ThermostatTemperature[0]);
			}
			else
			{
				Temperature = GetRandomNear(ThermostatTemperature[2]);
			}
			return Temperature;
		}
		return WeatherTemperature;
	}


	/// <summary>
	/// Saves the current state of the UI components into a dictionary of string arrays
	/// that can be saved to a file. The dictionary contains the following elements:
	/// - "Thermostat": an array of strings containing the following elements:
	///		+ Index 0: SpinBox for repetition count
	///		+ Index 1: Selected index for the 'Si' option button
	///		+ Index 2: SpinBox for 'Si' condition value
	///		+ Index 3: SpinBox for 'ActionSi' value
	///		+ Index 4: SpinBox for 'ActionSinon' value
	///		+ Index 5: Pressed state for the CheckButton
	/// - "ValuesTemp": an array of strings containing the following elements:
	///		+ Index 0: Key for the temperature in the DictionaryTemp
	///		+ Index 1: Value for the 'ActionSi' in the DictionaryTemp
	///		+ Index 2: Value for the 'ActionSinon' in the DictionaryTemp
	/// </summary>
	public void ExportData()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
		Godot.Collections.Array<string> ThermostatArray = new Godot.Collections.Array<string>();
		ThermostatArray.Add(GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value.ToString());
		ThermostatArray.Add(GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected.ToString());
		ThermostatArray.Add(GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value.ToString());
		ThermostatArray.Add(GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value.ToString());
		ThermostatArray.Add(GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value.ToString());
		ThermostatArray.Add(GetNode<CheckButton>("CheckButton").IsPressed().ToString());
		Data["Thermostat"] = ThermostatArray;

		Godot.Collections.Array<string> ArrayTemp = new Godot.Collections.Array<string>();
		foreach (KeyValuePair<double, List<double>> Entry in DictionaryTemp)
		{
			ArrayTemp.Add(Entry.Key.ToString());
			ArrayTemp.Add(Entry.Value[0].ToString());
			ArrayTemp.Add(Entry.Value[1].ToString());
		}
		Data["ValuesTemp"] = ArrayTemp;
		Global.SaveInteract["Thermostat"].KeepData(Data);
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


	/// <summary>
	/// Determines the effective programmed temperature value based on the current weather and thermostat settings.
	/// </summary>
	/// <returns>
	/// The programmed temperature as a double if the thermostat is set; otherwise, -1. 
	/// The value returned is `ActionSi` if the current weather temperature is below the 'Si' condition, 
	/// and `ActionSinon` if it is not.
	/// </returns>
	public double EffectiveProgrammedValue()
	{
		int WeatherTemperature = Convert.ToInt32(Global.SaveInteract["Calendrier"].SendData()["Weather"][1]);
		double[] TemperatureThermostat = GetValuesProg();
		double Temperature = 0;
		if (TemperatureThermostat is not null)
		{
			if (WeatherTemperature < TemperatureThermostat[1])
			{
				Temperature = TemperatureThermostat[0];
			}
			else
			{
				Temperature = TemperatureThermostat[2];
			}
			return Temperature;
		}
		return -1;
	}

	/// <summary>
	/// Returns a random double value near the given Value.
	/// The amount of offset is based on the current weather and the presence of certain upgrades.
	/// </summary>
	/// <param name="Value">The input value to generate a random value near.</param>
	/// <returns>A random double value within a certain range of the input value.</returns>
	private double GetRandomNear(double Value)
	{
		double MinOffset = 0.1;
		double MaxOffset = 1.9;
		MaxOffset += Math.Abs(double.Parse(Global.SaveInteract["Calendrier"].SendData()["Weather"][1]) - ProgrammationTemp) / 25;
		if (Global.SaveInteract["Cout Energetique"].SendData() != null)
		{
			if (Global.SaveInteract["Cout Energetique"].SendData()["Clime1"][0] == "Amélioration de la fiabilité de la clime")
			{
				MaxOffset -= 0.9;
			}
			if (Global.SaveInteract["Cout Energetique"].SendData()["Clime2"][0] == "Amélioration de la fiabilité de la clime")
			{
				MaxOffset -= 0.9;
			}
		}
		if (DictionaryTemp.ContainsKey(Value) && MaxOffset == DictionaryTemp[Value][1])
		{
			return DictionaryTemp[Value][0];
		}
		else
		{
			Random R = new Random();
			double Offset = R.NextDouble() * (MaxOffset - MinOffset) + MinOffset;
			double RandomValue = R.Next(2) == 0 ? Value - Offset : Value + Offset;
			DictionaryTemp[Value] = new List<double>() { RandomValue, MaxOffset };
			return RandomValue;
		}
	}


	/// <summary>
	/// Handles the input of the user in the PanneauProgrammationThermostat scene.
	/// If the user presses the "Interact" action or the escape key, the data is exported and the scene is changed to the Gameplay scene.
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
			}
		}
	}
}
