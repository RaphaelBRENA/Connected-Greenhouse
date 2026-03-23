using Godot;
using System;

public partial class Popup : Control
{
	private Godot.Popup MyPopup;
	private Label TextLabel;
	private Button MyButton;

	/// <summary>
	/// Called when the Popup node enters the scene tree.
	/// Sets the references to the popup and the text label.
	/// </summary>
	public override void _Ready()
	{
		MyPopup = GetNode<Godot.Popup>("Popup");
		TextLabel = GetNode<Label>("Popup/Label");
	}

	public void ShowPopup(string text,string title)
		
	/// <summary>
	/// Displays a popup with the specified text.
	/// Sets the text of the label, adjusts its size, and centers the popup on the screen.
	/// </summary>
	/// <param name="text">The text to display in the popup.</param>
	/// <param name="title">The title to display in the popup.</param>
	{
		MyPopup.Title = title;
		ConfigFile Save = new ConfigFile();
		Save.Load("user://Save/" + Global.CurrentSaveName + ".cfg");
		TextLabel.Text = text;
		TextLabel.SetSize(new Vector2(292, TextLabel.Size.Y));
		MyPopup.PopupCentered();
	}
}

