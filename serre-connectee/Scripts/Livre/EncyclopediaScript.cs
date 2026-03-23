using Godot;
using System;

public partial class EncyclopediaScript : Panel
{
	//ATTRIBUTS _____________________________________________________________________________________________________________

	private int PageNumber; //Numero de la page actuelle
	private Godot.Collections.Array<String> PlantsArray; //Liste des plantes du jeu
	private Godot.Collections.Array<String> StagesArray; //Liste des états de chaque plante du jeu
	private Godot.Collections.Array<String> DescriptionsArray; //Liste des descriptions de chaque plante du jeu

	//READY _______________________________________________________________________________________________________________

	public override void _Ready()
	{
		//Fonction d'initialisation de l'encyclopédie
		Input.MouseMode = Input.MouseModeEnum.Visible;
		PlantsArray = new Godot.Collections.Array<String>(){"Haricot","Ciboulette","Lavande","Lentille","Basilic",
						"Origan","Menthe","Sauge","Persil","Aneth","Cosmos"};
		StagesArray = new Godot.Collections.Array<String>() { "Graine", "Pousse", "Avant-Dernier", "Dernier", "Produit", "Mort" };
		DescriptionsArray = new Godot.Collections.Array<String>(){
			"Les haricots sont utilisés dans de nombreux plats.",
			"La tige de ciboulette permet d'arômatiser diverses préparations.",
			"La lavande peut être consommée en tisane, en confiture, ou simplement séchée pour parfumer une pièce.",
			"Les lentilles sont consommées en graines dans de multiples plats.",
			"Les feuilles de basilic arômatisent de nombreuses préparations, et sont notamment placées sur les pizzas.",
			"Les feuilles d'origan arômatisent divers mets.",
			"Les feuilles de menthe donnent du goût à bien des préparations, et même des boissons.",
			"Les feuilles de sauge permettent d'arômatiser de nombreux plats.",
			"Le persil est utilisé dans de nombreuses recettes pour donner du goût.",
			"Les tiges d'aneth sont souvent utilisées dans des salades.",
			"Les cosmos font de parfaits bouquets, et certaines espèces ont une fleur comestible."
		};
		PageNumber = 0;
		LoadPage();
	}

	//METHODES _____________________________________________________________________________________________________________

	private void LoadPage()
	{
		//Fonction qui affiche la page courante
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[0]);
		GetNode<Label>("Titre").SetText(PlantsArray[PageNumber]);
		GetNode<Label>("Carte1/Description").SetText(DescriptionsArray[PageNumber]);
		GetNode<TextureRect>("Carte0/Graine").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Graine.png");
		GetNode<TextureRect>("Carte0/Produit").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + ".png");
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Graine.png");
		LoadConditions();
	}

	private void LoadConditions()
	{
		//Fonction qui affiche les conditions optimales de croissance de la plante de la page
		Plant CurrentPlant = new Plant("Graines " + PlantsArray[PageNumber]);
		GetNode<Label>("Carte1/Conditions/Temperature").SetText(CurrentPlant.GetOptimalTemperature());
		GetNode<Label>("Carte1/Conditions/Humidite").SetText(CurrentPlant.GetOptimalHumidity());
		GetNode<Label>("Carte1/Conditions/Luminosite").SetText(CurrentPlant.GetOptimalLuminosity());
		GetNode<Label>("JoursParStade").SetText("Jours par stade : "+CurrentPlant.GetDaysPerStage());
	}


	//SIGNAUX _____________________________________________________________________________________________________________


	public void OnNextPagePressed()
	{
		//Fonction appelée lorsque le bouton page suivante est cliqué
		if (PageNumber + 1 < PlantsArray.Count)
		{
			PageNumber++;
			LoadPage();
		}
	}
	public void OnPreviousPagePressed()
	{
		//Fonction appelée lorsque le bouton page précédente est cliqué
		if (PageNumber > 0)
		{
			PageNumber--;
			LoadPage();
		}
	}

	public void OnButton0Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Graine.png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[0]);
	}
	public void OnButton1Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Stade1.png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[1]);
	}
	public void OnButton2Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Stade3.png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[2]);
	}
	public void OnButton3Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Stade4.png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[3]);
	}
	public void OnButton4Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + ".png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[4]);
	}
	public void OnButton5Pressed()
	{
		GetNode<TextureRect>("Plante").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + PlantsArray[PageNumber] + "Morte.png");
		GetNode<Label>("Stade").SetText("Stade : " + StagesArray[5]);
	}

}
