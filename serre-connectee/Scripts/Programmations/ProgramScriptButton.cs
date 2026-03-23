using Godot;
using System;

public partial class ProgramScriptButton : Button
{
	/// <summary>
	/// Called when the buttons are pressed.
	/// This function is connected to the "pressed" signal of the buttons.
	/// It shows the appropriate panel depending on the name of the parent and hide the current panel.
	/// </summary>
	public void OnButtonPressed()
	{
		if (GetParent().Name == "PanneauThermostat")
		{
			GetNode<Panel>("../../../ControlPanneau2/PanneauProgrammation").Show();
			GetNode<Panel>("../").Hide();
		}
		else if (GetParent().Name == "PanneauProgrammation")
		{
			GetNode<Panel>("../../../ControlPanneau1/PanneauThermostat").Show();
			GetNode<Panel>("../").Hide();
			if (Global.SaveInteract["Calendrier"].SendData() is not null)
			{
				Global.GlassHouseTemperature = (int)((ThermostatProgrammingPanel)(GetParent().GetParent().GetParent().GetNode<Panel>("ControlPanneau2/PanneauProgrammation"))).CalculateGlassHouseTemperature();
				double ProgrammationTemp = ((ThermostatProgrammingPanel)(GetParent().GetParent().GetParent().GetNode<Panel>("ControlPanneau2/PanneauProgrammation"))).EffectiveProgrammedValue();
				GetNode<Label>("../../../ControlPanneau1/PanneauThermostat/Température").Text = Global.GlassHouseTemperature.ToString("0.0");
				if (ProgrammationTemp != -1)
				{
					GetNode<Label>("../../../ControlPanneau1/PanneauThermostat/ValeurJour").Text = ProgrammationTemp.ToString("0.0");
				}
				else
				{
					GetNode<Label>("../../../ControlPanneau1/PanneauThermostat/ValeurJour").Text = "[]°C";
				}

			}
		}
		else if (GetParent().GetParent().Name == "ControlPanneauxSecondaires")
		{
			GetNode<Panel>("../../../ControlPanneauPrincipal/PanneauPrincipal").Show();
			GetNode<Panel>("../").Hide();
			GetNode<ComputerInterface>("../../../").RefreshDisplay();
		}
		else if (GetParent().GetParent().Name == "PanneauPrincipal")
		{
			GetNode<Panel>("../../../../ControlPanneauxSecondaires/Panneau" + this.Name).Show();
			GetNode<Panel>("../../").Hide();
			PopupManager.ShowPopup("Essayez de modifier les lignes de code pour automatiser votre serre !");
		}
	}
}
