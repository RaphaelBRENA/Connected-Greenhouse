using Godot;
using System;

public partial class PotsProgrammingPanel : Panel
{
	OptionButton OptionButtonIf;
	SpinBox SpinBoxRepeat;
	SpinBox SpinBoxIf;
	SpinBox SpinBoxActionIf;
	SpinBox SpinBoxActionIfNot;
	bool IsProgrammed = false;

	/// <summary>
	/// Called when entering the OrdinateurInterface scene. Set the references to the UI elements.
	/// </summary>
	public override void _Ready()
	{
		SpinBoxRepeat = GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox");
		OptionButtonIf = GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2");
		SpinBoxIf = GetNode<SpinBox>("ContainerBlocs/Si/SpinBox");
		SpinBoxActionIf = GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox");
		SpinBoxActionIfNot = GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox");
	}

	/// <summary>
	/// Loads the provided data into the respective UI elements.
	/// This function expects an array of strings where each element
	/// corresponds to a specific UI component's value, which will be parsed
	/// and set to the component:
	/// Index 0: SpinBox for repetition count
	/// Index 1: Selected index for the 'Si' option button
	/// Index 2: SpinBox for 'Si' condition value
	/// Index 3: SpinBox for 'ActionSi' value
	/// Index 4: SpinBox for 'ActionSinon' value
	/// Index 5: Pressed state for the CheckButton
	/// </summary>
	/// <param name="Data">An array of strings containing data to load into the UI components.</param>
	public void Loading(Godot.Collections.Array<string> Data)
	{

		GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value = double.Parse(Data[0]);
		GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected = int.Parse(Data[1]);
		GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value = double.Parse(Data[2]);
		GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value = int.Parse(Data[3]);
		GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value = int.Parse(Data[4]);
		GetNode<CheckButton>("CheckButton").SetPressed(bool.Parse(Data[5]));

	}


	/// <summary>
	/// Saves the current state of the UI components into an array of strings
	/// that can be saved to a file.
	/// The array contains the following elements:
	/// Index 0: SpinBox for repetition count
	/// Index 1: Selected index for the 'Si' option button
	/// Index 2: SpinBox for 'Si' condition value
	/// Index 3: SpinBox for 'ActionSi' value
	/// Index 4: SpinBox for 'ActionSinon' value
	/// Index 5: Pressed state for the CheckButton
	/// </summary>
	public Godot.Collections.Array<string> ExportData()
	{
		Godot.Collections.Array<string> Data = new Godot.Collections.Array<string>();
		Data.Add(GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value.ToString());
		Data.Add(GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value.ToString());
		Data.Add(GetNode<CheckButton>("CheckButton").IsPressed().ToString());
		return Data;
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
	/// Check if the panel is programmed.
	/// </summary>
	/// <returns>True if the panel is programmed and the isCorrect function returns true.
	/// false otherwise.</returns>
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
	/// Retrieves the programmed values as an array if the current panel is programmed.
	/// </summary>
	/// <returns>
	/// A double array containing the values of the SpinBoxes and OptionButton selections.
	/// Index 0 and 2 correspond to 'ActionSi' and 'ActionSinon' values based on the selected option.
	/// Index 1 corresponds to the 'Si' condition value.
	/// Returns null if the panel is not programmed.
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
}
