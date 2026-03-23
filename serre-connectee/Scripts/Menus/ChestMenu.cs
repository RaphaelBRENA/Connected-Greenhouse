using Godot;
using System;
using System.Collections.Generic;

public partial class ChestMenu : Control
{
	List<HFlowContainer> ListLines;
	VBoxContainer StockSheet;
	string CurrentDate;
	InventoryInterface Inventory;
	int LabelQuantity = 0;
	Dictionary<string, int> ChestDictionary;
	string Selected = "";
	int SelectedQuantity = 0;
	Dictionary<string, Godot.Collections.Array<string>> StockSheetDictionary;
	public static bool IsChestOpen;

	/// <summary>
	/// Called when the node enters the ChestMenu scene.
	/// Set the `IsChestOpen` flag to true and show the inventory.
	/// Show the date and the stock sheet.
	/// Load the data from the "Coffre" section of the global save interaction.
	/// </summary>
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		IsChestOpen = true;
		PopupManager.ShowPopup("Voici le coffre. Vous pouvez y stocker des plantes et des objets. Ne vous inquiétez pas, vous pouvez les récupérez à tout moment !");
		ChangeDay();
		Inventory = GetNode<InventoryInterface>("InventaireInterface");
		ChestDictionary = new Dictionary<string, int>();
		Inventory.Show();
		GetNode<AnimationPlayer>("InventaireInterface/AnimationPlayer").Play("blur");
		StockSheet = GetNode<VBoxContainer>("ScrollContainer/FicheStock");
		ListLines = new List<HFlowContainer>();
		StockSheetDictionary = new Dictionary<string, Godot.Collections.Array<string>>();
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Coffre"].SendData();
		if (Data != null)
		{
			foreach (var KeyValuePair in Data)
			{
				if (KeyValuePair.Key != "Tab" && KeyValuePair.Key != "DictionnaireCoffre")
				{
					CreateLine(KeyValuePair.Key);
					for (int i = 0; i < KeyValuePair.Value.Count; i += 2)
					{
						DisplayLines();
						AddToLine(KeyValuePair.Value[i], int.Parse(KeyValuePair.Value[i + 1]), KeyValuePair.Key, true);
					}
				}
			}
		}
		DisplayLines();
	}

	/// <summary>
	/// Called when the node receives an input event.
	/// If the escape key or the interact action is pressed, save the data and go back to the gameplay scene.
	/// Set the `IsChestOpen` flag to false and set the mouse mode to captured.
	/// </summary>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Pressed && (EventKey.IsActionPressed("Interact") || EventKey.Keycode == Key.Escape))
			{
				ExportData();
				IsChestOpen = false;
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn"); 
			}
		}
	}

	/// <summary>
	/// Update the current date text with the data from the "Calendrier" section of the global save interaction.
	/// If the data is not found, set the date to "Jour 1 - Année 1".
	/// </summary>
	public void ChangeDay()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> CalendarData = Global.SaveInteract["Calendrier"].SendData();
		if (CalendarData != null)
		{
			CurrentDate = "Jour " + CalendarData["Date"][0] + " - Année " + CalendarData["Date"][2];
		}
		else
		{
			CurrentDate = "Jour 1 - Année 1";
		}
	}

	/// <summary>
	/// Create a new line in the stock sheet with the given date as its title.
	/// If the line already exists, do nothing.
	/// </summary>
	/// <param name="Date">The date of the line to be created.</param>
	public void CreateLine(string Date)
	{
		if (StockSheet.GetNodeOrNull("Line" + Date) == null)
		{
			HFlowContainer NewContainer = new HFlowContainer();
			NewContainer.CustomMinimumSize = new Vector2(564, 0);
			NewContainer.Name = "Line" + Date;
			Label Label = new Label();
			Label.SetText(Date);
			NewContainer.AddChild(Label);
			ListLines.Add(NewContainer);
			StockSheetDictionary[Date] = new Godot.Collections.Array<string>();
		}
	}

	/// <summary>
	/// Display all the lines in the stock sheet by adding them as children of the stock sheet if they are not already there.
	/// </summary>
	public void DisplayLines()
	{
		foreach (HFlowContainer Container in ListLines)
		{
			if (StockSheet.GetNodeOrNull(Container.Name.ToString()) == null)
			{
				StockSheet.AddChild(Container);
			}
		}
	}

	/// <summary>
	/// Add a quantity of an item to the stock sheet.
	/// If the item is already in the stock sheet, add the quantity to the existing quantity.
	/// If the item is not in the stock sheet, add the item to the stock sheet with the given quantity.
	/// If the quantity is 0, do nothing.
	/// </summary>
	/// <param name="Item">The item to be added.</param>
	/// <param name="Quantity">The quantity of the item to be added.</param>
	/// <param name="Date">The date of the stock sheet line to be added to.</param>
	/// <param name="ShouldChangeInventory">Whether to change the inventory quantity of the item.</param>
	public void AddToLine(string Item, int Quantity, string Date, bool ShouldChangeInventory)
	{
		if (Quantity == 0) return;
		bool IsThere = false;
		bool HasFound = false;
		for (int i = 0; i < StockSheetDictionary[Date].Count; i += 2)
		{
			if (StockSheetDictionary[Date][i] == Item)
			{
				IsThere = true;
				StockSheetDictionary[Date][i + 1] = (int.Parse(StockSheetDictionary[Date][i + 1]) + Quantity).ToString();
				if (int.Parse(StockSheetDictionary[Date][i + 1]) == 0)
				{
					StockSheetDictionary[Date].RemoveAt(i + 1);
					StockSheetDictionary[Date].RemoveAt(i);
				}
			}
		}
		if (!IsThere)
		{
			StockSheetDictionary[Date].Add(Item);
			StockSheetDictionary[Date].Add(Quantity.ToString());
			Label Label = new Label();
			Label.Name = Item + Date;
			Label.SetText(" | " + Item + " : ");
			Label label2 = new Label();
			label2.Name = Item + Quantity + Date;
			label2.SetText(Quantity.ToString());
			StockSheet.GetNode<HFlowContainer>("Line" + Date).AddChild(Label);
			StockSheet.GetNode<HFlowContainer>("Line" + Date).AddChild(label2);
			HasFound = false;
			foreach (Label ChildLabel in StockSheet.GetNode<HFlowContainer>("Line" + Date).GetChildren())
			{
				if (ChildLabel.Name.ToString().Contains(Item + Date))
				{
					HasFound = true;
				}
				else if (HasFound == true)
				{
					HasFound = false;
					if (int.Parse(ChildLabel.Text) > 0)
					{
						StockSheet.GetNode<Label>("Line" + Date + "/" + Item + Date).Text = " | " + Item + " : +";

					}
					else
					{
						StockSheet.GetNode<Label>("Line" + Date + "/" + Item + Date).Text = " | " + Item + " : ";

					}
				}
			}
		}
		LabelQuantity = 0;
		if (!ShouldChangeInventory)
		{
			if (Global.Inventory.ContainsProduct(Item))
			{
				Global.Inventory.ModifyProductQuantity(Item, -Quantity);
			}
			else
			{
				Global.Inventory.AddProduct(Item, -Quantity);
			}
		}
		if (ChestDictionary.ContainsKey(Item))
		{
			ChestDictionary[Item] += Quantity;
		}
		else
		{
			ChestDictionary[Item] = Quantity;
		}
		if (Quantity > 0)
		{
			AddATab(Item, Quantity);
		}
		if (!StockSheetDictionary[Date].Contains(Item))
		{
			HasFound = false;
			string PreviousQuantity = "";
			foreach (Label ChildLabel in StockSheet.GetNode<HFlowContainer>("Line" + Date).GetChildren())
			{
				if (ChildLabel.Name.ToString().Contains(Item + Date))
				{
					HasFound = true;
				}
				else if (HasFound == true)
				{
					HasFound = false;
					PreviousQuantity = ChildLabel.Text;
				}
			}
			StockSheet.GetNode<Label>("Line" + Date + "/" + Item + Date).QueueFree();

			StockSheet.GetNode<Label>("Line" + Date + "/" + Item + PreviousQuantity + Date).QueueFree();
		}
		if (IsThere)
		{
			HasFound = false;
			foreach (Label ChildLabel in StockSheet.GetNode<HFlowContainer>("Line" + Date).GetChildren())
			{
				if (ChildLabel.Name.ToString().Contains(Item + Date))
				{
					HasFound = true;
				}
				else if (HasFound == true)
				{
					HasFound = false;
					ChildLabel.Text = (int.Parse(ChildLabel.Text) + Quantity).ToString();
					ChildLabel.Name = Item + ChildLabel.Text + Date;
					if (int.Parse(ChildLabel.Text) > 0)
					{
						StockSheet.GetNode<Label>("Line" + Date + "/" + Item + Date).Text = " | " + Item + " : +";
					}
					else
					{
						StockSheet.GetNode<Label>("Line" + Date + "/" + Item + Date).Text = " | " + Item + " : ";
					}
				}
			}
		}
		Inventory.loadInventory();
		Inventory.Refresh();
	}

	/// <summary>
	/// Called when the user presses the "Ajouter" button in the ChestMenu scene.
	/// If the SelectedQuantity is greater than 0, create a new line in the stock sheet with the current date as its title if it doesn't already exist,
	/// display all the lines in the stock sheet, and add the selected item to the line with the given quantity.
	/// </summary>
	public void OnAddPressed()
	{
		if (SelectedQuantity > 0)
		{
			CreateLine(CurrentDate);
			DisplayLines();
			AddToLine(Selected, LabelQuantity, CurrentDate, false);
		}
	}

	/// <summary>
	/// Increments the label quantity displayed in the ChestMenu scene if the quantity is less than the selected quantity.
	/// Resets the label quantity to zero if it exceeds the selected quantity.
	/// </summary>
	public void OnPlusPressed()
	{
		if (Selected != "" && LabelQuantity < SelectedQuantity)
		{
			LabelQuantity++;
			GetNode<Label>("LabelQuantite").SetText(LabelQuantity.ToString());
		}
		if (LabelQuantity > SelectedQuantity)
		{
			LabelQuantity = 0;
		}
	}

	/// <summary>
	/// Decrements the label quantity displayed in the ChestMenu scene if the quantity is greater than 0.
	/// Resets the label quantity to zero if it exceeds the selected quantity.
	/// </summary>
	public void OnMinusPressed()
	{
		if (LabelQuantity > SelectedQuantity)
		{
			LabelQuantity = 0;
		}
		if (LabelQuantity > 0)
		{
			LabelQuantity--;
			GetNode<Label>("LabelQuantite").SetText(LabelQuantity.ToString());
		}
	}

	/// <summary>
	/// Adds a new tab to the TabContainer with the given name and quantity.
	/// If the tab already exists, increments the maximum value of the slider in the existing tab.
	/// </summary>
	/// <param name="Item">The name of the tab to add.</param>
	/// <param name="Quantity">The maximum value of the slider in the tab.</param>
	public void AddATab(string Item, int Quantity)
	{
		if (!TabAlreadyExists(Item))
		{
			VBoxContainer VboxContainer = new VBoxContainer();
			VboxContainer.Name = Item;
			HBoxContainer HboxContainer = new HBoxContainer();
			HBoxContainer HboxContainer2 = new HBoxContainer();
			TextureRect Texturerect = new TextureRect();
			string ImagePath;
			if (Item.Contains("Graines"))
			{
				ImagePath = Item.Substring(8) + "Graine.png";
			}
			else
			{
				ImagePath = Item + ".png";
			}
			Texture2D CurrentTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/" + ImagePath);
			Texturerect.Texture = CurrentTexture;
			HboxContainer.AddChild(Texturerect);
			Button Button = new Button();
			Button.SetText("Ajouter à l'inventaire");
			Button.Pressed += OnButtonPressed;
			HboxContainer.AddChild(Button);
			HSlider Slider = new HSlider();
			Slider.Name = "Slider" + Item;
			Slider.MinValue = 0;
			Slider.MaxValue = Quantity;
			Slider.CustomMinimumSize = new Vector2(400, 0);
			HboxContainer2.Name = "hbox2" + Item;
			Slider.ValueChanged += OnSliderValueChanged;
			HboxContainer2.AddChild(Slider);
			Label CurrentLabel = new Label();
			CurrentLabel.Name = "label" + Item;
			CurrentLabel.SetText((Slider.Value).ToString());
			HboxContainer2.AddChild(CurrentLabel);
			VboxContainer.AddChild(HboxContainer2);
			VboxContainer.AddChild(HboxContainer);
			GetNode<TabContainer>("TabContainer").AddChild(VboxContainer);
		}
		else
		{
			GetNode<Slider>("TabContainer/" + Item + "/hbox2" + Item + "/Slider" + Item).MaxValue = GetNode<Slider>("TabContainer/" + Item + "/hbox2" + Item + "/Slider" + Item).MaxValue + Quantity;
		}
	}

	/// <summary>
	/// Change the value of the label next to the slider to the slider's current value.
	/// </summary>
	/// <param name="value">The slider's current value.</param>
	public void OnSliderValueChanged(double value)
	{
		string TabTitle = GetNode<TabContainer>("TabContainer").GetTabTitle(GetNode<TabContainer>("TabContainer").GetCurrentTab());
		GetNode<Label>("TabContainer/" + TabTitle + "/hbox2" + TabTitle + "/label" + TabTitle).SetText(value.ToString());
	}

	/// <summary>
	/// Handles the button press event to transfer items from the chest to the inventory.
	/// Retrieves the current tab title and the slider value for the item quantity to transfer.
	/// Updates the stock sheet by creating a new line with the current date and displaying the updated lines.
	/// Adjusts the maximum value of the slider and updates the stock sheet line with the reduced item quantity.
	/// If the item quantity in the chest reaches zero, deletes the tab.
	/// Refreshes the inventory display.
	/// </summary>
	public void OnButtonPressed()
	{
		string TabTitle = GetNode<TabContainer>("TabContainer").GetTabTitle(GetNode<TabContainer>("TabContainer").GetCurrentTab());
		int ValueSlider = (int)(GetNode<HSlider>("TabContainer/" + TabTitle + "/hbox2" + TabTitle + "/Slider" + TabTitle).Value);
		double MaxValue = GetNode<HSlider>("TabContainer/" + TabTitle + "/hbox2" + TabTitle + "/Slider" + TabTitle).MaxValue;
		CreateLine(CurrentDate);
		DisplayLines();
		GetNode<HSlider>("TabContainer/" + TabTitle + "/hbox2" + TabTitle + "/Slider" + TabTitle).MaxValue = MaxValue - ValueSlider;
		AddToLine(TabTitle, -ValueSlider, CurrentDate, false);
		if (ChestDictionary[TabTitle] == 0)
		{
			DeleteTab(TabTitle);
		}
		Inventory.loadInventory();
		Inventory.Refresh();
	}

	/// <summary>
	/// Checks if a tab with the given title already exists in the TabContainer.
	/// </summary>
	/// <param name="Item">The title of the tab to look for.</param>
	/// <returns>True if the tab already exists, false otherwise.</returns>
	public bool TabAlreadyExists(string Item)
	{
		TabContainer TabContainer = GetNode<TabContainer>("TabContainer");
		for (int i = 0; i < TabContainer.GetTabCount(); i++)
		{
			if (TabContainer.GetTabTitle(i) == Item)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Deletes a tab with the given title from the TabContainer.
	/// If the tab does not exist, does nothing.
	/// </summary>
	/// <param name="Item">The title of the tab to delete.</param>
	public void DeleteTab(string Item)
	{
		if (TabAlreadyExists(Item))
		{
			TabContainer TabContainer = GetNode<TabContainer>("TabContainer");
			for (int i = 0; i < TabContainer.GetTabCount(); i++)
			{
				if (TabContainer.GetTabTitle(i) == Item)
				{
					Node TabContent = TabContainer.GetChild(i);
					TabContainer.RemoveChild(TabContent);
					TabContent.QueueFree();
				}
			}
		}
	}

	/// <summary>
	/// Exports the current state of the chest's content and stock sheet to the save system.
	/// This includes the titles and maximum values of sliders for each tab in the TabContainer,
	/// the items and their quantities in the chest dictionary, and the stock sheet dictionary.
	/// The data is stored in the "Coffre" section of the global save interaction, and the scene transitions
	/// back to the gameplay scene.
	/// </summary>
	public void ExportData()
	{
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
		Godot.Collections.Array<string> CurrentArray = new Godot.Collections.Array<string>();
		TabContainer TabContainer = GetNode<TabContainer>("TabContainer");
		for (int i = 0; i < TabContainer.GetTabCount(); i++)
		{
			CurrentArray.Add(TabContainer.GetTabTitle(i));
			CurrentArray.Add(GetNode<Slider>("TabContainer/" + TabContainer.GetTabTitle(i) + "/hbox2" + TabContainer.GetTabTitle(i) + "/Slider" + TabContainer.GetTabTitle(i)).MaxValue.ToString());
		}
		Data["Tab"] = CurrentArray;
		Godot.Collections.Array<string> Tableau2 = new Godot.Collections.Array<string>();
		foreach (var entry in ChestDictionary)
		{
			Tableau2.Add(entry.Key);
			Tableau2.Add(entry.Value.ToString());
		}
		Data["DictionnaireCoffre"] = Tableau2;

		foreach (var KeyValuePair in StockSheetDictionary)
		{
			Data[KeyValuePair.Key] = KeyValuePair.Value;
		}
		Global.SaveInteract["Coffre"].KeepData(Data);
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	/// <summary>
	/// Sets the currently selected item and its quantity in the ChestMenu scene.
	/// Resets the label quantity to zero and updates the label text to reflect this.
	/// </summary>
	/// <param name="Item">The name of the item to be selected.</param>
	/// <param name="Quantity">The quantity of the item to be selected.</param>
	public void SetSelected(string Item, int Quantity)
	{
		Selected = Item;
		SelectedQuantity = Quantity;
		LabelQuantity = 0;
		GetNode<Label>("LabelQuantite").SetText("0");
	}
}
