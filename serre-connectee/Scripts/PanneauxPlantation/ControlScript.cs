using Godot;
using System;

public partial class ControlScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________


	/*
	ActionCursor est statique car il doit pouvoir être récupéré 
	par les cases de plantation. Pour augmenter leur humidité si le curseur est un arrosoir par exemple.
	ActionCursor peut prendre les valeurs (string) suivantes :
	normal,arrosoir,compost,pesticide,bouillie
	*/
	public static string ActionCursor;
	
	public Resource WateringCursor = ResourceLoader.Load("../Assets/Images/ImagesPots/Arrosoir.png");
	public Resource CompostCursor = ResourceLoader.Load("../Assets/Images/ImagesPots/Compost.png");
	public Resource ChemicalsCursor = ResourceLoader.Load("../Assets/Images/ImagesPots/Pesticide.png");
	public Resource MixtureCursor = ResourceLoader.Load("../Assets/Images/ImagesPots/Bouillie.png");


	//READY ___________________________________________________________________________________________


	public override void _Ready()
	{
		ActionCursor = "normal";
	}
	public override void _Process(double delta) { }


	//SIGNAUX _________________________________________________________________________________________



	public void OnWateringCanPressed()
	{
		//Si le bouton Arroser du Menu est cliqué on change le curseur
		if (ActionCursor != "arrosoir")
		{
			Input.SetCustomMouseCursor(WateringCursor);
			ActionCursor = "arrosoir";
		}
		else
		{
			Input.SetCustomMouseCursor(null);
			ActionCursor = "normal";
		}
	}


	public void OnCompostPressed()
	{
		//Si le bouton Compost du Menu est cliqué on change le curseur
		if (ActionCursor != "compost")
		{
			Input.SetCustomMouseCursor(CompostCursor);
			ActionCursor = "compost";
		}
		else
		{
			Input.SetCustomMouseCursor(null);
			ActionCursor = "normal";
		}

	}


	public void OnChemicalsPressed()
	{
		//Si le bouton Pesticide du Menu est cliqué on change le curseur
		if (ActionCursor != "pesticide")
		{
			Input.SetCustomMouseCursor(ChemicalsCursor);
			ActionCursor = "pesticide";
		}
		else
		{
			Input.SetCustomMouseCursor(null);
			ActionCursor = "normal";
		}
	}

	public void OnTreatPressed()
	{
		//Si le bouton Traiter du Menu est cliqué on change le curseur
		if (ActionCursor != "bouillie")
		{
			Input.SetCustomMouseCursor(MixtureCursor);
			ActionCursor = "bouillie";
		}
		else
		{
			Input.SetCustomMouseCursor(null);
			ActionCursor = "normal";
		}

	}

	public void OnCroixPressed()
	{
		//Si le bouton Croix du Menu est cliqué on change le curseur
		Input.SetCustomMouseCursor(null);
	    ActionCursor = "normal";

	}
}
