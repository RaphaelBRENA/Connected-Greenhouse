using Godot;
using System;

public partial class LampsOptionButton : OptionButton
{
	/// <summary>
	/// Called when the selected item changes.
	/// If the selected item is 1, hide the second label, the spin box, and the third label.
	/// In other cases, make them visible.
	/// </summary>
	/// <param name="Index">The index of the selected item.</param>
	public void OnItemSelected(int Index)
	{
		if (Index == 1)
		{
			GetParent().GetNode<Label>("Label2").Visible = false;
			GetParent().GetNode<SpinBox>("SpinBox").Visible = false;
			GetParent().GetNode<Label>("Label3").Visible = false;
		}
		else
		{
			GetParent().GetNode<Label>("Label2").Visible = true;
			GetParent().GetNode<SpinBox>("SpinBox").Visible = true;
			GetParent().GetNode<Label>("Label3").Visible = true;
		}
	}
}
