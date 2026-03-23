using Godot;
using System;

public partial class InventoryScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________

	private double PlayerMoney; //Argent possédé par le joueur
	private Godot.Collections.Dictionary<string, int> Inventory; //Produits:quantités possédés par le joueur
	private Godot.Collections.Array[,] InventoryArray; //Positions des objets dans l'inventaire
	public const int MAXINVENTORYSIZE = 24;  //Nombre maximum d'objets différents que peut stoquer l'inventaire
	public const int INVENTORYLINESNUMBER = 3; //Nombre de lignes que possède l'affichage de l'inventaire


	//READY ___________________________________________________________________________________________

	public override void _Ready()
	{
		if (Inventory is null)
		{
			Initialisation();
			InitialiseArray();
		}
	}

	public void Initialisation()
	{
		Inventory = new Godot.Collections.Dictionary<string, int>();
		AddProduct("Graines Haricot", 6);
		AddProduct("Graines Basilic", 6);
		AddProduct("Hygromètre", 3);
		AddProduct("Luxmètre", 3);
		AddProduct("Thermomètre", 3);
		InventoryArray = new Godot.Collections.Array[InventoryScript.INVENTORYLINESNUMBER, (InventoryScript.INVENTORYLINESNUMBER/InventoryScript.MAXINVENTORYSIZE)];
		PlayerMoney = 0;
		ModifyMoney(54); // ICI : Modifier par la valeur de départ de l'argent du joueur dans le jeu
	}

	//METHODES ________________________________________________________________________________________

	public bool ModifyMoney(double Value)
	{
		//Augmente ou diminue l'argent possédé par le joueur
		//Renvoie false s'il tomberait en négatif, et true sinon.
		bool Result = false;
		if ((PlayerMoney + Value) >= 0)
		{
			PlayerMoney += Value;
			Result = true;
		}
		return Result;
	}

	public Godot.Collections.Dictionary<string, int> GetInventory() {
		return Inventory;
	}

	public void SetInventory(Godot.Collections.Dictionary<string, int> NewInventory) {
		Inventory = NewInventory;
	}


	public Godot.Collections.Array[,] GetArray()
	{
		return InventoryArray;
	}

	public void SetPositionArray(Godot.Collections.Array[,] NewArray)
	{
		InventoryArray = NewArray;
	}

	public double GetPlayerMoney()
	{
		//Renvoie l'argent possédé par le joueur
		return PlayerMoney;
	}


	public int GetProductQuantity(string Product)
	{
		//Renvoie la quantité du produit que le joueur possède en inventaire
		if (Inventory.ContainsKey(Product))
		{
			return Inventory[Product];
		}
		return 0;
	}

	public Godot.Collections.Array<string> GetInventoryKeys()
	{
		if (Inventory is not null)
		{
			return (Godot.Collections.Array<string>)Inventory.Keys;
		}
		else
		{
			Initialisation();
			return (Godot.Collections.Array<string>)Inventory.Keys;
		}
	}

	public bool ContainsProduct(string Product)
	{
		//Renvoie true si le produit existe dans l'inventaire et false sinon
		return Inventory.ContainsKey(Product);
	}

	public bool AddProduct(string Product, int Quantity)
	{
		//Renvoie true si le produit a bien été ajouté et false sinon
		bool Result = false;
		if (!ContainsProduct(Product) && Inventory.Count < MAXINVENTORYSIZE && Quantity > 0)
		{
			Inventory[Product] = Quantity;
			Result = true;
		}
		return Result;
	}

	public bool ModifyProductQuantity(string Product, int Quantity)
	{

		//Renvoie true si le produit a bien été augmenté/diminué de Quantity
		//Renvoie false sinon (il n'y a pas assez de ce Product, ou il n'existe pas dans l'inventaire)
		bool Result = false;
		if(ContainsProduct(Product)){
			if((Inventory[Product]+Quantity)>=0){
				Inventory[Product] += Quantity;
				Result = true;
			}
			if((Inventory[Product])==0){
				Inventory.Remove(Product);
				Result = true;
			}
		}
		return Result;
	}


	public bool IsFull()
	{
		//Renvoie true si l'inventaire ne peut plus accueillir un nouveau type d'objet, et false sinon
		return (Inventory.Count >= MAXINVENTORYSIZE);
	}

	public void InitialiseArray() 
	{
		InventoryArray = new Godot.Collections.Array[INVENTORYLINESNUMBER, (MAXINVENTORYSIZE/INVENTORYLINESNUMBER)];
		for (int i = 0; i < INVENTORYLINESNUMBER; i++) {
			for (int j = 0; j < (MAXINVENTORYSIZE/INVENTORYLINESNUMBER); j++) {
				InventoryArray[i, j] = new Godot.Collections.Array{"", 0};
			}
		}
	}
}
