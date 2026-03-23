using Godot;
using System;
using System.Collections.Generic;
public partial class LargeCellsSlots : StaticBody2D
{
	string Item;
	List<string> SensorsArray;
	List<Node2D> SensorsNodesArray;
	Button SeeSensors;
	bool IsOnSeed = true;
	static ulong Counter = 18446744073709551613;
	Godot.Collections.Array<string> ListSensorsItems;

	//READY __________________________________________________________________________________________________


	/// <summary>
	/// Called when entering the SerreInterface scene. Initialises the values of the instance variables.
	/// </summary>
	public override void _Ready()
	{
		Item = "";
		SensorsArray = new List<string>();
		SensorsNodesArray = new List<Node2D>();
		SeeSensors = GetParent().GetParent().GetNode<Button>("ControlGeneral/Control/Panneau/VueInventaire/Button");
		ListSensorsItems = new Godot.Collections.Array<string>() { "Luxmètre", "Hygromètre", "Thermomètre" };
	}

	//PROCESS ________________________________________________________________________________________________

	/// <summary>
	/// Called every frame. Displays the sprites in the SensorsNodesArray list.
	/// </summary>
	public override void _Process(double delta)
	{
		DisplaySprites();
	}


	//SETTERS et GETTERS _____________________________________________________________________________________


	/// <summary>
	/// Sets the item of the SlotsGrandesCases to the given seed name, and refreshes the view in the VueInventaireScript.
	/// </summary>
	/// <param name="Seed">The name of the seed to plant.</param>
	public void SetItem(string Seed)
	{
		GetNode<CellControlScript>("../../ControlCarre/ControlCase" + this.Name.ToString().Substring(11, 1)).SetPlant(Seed);
		InventoryViewScript Node = GetNode<InventoryViewScript>("../../ControlGeneral/Control/Panneau/VueInventaire");
		int CellIndex = Node.FindCell(Seed);
		Node.LoadView();
		Node.RefreshCell(CellIndex);
	}

	/// <summary>
	/// Sets the status of the SlotsGrandesCases to true if the parameter is true, or false otherwise.
	/// If the parameter is true, the InventoryViewScriptis refreshed.
	/// </summary>
	/// <param name="ShouldChangeInventory">Whether the InventoryViewScriptshould be refreshed or not.</param>
	public void SetIsOnSeed(bool ShouldChangeInventory)
	{
		IsOnSeed = ShouldChangeInventory;
	}


	/// <summary>
	/// Retrieves the name of the plant associated with this SlotsGrandesCases instance.
	/// </summary>
	/// <returns>The name of the plant if it exists; otherwise, an empty string.</returns>
	public string GetItem()
	{
		Plant CurrentPlant = GetNode<CellControlScript>("../../ControlCarre/ControlCase" + this.Name.ToString().Substring(11, 1)).GetPlant();
		if (CurrentPlant is null)
		{
			return "";
		}
		return CurrentPlant.GetName();
	}

	/// <summary>
	/// Retrieves the size of the SensorsArray list.
	/// </summary>
	/// <returns>The size of the SensorsArray list.</returns>
	public int GetSensorsArraySize()
	{
		return SensorsArray.Count;
	}

	/// <summary>
	/// Retrieves a list of the names of sensors associated with this SlotsGrandesCases instance.
	/// </summary>
	/// <returns>A list of strings containing the names of the sensors.</returns>
	public List<string> GetSensorsArray()
	{
		return SensorsArray;
	}


	/// <summary>
	/// Retrieves a list of Node2D that are the instantiated scenes of the sensors associated with this SlotsGrandesCases instance.
	/// </summary>
	/// <returns>A list of Node2D containing the instantiated scenes of the sensors.</returns>
	public List<Node2D> GetSensorsNodesArray()
	{
		List<Node2D> CurrentList;
		CurrentList = new List<Node2D>();
		PackedScene Scene = (PackedScene)GD.Load("res://Scenes/Interface/PlanDeTerre/DragAndDrop/Capteurs.tscn");
		for (int i = 0; i < SensorsArray.Count; i++)
		{

			CurrentList.Add((Node2D)Scene.Instantiate());
			CurrentList[i].Name = SensorsArray[i] + Counter;
			Counter--;
			Sprite2D Sprite = new Sprite2D();
			Sprite.Texture = CreateTexture(SensorsArray[i]);
			Sprite.Scale = new Vector2((float)0.14, (float)0.14);
			Sprite.Name = SensorsArray[i];
			CurrentList[i].AddChild(Sprite);
			CurrentList[i].AddToGroup("Capteurs");
			if (i == 0) { CurrentList[i].Position = this.Position + new Vector2(-40, -40); } else if (i == 1) { CurrentList[i].Position = this.Position + new Vector2(40, -40); } else if (i == 2) { CurrentList[i].Position = this.Position + new Vector2(-40, 40); } else if (i == 3) { CurrentList[i].Position = this.Position + new Vector2(40, 40); }
		}
		SensorsNodesArray = CurrentList;
		return SensorsNodesArray;
	}

	/// <summary>
	/// Creates and returns a texture for a given sensor name. If the sensor is in the ListSensorsItems,
	/// it loads the corresponding texture directly. For visual humidity or temperature sensors, it checks
	/// the current conditions against the plant's optimal range and loads a specific texture indicating
	/// "trop" or "pas assez" if the conditions are outside the range. Otherwise, it loads the default
	/// sensor texture.
	/// </summary>
	/// <param name="SensorName">The name of the sensor for which the texture is created.</param>
	/// <returns>A Texture2D object representing the sensor's visual representation.</returns>
	private Texture2D CreateTexture(string SensorName)
	{
		Texture2D SensorTexture = null;
		if (ListSensorsItems.Contains(SensorName))
		{
			SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + ".png");
		}
		else
		{
			CellControlScript Cell = GetNode<CellControlScript>("../../ControlCarre/ControlCase" + this.Name.ToString().Substring(11, 1));
			Plant CurrentPlant = Cell.GetPlant();
			if (CurrentPlant is not null)
			{
				if (SensorName == "Capteur d'humidité visuel")
				{
					if (Cell.GetHumidity() > CurrentPlant.GetMaxHumidity())
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + " Trop.png");
					}
					else if (Cell.GetHumidity() < CurrentPlant.GetHumidityMin())
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + " Pas Assez.png");
					}
					else
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + ".png");
					}
				}
				if (SensorName == "Capteur de température visuel")
				{
					if (Global.GlassHouseTemperature > CurrentPlant.GetTemperatureMax())
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + " Trop.png");
					}
					else if (Global.GlassHouseTemperature < CurrentPlant.GetTemperatureMin())
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + " Pas Assez.png");
					}
					else
					{
						SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + ".png");
					}
				}
			}
			else
			{
				SensorTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + SensorName + ".png");
			}
		}
		return SensorTexture;
	}

	//METHODES ___________________________________________________________________________________________


	/// <summary>
	/// Adds an item to the array of sensors, and refreshes the view of the VueInventaireScript.
	/// </summary>
	/// <param name="i">The name of the item to add.</param>
	public void AddItem(string i)
	{
		SensorsArray.Add(i);
		InventoryViewScript Node = GetNode<InventoryViewScript>("../../ControlGeneral/Control/Panneau/VueInventaire");
		int CellIndex = Node.FindCell(i);
		Node.LoadView();
		Node.RefreshCell(CellIndex);
	}

	/// <summary>
	/// Adds an item to the array of sensors without refreshing the view of the VueInventaireScript.
	/// </summary>
	/// <param name="i">The name of the item to add.</param>
	public void AddItemWithoutRefreshingView(string i)
	{
		SensorsArray.Add(i);
	}

	/// <summary>
	/// Checks if an item is already in the list of sensors for this SlotsGrandesCases instance.
	/// This method takes into account the fact that a "Hygromètre" is equivalent to a "Capteur d'humidité visuel",
	/// a "Thermomètre" is equivalent to a "Capteur de température visuel", and a "Luxmètre" is equivalent to a
	/// "Capteur de luminosité visuel".
	/// </summary>
	/// <param name="i">The name of the item to check.</param>
	/// <returns>True if the item is already in the list of sensors, false otherwise.</returns>
	public bool IsItemAlreadyThere(string i)
	{
		for (int j = 0; j < SensorsArray.Count; j++)
		{
			if (SensorsArray[j] == i)
			{
				return true;
			}
			if (i == "Hygromètre" || SensorsArray[j] == "Hygromètre")
			{
				if (i == "Capteur d'humidité visuel" || SensorsArray[j] == "Capteur d'humidité visuel")
				{
					return true;
				}
			}
			if (i == "Thermomètre" || SensorsArray[j] == "Thermomètre")
			{
				if (i == "Capteur de température visuel" || SensorsArray[j] == "Capteur de température visuel")
				{
					return true;
				}
			}
			if (i == "Luxmètre" || SensorsArray[j] == "Luxmètre")
			{
				if (i == "Capteur de luminosité visuel" || SensorsArray[j] == "Capteur de luminosité visuel")
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Adds each sensor node from the SensorsNodesArray to the parent node in the scene,
	/// effectively displaying them within the scene hierarchy.
	/// </summary>
	public void DisplaySensors()
	{
		for (int i = 0; i < SensorsNodesArray.Count; i++)
		{
			GetParent().AddChild(SensorsNodesArray[i]);
		}
	}

	/// <summary>
	/// Updates the texture of each sprite in the SensorsNodesArray with the texture
	/// corresponding to their respective sensor name in the SensorsArray.
	/// </summary>
	public void DisplaySprites()
	{
		for (int i = 0; i < SensorsNodesArray.Count; i++)
		{
			SensorsNodesArray[i].GetChild<Sprite2D>(1).Texture = CreateTexture(SensorsArray[i]);
		}
	}

	/// <summary>
	/// Deletes all the sensor nodes in the SensorsNodesArray by freeing them from memory.
	/// </summary>
	public void DeleteSensorsNodes()
	{
		for (int i = 0; i < SensorsNodesArray.Count; i++)
		{
			SensorsNodesArray[i].QueueFree();
		}
	}

	/// <summary>
	/// Deletes an item from the array of sensors associated with this SlotsGrandesCases instance.
	/// </summary>
	/// <param name="i">The name of the item to delete.</param>
	public void Delete(string i)
	{
		SensorsArray.Remove(i);
	}

	/// <summary>
	/// Hides all the sensors associated with this SlotsGrandesCases instance
	/// by setting their visibility to false.
	/// </summary>
	public void HideSensors()
	{
		for (int k = 0; k < SensorsNodesArray.Count; k++)
		{
			SensorsNodesArray[k].SetVisible(false);
		}
	}

	//SIGNAUX ___________________________________________________________________________________________


	/// <summary>
	/// Called when the button associated with this SlotsGrandesCases instance in the inventory panel is pressed.
	/// If the button is currently showing the sensors, it hides them and changes the text of the button to "Voir Capteurs".
	/// Otherwise, it displays the sensors and changes the text of the button to "Cacher Capteurs".
	/// </summary>
	public void OnButtonPressed()
	{
		if (IsOnSeed)
		{
			IsOnSeed = false;
			DeleteSensorsNodes();
			GetSensorsNodesArray();
			DisplaySensors();
			GetParent().GetParent().GetNode<Button>("ControlGeneral/Control/Panneau/VueInventaire/Button").SetText("Cacher Capteurs");
		}
		else
		{
			IsOnSeed = true;
			HideSensors();
			GetParent().GetParent().GetNode<Button>("ControlGeneral/Control/Panneau/VueInventaire/Button").SetText("Voir Capteurs");
		}
	}
}
