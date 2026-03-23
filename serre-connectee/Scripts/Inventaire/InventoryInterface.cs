using Godot;
using System;

public partial class InventoryInterface : Control
{
	// ATTRIBUTS _______________________________________________________________________________________

	private bool IsOpen = false;
	private InventoryCell SelectedCell = null;
	private Hand CurrentHand;
	private bool IsGrabbing = false;

	// READY ___________________________________________________________________________________________

	public override void _Ready()
    {
		GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
		this.Hide();
		CurrentHand = GetNode<Hand>("Hand");
		loadInventory();
		if(GetParent().Name == "MenuCoffre")
			IsOpen = true;
    }

	// INPUT ___________________________________________________________________________________________

	public override void _Input(InputEvent @event)
    {
		if(!ChestMenu.IsChestOpen){
			if (@event is InputEventKey EventKey) {
				if (EventKey.Pressed && EventKey.IsActionPressed("Inventaire") && GetTree().Paused == false)
					Start();
				else if (EventKey.Pressed && EventKey.IsActionPressed("Inventaire") && GetTree().Paused == true && !IsGrabbing) 
					Leave();
			} else if (@event is InputEventMouseButton eventMouse) {
				if (eventMouse.IsPressed() && eventMouse.ButtonIndex == MouseButton.Right && IsOpen)
					Grab();
				else if (!eventMouse.IsPressed() && eventMouse.ButtonIndex == MouseButton.Right && IsOpen)
					Released();
			}
		}
    }

	// METHODES ___________________________________________________________________________________________

	public void Leave()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		this.Hide();
		GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
		IsOpen = false;
	}

	public void Start()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;
		this.Show();
		GetNode<AnimationPlayer>("AnimationPlayer").Play("blur");
		IsOpen = true;
		PopupManager.ShowPopup("C'est dans votre inventaire que sont stockés les plantes et capteurs que vous possédez. Vous pouvez l'organiser comme vous le souhaitez !");
	}

	public void Grab()
	{
		Godot.Collections.Array[,] ArrayInventaire = Global.Inventory.GetArray();
		SelectedCell = null;
		if (IsOpen)
		{
			if (CurrentHand.Hover) {
				for(int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
					for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++)
						if (GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetHover() &&
						GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetItem() != "") {
							SelectedCell = GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString());
							ArrayInventaire[i, j][0] = "";
							ArrayInventaire[i, j][1] = 0;
							IsGrabbing = true;
						}
				if (SelectedCell != null) {
					if(SelectedCell.GetItem().Length > 8 && SelectedCell.GetItem().Substring(0, 7) == "Graines"){
						GetNode<Hand>("Hand").CurrentTexture.Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + (SelectedCell.GetItem().Substring(8, SelectedCell.GetItem().Length - 8)) + "Graine.png");
						GetNode<Hand>("Hand").Label.Text 
						= SelectedCell.GetQuantity().ToString();
						CurrentHand.Item = SelectedCell.GetItem();
						CurrentHand.Quantity = SelectedCell.GetQuantity();
					} else {
						GetNode<Hand>("Hand").CurrentTexture.Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + SelectedCell.GetItem() + ".png");
						GetNode<Hand>("Hand").Label.Text 
						= SelectedCell.GetQuantity().ToString();
						CurrentHand.Item = SelectedCell.GetItem();
						CurrentHand.Quantity = SelectedCell.GetQuantity();
					}
					SelectedCell.SetItem("", 0);
					Global.Inventory.SetPositionArray(ArrayInventaire);
				}
			}
		}
		Refresh();
	}

	public void Released()
	{
		Godot.Collections.Array[,] ArrayInventaire = Global.Inventory.GetArray();
		InventoryCell NewCell = null;
		if (IsOpen)
		{
			if (CurrentHand.Hover) {
				for(int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
					for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++)
						if (GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetHover() &&
						GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetItem() == "") {
							NewCell = GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString());
							ArrayInventaire[i, j][0] = CurrentHand.Item;
							ArrayInventaire[i, j][1] = CurrentHand.Quantity;
							IsGrabbing = false;
						}
				if (NewCell != null) {
					NewCell.SetItem(CurrentHand.Item, CurrentHand.Quantity);
					CurrentHand.CurrentTexture.Texture = null;
					CurrentHand.Label.Text = "";
				} else {
					SelectedCell.SetItem(CurrentHand.Item, CurrentHand.Quantity);
					for(int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
						for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++)
							if (SelectedCell == GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString())) {
								ArrayInventaire[i, j][0] = CurrentHand.Item;
								ArrayInventaire[i, j][1] = CurrentHand.Quantity;
							}
					IsGrabbing = false;
					CurrentHand.CurrentTexture.Texture = null;
					CurrentHand.Label.Text = "";
				}
			} else {
				if (SelectedCell != null){
					SelectedCell.SetItem(CurrentHand.Item, CurrentHand.Quantity);
					for(int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
						for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++)
							if (SelectedCell == GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString())) {
								ArrayInventaire[i, j][0] = CurrentHand.Item;
								ArrayInventaire[i, j][1] = CurrentHand.Quantity;
							}
					IsGrabbing = false;
					CurrentHand.CurrentTexture.Texture = null;
					CurrentHand.Label.Text = "";
				}
			}
		}
		CurrentHand.Item = "";
		CurrentHand.Quantity = 0;
		Global.Inventory.SetPositionArray(ArrayInventaire);
		Refresh();
	}

	public void Refresh()
	{
		Godot.Collections.Array[,] ArrayInventaire = Global.Inventory.GetArray();
		for (int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
			for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++) {
				if (GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetItem() != ""){
					String Item = GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).GetItem();
					if(Item.Length > 8 && Item.Substring(0, 7) == "Graines"){
						GetNode<TextureRect>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/TextureRect").Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + (Item.Substring(8, Item.Length - 8)) + "Graine.png");
						GetNode<Label>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/Label").Text = ArrayInventaire[i, j][1].ToString();
					} else {
						GetNode<TextureRect>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/TextureRect").Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + Item + ".png");
						GetNode<Label>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/Label").Text = ArrayInventaire[i, j][1].ToString();
					}
				} else {
					GetNode<TextureRect>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/TextureRect").Texture = null;
					GetNode<Label>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/Label").Text = "";
				}
			}
	}

	public void loadInventory()
	{
		int Counter = 0;
		Global.Inventory.InitialiseArray();
		Godot.Collections.Array[,] ArrayInventaire = Global.Inventory.GetArray();
		foreach (String CurrentKey in Global.Inventory.GetInventoryKeys()) {
			for(int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++) {
				while (Counter <= 7) {
					String ArrayKey = ArrayInventaire[i, Counter][0].ToString();
					if (ArrayKey == "") {
						ArrayInventaire[i, Counter][0] = CurrentKey;
						ArrayInventaire[i, Counter][1] = Global.Inventory.GetProductQuantity(CurrentKey);
						break;
					} else 
						Counter++;
				}
				if (Counter <= 7)
					break;
				Counter = 0;
			}
		}
		for (int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++) {
			for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++) {
				GetNode<TextureRect>(
					"PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + (j).ToString() + "/TextureRect"
				).Texture = null;
				GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).SetItem("", 0);
				String ArrayKey = ArrayInventaire[i, j][0].ToString();
				if (ArrayKey != "") {
					if(ArrayKey.Length > 8 && ArrayKey.Substring(0, 7) == "Graines"){
						GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).SetItem(
							ArrayInventaire[i, j][0].ToString(), (int) ArrayInventaire[i, j][1]
						);
						GetNode<TextureRect>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/TextureRect").Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + (ArrayKey.Substring(8, ArrayKey.Length - 8)) + "Graine.png");
						GetNode<Label>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/Label").Text = ArrayInventaire[i, j][1].ToString();
					} else{
						GetNode<InventoryCell>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString()).SetItem(
							ArrayInventaire[i, j][0].ToString(), (int) ArrayInventaire[i, j][1]
						);
						GetNode<TextureRect>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/TextureRect").Texture 
						= (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/" + ArrayKey + ".png");
						GetNode<Label>("PanelInventaire/VBoxContainer/Ligne" + (i+1).ToString() + "/Case" + j.ToString() + "/Label").Text = ArrayInventaire[i, j][1].ToString();
					}
				}
			}
		}
		Global.Inventory.SetPositionArray(ArrayInventaire);
	}

	public bool GetIsOpen()
	{
		return IsOpen;
	}

	public InventoryCell GetCaseSelected()
	{
		return SelectedCell;
	}
}
