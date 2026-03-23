using Godot;
using System;

public partial class Global : Node
{
	
	//ATTRIBUTS _______________________________________________________________________________________________________________________

	//Scène 3D
	public static ContextComponent UiContext;
	public static Vector3 Coordinates = new Vector3(0, 0, 0);
	public static Vector3 RotationCamera = new Vector3(0, 0, 0);
	public static Vector3 RotationPlayer = new Vector3(0, 0, 0);
	

	//Informations de la Serre
	public static int GlassHouseLuminosity = -4000;
	public static int GlassHouseTemperature = -4000;
	public const int POTSNUMBER = 6; 

	
	
	//Gestionnaires de Sauvegardes
	public static bool FirstConnection;
	public static string PlayerStatus;

	public static Godot.Collections.Dictionary<string, SaveManager> SaveInteract;
	public static string CurrentSaveName = "";
	public static bool IsDragging = false;
	

	//Menus

	public static bool ComesFromOptionsMenu = false;
	public static bool ComesFromBook = false;
	public static OptionsMenu OptionsMenu = new OptionsMenu();

	//Inventaire

	public static InventoryScript Inventory = new InventoryScript();

	//READY _______________________________________________________________________________________________________________________

    public override void _Ready()
    {
		//Fonction d'initialisation du script Global
		FirstConnection = true;
		OptionsMenu.InitialiseInputMap();
		OptionsMenu.InitFullScreen();
		Inventory._Ready();
    }

	//METHODES _______________________________________________________________________________________________________________________

	
	
	
}

