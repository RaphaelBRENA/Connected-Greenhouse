using Godot;
using System;

public partial class InventoryCell : Button
{

	//ATTRIBUTS _______________________________________________________________________________________

	private Hand CurrentHand;
	private bool Hover = false;
	private string Item;
	private int Quantity;
	private Label DisplayNode;

	//READY ___________________________________________________________________________________________

	public override void _Ready()
	{
		CurrentHand = GetNode<Hand>("../../../../Hand");
		DisplayNode = GetNode<Label>("../../../LigneProduit/Produit");
		Item = "";
		Quantity = 0;
	}

	//PROCESS __________________________________________________________________________________________

	public override void _Process(double delta) {}

	//METHODES _________________________________________________________________________________________

	public void SetItem(string ProductName, int ProductQuantity)
	{
		//Attribue un produit p en quantité q sur cette case
		Item = ProductName;
		Quantity = ProductQuantity;
	}

	//GETTER _________________________________________________________________________________________

	public string GetItem()
	{
		return Item;
	}
	public int GetQuantity()
	{
		return Quantity;
	}

	//SIGNAUX __________________________________________________________________________________________

	public void OnCellPressed()
	{
		Node Parent = GetParent().GetParent().GetParent().GetParent().GetParent();
		ChestMenu CurrentChestMenu = new ChestMenu();
		if(Parent is ChestMenu){
			 CurrentChestMenu = (ChestMenu)Parent;
		}
		
		if (Item != ""){
			DisplayNode.SetText(Item + " : " + Quantity.ToString());
			if(Parent is ChestMenu){
				CurrentChestMenu.SetSelected(Item, Quantity);
			}
			
		}
		else{
			DisplayNode.SetText("");
			if(Parent is ChestMenu){
				CurrentChestMenu.SetSelected("", 0);
			}
		}
	}

	public bool GetHover()
	{
		return Hover;
	}

	public void OnMouseEntered()
	{
		CurrentHand.Hover = true;
		Hover = true;
	}

	public void OnMouseExited()
	{
		CurrentHand.Hover = false;
		Hover = false;
	}

}
