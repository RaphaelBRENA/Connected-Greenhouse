using Godot;
using System;

public partial class MaterialScript : Panel
{

	//ATTRIBUTS _____________________________________________________________________________________________________________

	private int PageNumber; //Numero de la page actuelle
	private Godot.Collections.Dictionary<String, String[]> MaterialArray; //Liste du materiel du jeu et sa liste d'états
	private Godot.Collections.Array<String> DescriptionsArray; //Liste des descriptions du materiel du jeu

	//READY _______________________________________________________________________________________________________________

	public override void _Ready()
	{
		MaterialArray = new Godot.Collections.Dictionary<String, String[]>(){
			{"Pot haute qualité",null},
			{"Capteur d'humidité visuel",new string[]{"Capteur d'humidité visuel","Capteur d'humidité visuel Trop","Capteur d'humidité visuel Pas Assez"}},
			{"Réduction de la consommation de la clime",null},
			{"Amélioration de la fiabilité de la clime",null},
			{"Capteur de température visuel",new string[]{"Capteur de température visuel","Capteur de température visuel Trop","Capteur de température visuel Pas Assez"}},
			{"Réduction de la consommation du générateur",null},
			{"Panneaux solaires et batterie",null},
			{"LED basse consommation",null},
			{"Hygromètre",null},
			{"Thermomètre",null},
			{"Luxmètre",null}
			};
		DescriptionsArray = new Godot.Collections.Array<String>(){
			"Le [Pot haute qualité] permet de diminuer les fuites !\n\nVous aurez moins besoin d'arroser vos plantes car l'eau sera répartie uniformément à l'intérieur du Pot sans craindre qu'elle ne s'en échappe.",
			"Le [Capteur d'humidité visuel] captera tout aussi bien l'humidité qu'un hygromètre classique.\n\nToutefois, il aura un visage mécontent bleu s'il y a trop d'eau sur la plante, et orange s'il n'y en a pas assez !",
			"L'amélioration [Réduction de la consommation de la clime] rendra la clime réversible moins consommatrice en électricité.",
			"L'[Amélioration de la fiabilité de la clime] va vous faciliter la vie !\n\nVotre clime n'oscillera plus autour de la valeur à laquelle vous l'avez programmée, elle fonctionnera désormais précisément sur cette valeur.",
			"Le [Capteur de température visuel] captera tout aussi bien la température qu'un thermomètre classique.\n\nToutefois, il aura un visage mécontent bleu si la plante a trop froid, et orange si la plante a trop chaud !",
			"L'amélioration [Réduction de la consommation du générateur] rendra le générateur moins consommateur en électricité.",
			"Fini l'énergie fossile dans votre serre ! Les [Panneaux solaires et batterie] vous permettent de supprimer totalement les frais quotidiens liés au fonctionnement du générateur.\n\nLa clime, les lampes, le thermostat, tout ce qui consomme de l'électricité ne vous coûte plus de l'argent quotidiennement.",
			"Les [LED basse consommation] permettent de remplacer vos lampes actuelles qui consomment beaucoup d'électricité.",
			"L'hygromètre est l'un des capteurs que vous possédez de base dans la serre. Il mesure la quantité d'humidité, en pourcentage, qu'il y a sur la plante.",
			"Le thermomètre est l'un des capteurs que vous possédez de base dans la serre. Il mesure la température, en degré Celcius, qu'il y a dans la serre.",
			"Le luxmètre est l'un des capteurs que vous possédez de base dans la serre. Il mesure la quantité de lumière, en lux, qu'il y a sur la plante."
			};
		PageNumber = 0;
		LoadPage();
	}

	//METHODES _____________________________________________________________________________________________________________

	private void LoadPage()
	{
		//Charge la page en fonction du numéro de page

		string Materiel = ((Godot.Collections.Array<String>)MaterialArray.Keys)[PageNumber];
		GetNode<Label>("Titre").SetText(Materiel);
		GetNode<Label>("Carte1/Description").SetText(DescriptionsArray[PageNumber]);

		for (int j = 0; j < 4; j++)
		{ //On cache les cases inutilisées
			GetNode<TextureRect>("Carte0/HBoxContainer/Case" + j.ToString()).Hide();
		}
		if (MaterialArray[Materiel].IsEmpty())
		{
			TextureRect Item = GetNode<TextureRect>("Carte0/HBoxContainer/Case0");
			Item.GetChild<TextureRect>(0).Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + Materiel + ".png");
			Item.Show();
		}
		else
		{
			for (int i = 0; i < MaterialArray[Materiel].Length; i++)
			{ //On charge les cases
				TextureRect Item = GetNode<TextureRect>("Carte0/HBoxContainer/Case" + i.ToString());
				Item.GetChild<TextureRect>(0).Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + MaterialArray[Materiel][i] + ".png");
				Item.Show();
			}
		}
		GetNode<TextureRect>("Objet").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesObjets/" + Materiel + ".png");
	}

	//SIGNAUX _____________________________________________________________________________________________________________


	public void OnNextPagePressed()
	{
		//Fonction appelée lorsque le bouton page suivante est cliqué
		if (PageNumber + 1 < MaterialArray.Count)
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


}
