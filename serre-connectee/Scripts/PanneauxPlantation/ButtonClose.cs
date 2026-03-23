using Godot;
using System;

public partial class ButtonClose : Button
{
	public void OnButtonPressed(){
		GetParent<Panel>().Hide();
	}
}
