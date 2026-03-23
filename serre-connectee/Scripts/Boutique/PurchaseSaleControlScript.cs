using Godot;
using System;

public partial class PurchaseSaleControlScript : Control
{

	//ATTRIBUTS _______________________________________________________________________________________


	public static ShopCell SelectedCell;
	public static bool CellType; //false = sale cell, true = purchase cell
	private Godot.Collections.Dictionary<string,double> SeedsMarketPrice; 
	private Godot.Collections.Dictionary<string,double> MaterialMarketPrice; 
	private Godot.Collections.Dictionary<string,double> PlantsMarketPrice; 


	private Godot.Collections.Array<string> SeedsSale; //8 seeds currently in sale
	private Godot.Collections.Array<string> MaterialSale; //8 materials currently in sale

	private double MerchantTolerance; //Defines how much will the merchant accept to pay
	private const int SEEDSARTICLESNUMBER = 8; 
	private const int SEEDSNUMBER = 11; //Total number of seeds in the game

	private bool MustChangeArticles;
	private bool MustChangePrices;


	//READY ___________________________________________________________________________________________


	public override void _Ready(){
		Input.MouseMode = Input.MouseModeEnum.Visible;
		PopupManager.ShowPopup("Bienvenue dans la boutique. C'est ici que vous pouvez commercer avec le marchand. Mais attention, n'essayez pas de l'entourlouper !");
		
		MaterialMarketPrice=new Godot.Collections.Dictionary<string,double>{
			{"Pot haute qualité",10},
			{"Capteur d'humidité visuel",15},
			{"Luxmètre",5},
			{"Réduction de la consommation de la clime",20},
			{"Amélioration de la fiabilité de la clime",20},
			{"Capteur de température visuel",30},
			{"Réduction de la consommation du générateur",40},
			{"Panneaux solaires et batterie",50},
			{"LED basse consommation",50}
		};
		
		MaterialSale= new Godot.Collections.Array<string>();
		SeedsSale = new Godot.Collections.Array<string>();

		MustChangeArticles = false;
	    MustChangePrices = false;
		LoadShop();
		LoadPurchase();
		LoadSale();
		RefreshMoney();
		
		
	}

	//METHODES DE SAUVEGARDE ______________________________________________________________________________

	/// <summary>
	/// Assigns the markets and sales arrays
	/// </summary>
	public void LoadShop(){
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Boutique"].SendData();
		if(Data is not null){
			Godot.Collections.Array<string> CurrentArray = Data["SeedsMarketPrice"];
			SeedsMarketPrice = new Godot.Collections.Dictionary<string,double>();
			for(int i=0; i<CurrentArray.Count;i+=2){
				SeedsMarketPrice[CurrentArray[i]]=Convert.ToDouble(CurrentArray[i+1]);
			}
			CurrentArray = Data["PlantsMarketPrice"];
			PlantsMarketPrice = new Godot.Collections.Dictionary<string,double>();
			for(int i=0; i<CurrentArray.Count;i+=2){
				PlantsMarketPrice[CurrentArray[i]]=Convert.ToDouble(CurrentArray[i+1]);
			}

			SeedsSale = Data["SeedsSale"] ;
			MaterialSale = Data["MaterialSale"] ;
			MustChangeArticles = Convert.ToBoolean(Data["MustChangeArticles"][0]);
			MustChangePrices = Convert.ToBoolean(Data["MustChangePrices"][0]);

			if(MustChangePrices){
			ChangeMarketPrices();
			MustChangePrices = false;
			}

			if(MustChangeArticles){
				ChangeArticles();
				MustChangeArticles = false;
			}

		}
		else{
			ChangeMarketPrices();
			ChangeArticles();
		}

	}

	/// <summary>
	/// Saves the data into the SaveManager
	/// </summary>
	public void ExportData(){
		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>(); 
		
		Godot.Collections.Array<string> CurrentArray = new Godot.Collections.Array<string>();
		foreach(string CurrentKey in SeedsMarketPrice.Keys){
			CurrentArray.Add(CurrentKey);
			CurrentArray.Add(SeedsMarketPrice[CurrentKey].ToString());
		}
		Data["SeedsMarketPrice"] = CurrentArray;

		CurrentArray = new Godot.Collections.Array<string>();
		foreach(string CurrentKey in PlantsMarketPrice.Keys){
			CurrentArray.Add(CurrentKey);
			CurrentArray.Add(PlantsMarketPrice[CurrentKey].ToString());
		}
		Data["PlantsMarketPrice"] = CurrentArray;

		Data["SeedsSale"] = SeedsSale;
		Data["MaterialSale"] = MaterialSale;
		Data["MustChangeArticles"] = new Godot.Collections.Array<string>{MustChangeArticles.ToString()};
		Data["MustChangePrices"] = new Godot.Collections.Array<string>{MustChangePrices.ToString()};

		Global.SaveInteract["Boutique"].KeepData(Data);
	}

	//METHODES ____________________________________________________________________________________________

	/// <summary>
	/// Loads the cells of the shop
	/// </summary>
	public void LoadPurchase(){
		
		for(int i=0; i<SeedsSale.Count;i++){
			GetNode<ShopCell>("Achat/VBoxContainer/Ligne1/Case"+i.ToString()).SetProduit(SeedsSale[i],10);
		}
		
		for(int i=0; i<MaterialSale.Count;i++){
			
			GetNode<ShopCell>("Achat/VBoxContainer/Ligne2/Case"+i.ToString()).SetProduit(MaterialSale[i],1000);
		}
	}

	/// <summary>
	/// Loads the cells of the Inventory
	/// </summary>
	public void LoadSale(){
		
		Godot.Collections.Array<string> AllKeys = Global.Inventory.GetInventoryKeys();
		int LineSize = (InventoryScript.MAXINVENTORYSIZE)/(InventoryScript.INVENTORYLINESNUMBER); 
		for(int i=0; i<AllKeys.Count;i++){
			if(i<LineSize){
				GetNode<ShopCell>("Vente/VBoxContainer/Ligne1/Case"+i.ToString()).SetProduit(AllKeys[i],Global.Inventory.GetProductQuantity(AllKeys[i]));
			}
			if(i>=LineSize && i<(2*LineSize)){
				GetNode<ShopCell>("Vente/VBoxContainer/Ligne2/Case"+(i-LineSize).ToString()).SetProduit(AllKeys[i],Global.Inventory.GetProductQuantity(AllKeys[i]));
			}
			if(i>=(2*LineSize) && i<(3*LineSize)){
				GetNode<ShopCell>("Vente/VBoxContainer/Ligne3/Case"+(i-2*LineSize).ToString()).SetProduit(AllKeys[i],Global.Inventory.GetProductQuantity(AllKeys[i]));
			}
		}

	}

	/// <summary>
	/// Defines which was the last cell to be clicked by the player
	/// </summary>
	/// <param name="c">The cell.</param>
	/// <param name="type">The type of the cell : true if it's a purchase cell, false if it's a sale cell.</param>
	public void SetSelectedCell(ShopCell c, bool type){
		SelectedCell = c;
		CellType = type;
	}


	public double GetUnitPrice(string Product){
		if(MaterialMarketPrice.ContainsKey(Product)){
			return MaterialMarketPrice[Product];
		}
		if(SeedsMarketPrice.ContainsKey(Product)){
			return SeedsMarketPrice[Product];
		}
		return 0.0;
		
	}

	
	/// <summary>
	/// Returns a string of the value formatted as a money 
	/// Exemple : double 10.0 --> string 10.00€
	/// </summary>
	/// <param name="Value">The unformatted number.</param>
	public static string MoneyFormat(double Value){
		int IntegerPart = (int) Value;
		int DecimalPart = (int)((Value-IntegerPart)*100);
		string Result = "";
		if(DecimalPart==0){
			Result = IntegerPart + ".00€";
		}else if(DecimalPart%10==0){
			Result = IntegerPart+"."+(DecimalPart/10)+"0€";
		}else{
			Result = IntegerPart+"."+DecimalPart+"€";
		}
		return Result;
	}

	/// <summary>
	/// Returns false if the merchant accepts to pay or false if not. It depends on the total price, his tolerance,
	/// and if he doesn't usually sell this product (seeds, material).
	/// </summary>
	/// <param name="Product">The product to sell.</param>
	/// <param name="SaleQuantity">The quantity to sell.</param>
	/// <param name="TotalSalePrice">The total price of the sale.</param>
	public int AcceptPayment(string Product, int SaleQuantity, double TotalSalePrice){
		int Accept = 0;
		Godot.Collections.Dictionary<string,double> Market = null;
		if(PlantsMarketPrice.ContainsKey(Product)){
			Market = PlantsMarketPrice;
		}else{
			Accept = 2;
		}
		if(Market is not null){
			double UnitPrice = TotalSalePrice/SaleQuantity;
			double Tolerance=0;
			if(Market[Product] < MerchantTolerance){
				Tolerance = Market[Product]*2;
			}
			if(UnitPrice<=(Market[Product]+MerchantTolerance) || UnitPrice<=Tolerance){
				Accept = 1;
			}
		}
		return Accept;
	}
	
	/// <summary>
	/// Returns a random int between Min and Max
	/// </summary>
	public int Random(int Min, int Max)
	{
		RandomNumberGenerator r = new RandomNumberGenerator();
		return (int)r.RandiRange(Min,Max);
	}
	
	/// <summary>
	/// Changes the articles and the merchant's tolerance
	/// </summary>
	public void ChangeArticles(){
		MerchantTolerance = Random(10,30); 

		Godot.Collections.Array<String> SeedsArray = (Godot.Collections.Array<String>)SeedsMarketPrice.Keys;
		SeedsSale=new Godot.Collections.Array<string>(){"","","","","","","",""};
		String Seed;
		int Index = Random(0,SEEDSNUMBER-1);
		for(int i=0; i<SEEDSARTICLESNUMBER;i++){
			Seed = SeedsArray[Index];
			while(SeedsSale.Contains(Seed)){
				Index = Random(0,SEEDSNUMBER-1);
				Seed = SeedsArray[Index];
			}
			SeedsSale[i]=Seed;
		}

		Godot.Collections.Array<String> MaterialArray = (Godot.Collections.Array<String>)MaterialMarketPrice.Keys;
		MaterialSale = new Godot.Collections.Array<string>();
		int r = Random(0,MaterialArray.Count-1);
		for(int i=0; i<MaterialArray.Count;i++){
			if(i!=r){
				MaterialSale.Add(MaterialArray[i]);	
			}
		}
		
		
	}

	/// <summary>
	/// Changes the market price
	/// </summary>
	public void ChangeMarketPrices(){
		//Chargement des prix des graines et des plantes cette semaine
		
		SeedsMarketPrice = new Godot.Collections.Dictionary<string,double>{
			{"Graines Haricot",((double)Random(50,300))/100},
			{"Graines Ciboulette",((double)Random(50,200))/100},
			{"Graines Lavande",((double)Random(50,100))/100},
			{"Graines Lentille",((double)Random(150,400))/100},
			{"Graines Basilic",((double)Random(50,300))/100},
			{"Graines Origan",((double)Random(150,400))/100}, 
			{"Graines Menthe",((double)Random(250,500))/100}, 
			{"Graines Sauge",((double)Random(50,350))/100}, 
			{"Graines Persil",((double)Random(250,500))/100},
			{"Graines Aneth",((double)Random(100,250))/100},
			{"Graines Cosmos",((double)Random(450,700))/100} 
		};

		PlantsMarketPrice = new Godot.Collections.Dictionary<string,double>{
			{"Haricot",SeedsMarketPrice["Graines Haricot"]*2},
			{"Ciboulette",SeedsMarketPrice["Graines Ciboulette"]*1.5},
			{"Lavande",SeedsMarketPrice["Graines Lavande"]*1.2},
			{"Lentille", SeedsMarketPrice["Graines Lentille"]*2.5},
			{"Basilic",SeedsMarketPrice["Graines Basilic"]*2.2},
			{"Origan",SeedsMarketPrice["Graines Origan"]*2}, 
			{"Menthe",SeedsMarketPrice["Graines Menthe"]*2}, 
			{"Sauge",SeedsMarketPrice["Graines Sauge"]*1.4}, 
			{"Persil",SeedsMarketPrice["Graines Persil"]*2},
			{"Aneth",SeedsMarketPrice["Graines Aneth"]*1.8},
			{"Cosmos",SeedsMarketPrice["Graines Cosmos"]*2} 
		};
	}


	/// <summary>
	/// Updates the displayed money
	/// </summary>
	public void RefreshMoney()
	//Fonction de réaffichage de l'argent
	{
		GetNode<Label>("Argent/ArgentJoueur").SetText("Argent : " + PurchaseSaleControlScript.MoneyFormat(Global.Inventory.GetPlayerMoney()));
	}

	/// <summary>
	/// Quits the shop interface when some keys are clicked
	/// </summary>
	public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey EventKey)
			if (EventKey.Pressed && (EventKey.IsActionPressed("Interact") || EventKey.Keycode == Key.Escape)) {
				ExportData();
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Gameplay.tscn");
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
    }

	/// <summary>
	/// Initializes the class members
	/// </summary>
	public void LoadShopExtern() {
		MaterialMarketPrice=new Godot.Collections.Dictionary<string,double>{
			{"Pot haute qualité",10},
			{"Capteur d'humidité visuel",15},
			{"Luxmètre",5},
			{"Réduction de la consommation de la clime",20},
			{"Amélioration de la fiabilité de la clime",20},
			{"Capteur de température visuel",30},
			{"Réduction de la consommation du générateur",40},
			{"Panneaux solaires et batterie",50},
			{"LED basse consommation",50}
		};
		
		MaterialSale= new Godot.Collections.Array<string>();
		SeedsSale = new Godot.Collections.Array<string>();

		MustChangeArticles = false;
	    MustChangePrices = false;
		ChangeMarketPrices();
		ChangeArticles();
		ExportData();
	}
}
