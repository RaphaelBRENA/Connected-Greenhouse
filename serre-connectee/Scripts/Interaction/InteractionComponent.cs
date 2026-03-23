using Godot;
using System;
using System.Collections.Generic;
public partial class InteractionComponent : Node
{

	// ATTRIBUTS STORE _______________________________________________________________________________________

	[ExportGroupAttribute("Store Settings")]
	[Export] float Speed = 0.5f;
	[Export] float CloseTime = 2.0f;


		// ATTRIBUTS FENETRE _______________________________________________________________________________________

	[ExportGroupAttribute("Window Settings")]
	[Export] Vector3 Direction = new Vector3();
	[Export] Vector3 WindowSize;
	private Vector3 Position;
	private AnimatableBody3D Window;
	private enum WindowStatus {OPEN, CLOSED};
	private WindowStatus CurrentWindowStatus = WindowStatus.CLOSED;

	// ATTRIBUTS GLOBAUX _______________________________________________________________________________________

	[ExportGroupAttribute("Global Settings")]
	[Export] public String Context = "";
	[Export] public Boolean OverrideIcon;
	[Export] public Texture2D NewIcon = new Texture2D();
	private Node ParentNode = new Node();

	[ExportGroupAttribute("Tween Settings")]
	[Export] Tween.TransitionType Transition;
	[Export] Tween.EaseType Easing;

	private AnimatableBody3D Store;
	private Vector3 Scale = new Vector3();
	private enum StoreStatus {UP, DOWN};
	private StoreStatus CurrentStoreStatus = StoreStatus.UP;


	// READY _______________________________________________________________________________________

	public override void _Ready()
	{
		GetTree().Paused = false;
		ParentNode = GetParent();
		if (ParentNode.Name == "Bouton") {
			Store = GetNode<AnimatableBody3D>("../../Store");
			Scale = Store.Scale;
		} else if (ParentNode.Name == "FirstVolet") {
			Window = GetNode<AnimatableBody3D>("../../FirstVolet");
			Position = Window.Position;
		}
		ConnectParent();
	}

	// METHODES GLOBALES _______________________________________________________________________________________

	public void ConnectParent()
	{
		ParentNode.AddUserSignal("focused");
		ParentNode.AddUserSignal("interacted");

		// Transformer en switch case plus tard
		ParentNode.Connect("focused", new Callable(this, "InRange"));

		switch (ParentNode.Name) {
			case "Bouton":
				ParentNode.Connect("interacted", new Callable(this, "CheckStore"));
				break;
			case "FirstVolet":
				ParentNode.Connect("interacted", new Callable(this, "CheckWindow"));
				break;
			case "Boutique":
				ParentNode.Connect("interacted", new Callable(this, "ShopMenu"));
				break;
			case "Coffre":
				ParentNode.Connect("interacted", new Callable(this, "ChestMenu"));
				break;
			case "Livre":
				ParentNode.Connect("interacted", new Callable(this, "BookMenu"));
				break;
			case "Calendrier":
				ParentNode.Connect("interacted", new Callable(this, "CalendarMenu"));
				break;
			case "Thermostat":
				ParentNode.Connect("interacted", new Callable(this, "ThermostatMenu"));
				break;
			case "Ordinateur":
				ParentNode.Connect("interacted", new Callable(this, "ComputerMenu"));
				break;
			case "Engrenage":
				ParentNode.Connect("interacted", new Callable(this, "GearMenu"));
				break;
			default:
                if (ParentNode.Name.ToString().Find("Casquette") != (-1)) {
                    ParentNode.Connect("interacted", new Callable(this, "CheckCaps"));
                } else if (ParentNode.Name.ToString().Find("Plan de Terre") != (-1)) {
                    ParentNode.Connect("interacted", new Callable(this, "PotMenu"));
                } else {
                    throw new Exception("No method found for this parent node");
                }
                break;
		}
	}

	public void InRange()
	{
		Global.UiContext.UpdateContext(Context);
	}

	// METHODES PLAN DE TERRE _______________________________________________________________________________________

	public void PotMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone = PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT étudie-t'on la croissance des plantes ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT étudie-t'on la croissance des plantes ?", "C'est dans le département Biologie que l'on étudie la croissance des plantes. Les étudiants de ce département apprennent comment fonctionne le cycle de vie d'une plante. Allez chercher la casquette Biologie pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 1, 30, "La croissance des plantes relève du domaine des sciences de la vie.");

		}
		if (Global.PlayerStatus == "BIO") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("/Interface/PlanDeTerre/SerreInterface.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void ChestMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone =PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT étudie-t'on la gestion des stocks ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT étudie-t'on la gestion des stocks ?", "C'est dans le département GEA que l'on étudie la gestion des stocks. Les étudiants de ce département apprennent comment à établir des fiches de stock contenant toutes les entrées et sorties de produits. Allez chercher la casquette GEA pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 2, 30, "GEA signifie Gestion des Entreprises et des Administrations.");
		}
		if (Global.PlayerStatus == "GEA") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Jeu/MenuCoffre.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void BookMenu()
	{
		Global.ComesFromBook = false;
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/EncyclopedieInterface.tscn");
		Global.UiContext.Reset();
	}

	public void CalendarMenu()
	{
		Global.ComesFromBook = false;
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/MeteoInterface.tscn");
		Global.UiContext.Reset();
	}

	public void ThermostatMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone = PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT étudie-t'on l'économie énergie ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT étudie-t'on l'économie énergie ?", "C'est dans les départements MT2E et Informatique que l'on étudie l'énergie. Les étudiants de MT2E apprennent comment fonctionnent les différents types d'énergies, et d'Informatique combien consomment leurs infrastructures comme les serveurs. Allez chercher la casquette MT2E ou Informatique pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 4, 30, "MT2E signifie Métiers de la transition et de l'efficacité énergétique.");
		}

		if (Global.PlayerStatus == "MT2E" || Global.PlayerStatus == "INFO") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/ThermostatInterface.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void ShopMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone = PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT étudie-t'on l'économie et la gestion de marché ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT étudie-t'on l'économie et la gestion de marché ?", "C'est dans le département GEA que l'on étudie l'économie et la gestion de marché. Les étudiants de ce département apprennent à gérer les dépenses au sein d'une enteprise et apprennent aussi comment fonctionne le marché économique. Allez chercher la casquette GEA pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 2, 30, "GEA signifie Gestion des Entreprises et des Administrations.");
		}

		if (Global.PlayerStatus == "GEA") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/BoutiqueInterface.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void ComputerMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone = PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT utilise-t'on des ordinateurs pour faire de la programmation ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT utilise-t'on des ordinateurs pour faire de la programmation ?", "C'est dans le département Informatique que l'on utilise des ordinateurs pour faire de la programmation. Les étudiants de ce département apprennent à réaliser des sites web, des applications de bureau, ainsi que des applications mobiles. Allez chercher la casquette Informatique pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 3, 30, "Les ordinateurs sont souvent utilisés par les développeurs.");
		}

		if (Global.PlayerStatus == "INFO") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/OrdinateurInterface.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void GearMenu()
	{
		//PopupManager.ShowQuizPopup();
		bool QuestionIsDone = PopupManager.QuestionIsDone("A votre avis, dans quel département de l'IUT étudie-t'on comment diminuer le gaspillage d'énergie dans les entreprises ?");
		if(!QuestionIsDone)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			PopupManager.ShowQuizPopup("A votre avis, dans quel département de l'IUT étudie-t'on comment diminuer le gaspillage d'énergie dans les entreprises ?", "C'est dans le département MT2E que l'on étudie la consommation énergétique des organisations dans le but de trouver des solutions plus économiques et écologiques. Allez chercher la casquette MT2E pour rentrer dans l'interface.", new List<string>(){"Biologie", "GEA", "Informatique", "MT2E"}, 4, 30, "En informatique, l'économie d'énergie est rapidement mentionnée mais n'est pas un point très développé du programme.");
		}

		if (Global.PlayerStatus == "MT2E") {
			GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/AmeliorationInterface.tscn");
			Global.UiContext.Reset();
		} else {
			if(QuestionIsDone){
				PopupManager.ShowInfo("Mauvaise Casquette !");
			}
		}
	}

	public void CheckCaps()
	{
		ShowCaps();
		if (ParentNode.Name == "CasquetteInfo") {
			Global.PlayerStatus = "INFO";
			GetNode<StaticBody3D>("../../../Etagere/CasquetteInfo").Visible = false;
		} else if (ParentNode.Name == "CasquetteBio") {
			Global.PlayerStatus = "BIO";
			GetNode<StaticBody3D>("../../../Etagere/CasquetteBio").Visible = false;
		} else if (ParentNode.Name == "CasquetteGea") {
			Global.PlayerStatus = "GEA";
			GetNode<StaticBody3D>("../../../Etagere/CasquetteGea").Visible = false;
		} else  {
			Global.PlayerStatus = "MT2E";
			GetNode<StaticBody3D>("../../../Etagere/CasquetteMT2E").Visible = false;
		}
	}

	public void ShowCaps(){
		GetNode<StaticBody3D>("../../../Etagere/CasquetteInfo").Visible = true;
		GetNode<StaticBody3D>("../../../Etagere/CasquetteBio").Visible = true;
		GetNode<StaticBody3D>("../../../Etagere/CasquetteGea").Visible = true;
		GetNode<StaticBody3D>("../../../Etagere/CasquetteMT2E").Visible = true;

	}

	// METHODES STORE _______________________________________________________________________________________

	public void CheckStore() {
		switch (CurrentStoreStatus) {
			case StoreStatus.UP:
			DownStore();
				break;
			case StoreStatus.DOWN:
			UpStore();
				break;
		}
	}

	public void DownStore() {
		CurrentStoreStatus = StoreStatus.DOWN;
		var CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(Store, "Scale", new Vector3(Scale.X, Scale.Y + 0.155f, Scale.Z), Speed).SetTrans(Transition).SetEase(Easing);
	}

	public void UpStore() {
		CurrentStoreStatus = StoreStatus.UP;
		var CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(Store, "Scale", Scale, Speed).SetTrans(Transition).SetEase(Easing);
	}

	// METHODES FENETRE _______________________________________________________________________________________

	public void CheckWindow() {
		switch (CurrentWindowStatus) {
			case WindowStatus.CLOSED:
			OpenWindow();
				break;
			case WindowStatus.OPEN:
			CloseWindow();
				break;
		}
	}

	public void OpenWindow() {
		CurrentWindowStatus = WindowStatus.OPEN;
		var CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(Window, "position", Position - (Direction * WindowSize), Speed).SetTrans(Transition).SetEase(Easing);
	}

	public void CloseWindow() {
		CurrentWindowStatus = WindowStatus.CLOSED;
		var CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(Window, "position", Position, Speed).SetTrans(Transition).SetEase(Easing);
	}
}
