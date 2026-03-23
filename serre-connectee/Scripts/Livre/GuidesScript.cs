using Godot;
using System;

public partial class GuidesScript : Panel
{

//ATTRIBUTS ____________________________________________________________________________________________________________

	private int PageNumber; //Numero de la page actuelle
	private Godot.Collections.Dictionary<String,String[]> GuidesArray; //Liste des guides du jeu au format "Titre":["Description","Image.png"]

//READY ________________________________________________________________________________________________________________

	public override void _Ready()
	{
		PopupManager.ShowPopup("Voici votre guide. Vous y retrouverez un tas d'informations sur les plantes que vous pouvez faire pousser, sur les objets, et bien plus encore !");
		GuidesArray = new Godot.Collections.Dictionary<String,String[]>{
			{"Les actions des pots",new string[]{"Dans l'interface des pots, la partie haute du panneau droit est dédiée aux actions d'entretien de chaque plante.\n\nJuste en dessous, les graines et les capteurs que vous possédez dans votre inventaire apparaîtront automatiquement. Vous pouvez ensuite les glisser jusqu'à la case de terre désirée.\n\nSi ces lignes sont vides, c'est qu'il est temps d'aller dans la boutique pour acheter des graines et des capteurs !","PotActions.png"}},
			{"Les capteurs des pots",new string[]{"Dans l'interface des pots, la partie basse du panneau droit est dédiée aux capteurs.\n\nLes capteurs que vous possédez dans votre inventaire apparaîtront automatiquement. Vous pouvez ensuite les glisser jusqu'à la case de terre désirée.\n\nVous pouvez choisir d'afficher ou de cacher les capteurs installés en cliquant sur le bouton 'Voir Plante' ou 'Voir Capteur' situé tout en bas\n\nSi ces lignes sont vides, c'est qu'il est temps d'aller dans la boutique pour acheter des graines et des capteurs !","PotCapteurs.png"}},
			{"Les informations des pots",new string[]{"Dans l'interface des pots, le panneau gauche est dédié aux informations de la case que vous avez sélectionnée.\n\nVous y trouvez le nom de la graine plantée, des informations sur l'état d'entretien de la case de terre, et des données de capteurs d'humidité, de température et de luminosité.\n\n S'il manque l'un de ces capteurs, la donnée associée sera un rond barré. Vous pouvez acheter des capteurs dans la boutique.","PotSelection.png"}}
		};
		PageNumber = 0;
		LoadPage();
	}


//METHODES _____________________________________________________________________________________________________________

	private void LoadPage(){
		//Charge la page en fonction du numéro de page
		string Guide = ((Godot.Collections.Array<String>)GuidesArray.Keys)[PageNumber];
		GetNode<Label>("Titre").SetText(Guide);
		GetNode<Label>("Carte1/Description").SetText(GuidesArray[Guide][0]);
		GetNode<TextureRect>("ContainerIllustration/Illustration").Texture = (Texture2D)ResourceLoader.Load("../Assets/Images/ImagesGuides/"+GuidesArray[Guide][1]);
	}

//SIGNAUX _____________________________________________________________________________________________________________


	public void OnNextPagePressed(){
		//Fonction appelée lorsque le bouton page suivante est cliqué
		if(PageNumber+1<GuidesArray.Count){
			PageNumber++;
			LoadPage();
		}
	}
	public void OnPreviousPagePressed(){
		//Fonction appelée lorsque le bouton page précédente est cliqué
		if(PageNumber>0){
			PageNumber--;
			LoadPage();
		}
	}


}
