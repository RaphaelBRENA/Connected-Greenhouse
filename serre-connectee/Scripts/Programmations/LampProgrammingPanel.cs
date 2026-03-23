using Godot;
using System;

public partial class LampProgrammingPanel : Panel
{
	OptionButton OptionButtonIf;
	SpinBox SpinBoxRepeat;
	SpinBox SpinBoxIf;
	OptionButton OptionButtonActionIf;
	SpinBox SpinBoxActionIf;
	OptionButton OptionButtonActionIfNot;
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
		OptionButtonActionIf = GetNode<OptionButton>("ContainerBlocs/ActionSi/OptionButtonSi");
		SpinBoxActionIf = GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox");
		OptionButtonActionIfNot = GetNode<OptionButton>("ContainerBlocs/ActionSinon/OptionButtonSinon");
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
	/// Index 3: Selected index for the 'ActionSi' option button
	/// Index 4: SpinBox for 'ActionSi' value
	/// Index 5: Selected index for the 'ActionSinon' option button
	/// Index 6: SpinBox for 'ActionSinon' value
	/// Index 7: Pressed state for the CheckButton
	/// After setting the values, it refreshes the visibility of certain UI elements.
	/// </summary>
	/// <param name="Data">An array of strings containing data to load into the UI components.</param>
	public void Loading(Godot.Collections.Array<string> Data)
	{

		GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value = double.Parse(Data[0]);
		GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected = int.Parse(Data[1]);
		GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value = double.Parse(Data[2]);
		GetNode<OptionButton>("ContainerBlocs/ActionSi/OptionButtonSi").Selected = int.Parse(Data[3]);
		GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value = double.Parse(Data[4]);
		GetNode<OptionButton>("ContainerBlocs/ActionSinon/OptionButtonSinon").Selected = int.Parse(Data[5]);
		GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value = double.Parse(Data[6]);
		GetNode<CheckButton>("CheckButton").SetPressed(bool.Parse(Data[7]));
		RefreshLines();
	}

	/// <summary>
	/// Exports the current state of the UI components into an array of strings
	/// that can be saved to a file. The array contains the following elements:
	/// Index 0: SpinBox for repetition count
	/// Index 1: Selected index for the 'Si' option button
	/// Index 2: SpinBox for 'Si' condition value
	/// Index 3: Selected index for the 'ActionSi' option button
	/// Index 4: SpinBox for 'ActionSi' value
	/// Index 5: Selected index for the 'ActionSinon' option button
	/// Index 6: SpinBox for 'ActionSinon' value
	/// Index 7: Pressed state for the CheckButton
	/// </summary>
	public Godot.Collections.Array<string> ExportData()
	{
		Godot.Collections.Array<string> Data = new Godot.Collections.Array<string>();
		Data.Add(GetNode<SpinBox>("ContainerBlocs/Repeter/SpinBox").Value.ToString());
		Data.Add(GetNode<OptionButton>("ContainerBlocs/Si/OptionButton2").Selected.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/Si/SpinBox").Value.ToString());
		Data.Add(GetNode<OptionButton>("ContainerBlocs/ActionSi/OptionButtonSi").Selected.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Value.ToString());
		Data.Add(GetNode<OptionButton>("ContainerBlocs/ActionSinon/OptionButtonSinon").Selected.ToString());
		Data.Add(GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Value.ToString());
		Data.Add(GetNode<CheckButton>("CheckButton").IsPressed().ToString());

		return Data;
	}

	/// <summary>
	/// Updates the visibility of UI components based on the selected options for
	/// 'ActionSi' and 'ActionSinon'. If the selected option is 0, the corresponding
	/// labels and spin boxes are made visible in the interface.
	/// </summary>
	public void RefreshLines()
	{
		if (GetNode<OptionButton>("ContainerBlocs/ActionSi/OptionButtonSi").Selected == 0)
		{
			GetNode<Label>("ContainerBlocs/ActionSi/Label2").Visible = true;
			GetNode<SpinBox>("ContainerBlocs/ActionSi/SpinBox").Visible = true;
			GetNode<Label>("ContainerBlocs/ActionSi/Label3").Visible = true;
		}
		if (GetNode<OptionButton>("ContainerBlocs/ActionSinon/OptionButtonSinon").Selected == 0)
		{
			GetNode<Label>("ContainerBlocs/ActionSinon/Label2").Visible = true;
			GetNode<SpinBox>("ContainerBlocs/ActionSinon/SpinBox").Visible = true;
			GetNode<Label>("ContainerBlocs/ActionSinon/Label3").Visible = true;
		}
	}

	/// <summary>
	/// Determines whether all required conditions are correctly set by checking the selected
	/// IDs of the option buttons related to the 'Si', 'ActionSi', and 'ActionSinon' conditions.
	/// </summary>
	/// <returns>True if all option buttons have valid selections; otherwise, false.</returns>
	public bool IsCorrect()
	{
		if (OptionButtonIf.GetSelectedId() == -1)
		{
			return false;
		}
		else if (OptionButtonActionIf.GetSelectedId() == -1)
		{
			return false;
		}
		else if (OptionButtonActionIfNot.GetSelectedId() == -1)
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
			OptionButtonActionIf.Disabled = false;
			SpinBoxActionIf.Editable = true;
			OptionButtonActionIfNot.Disabled = false;
			SpinBoxActionIfNot.Editable = true;
			IsProgrammed = true;
		}
		else
		{
			SpinBoxRepeat.Editable = false;
			OptionButtonIf.Disabled = true;
			SpinBoxIf.Editable = false;
			OptionButtonActionIf.Disabled = true;
			SpinBoxActionIf.Editable = false;
			OptionButtonActionIfNot.Disabled = true;
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
				if (OptionButtonActionIf.GetSelectedId() == 1)
				{
					CurrentArray[0] = 0;
				}
				if (OptionButtonActionIfNot.GetSelectedId() == 1)
				{
					CurrentArray[2] = 0;
				}
			}
			else if (OptionButtonIf.GetSelectedId() == 1)
			{
				CurrentArray[2] = SpinBoxActionIf.Value;
				CurrentArray[0] = SpinBoxActionIfNot.Value;
				if (OptionButtonActionIf.GetSelectedId() == 1)
				{
					CurrentArray[2] = 0;
				}
				if (OptionButtonActionIfNot.GetSelectedId() == 1)
				{
					CurrentArray[0] = 0;
				}
			}
			return CurrentArray;
		}
		return null;
	}
}
