using Godot;
using System;

public partial class ContextComponent : CenterContainer
{
	[Export] public TextureRect Icon = new TextureRect();
	[Export] public Label Context = new Label();
	[Export] public Texture2D DefaultIcon = new Texture2D();
 	public override void _Ready()
	{
		Global.UiContext = this;
		Reset();
	}
	public void Reset()
	{
		Icon.Texture = null;
		Context.Text = "";
	}
	public void UpdateIcon(Texture2D Image, Boolean ChangeIcon)
	{
		if (ChangeIcon)
			Icon.Texture = Image;
		else
			Icon.Texture = DefaultIcon;
	}
	public void UpdateContext(String CurrentText)
	{
		Context.Text = CurrentText;
	}
}
