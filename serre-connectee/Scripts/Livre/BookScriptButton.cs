using Godot;
using System;

public partial class BookScriptButton : Button
{
	public void OnButtonPressed()
	{
		if (this.Name == "OngletPlantes")
		{
			GetNode<Panel>("../Encyclopedie").Show();
			GetNode<Panel>("../Materiel").Hide();
			GetNode<Panel>("../Guides").Hide();
		}
		if (this.Name == "OngletMateriel")
		{
			GetNode<Panel>("../Materiel").Show();
			GetNode<Panel>("../Encyclopedie").Hide();
			GetNode<Panel>("../Guides").Hide();
		}
		if (this.Name == "OngletGuides")
		{
			GetNode<Panel>("../Guides").Show();
			GetNode<Panel>("../Materiel").Hide();
			GetNode<Panel>("../Encyclopedie").Hide();
		}


	}
}
