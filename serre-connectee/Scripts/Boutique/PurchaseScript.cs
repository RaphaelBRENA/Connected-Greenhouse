using Godot;
using System;


public partial class PurchaseScript : Panel
{


	//ATTRIBUTS _______________________________________________________________________________________

	public PurchaseSaleControlScript PurchaseSale; //Parent Node
	private int PurchaseQuantity; 
	private double PurchaseTotalPrice; 

	//READY ___________________________________________________________________________________________

	public override void _Ready(){
		PurchaseSale = GetNode<PurchaseSaleControlScript>("/root/BoutiqueInterface/Boutique/ControlAchatVente");
		PurchaseQuantity = 0; 
		
	}


	//METHODES ________________________________________________________________________________________

	/// <summary>
	/// Defines which cell was the last one clicked by the player.
	/// </summary>
	/// <param name="c">The cell which was the last one clicked by the player.</param>
	public void SetSelectedCell(ShopCell c){
		PurchaseSale.SetSelectedCell(c,true);
		SetSelectedProductLabel();
		DisplayPurchaseTotalPrice();
	}

	/// <summary>
	/// Updates the displayed name of the selected product
	/// </summary>
	public void SetSelectedProductLabel(){
		GetNode<Label>("LigneProduit/Produit").SetText(PurchaseSaleControlScript.SelectedCell.GetProduct());
	}

	/// <summary>
	/// Updates the displayed purchase total price
	/// </summary>
	public void DisplayPurchaseTotalPrice(){
		PurchaseTotalPrice = (PurchaseQuantity*PurchaseSale.GetUnitPrice(PurchaseSaleControlScript.SelectedCell.GetProduct()));
		GetNode<Label>("HBoxContainer/Prix").SetText("Prix total : "+PurchaseSaleControlScript.MoneyFormat(PurchaseTotalPrice));
	}

	/// <summary>
	/// Changes the quantity to purchase of the selected product
	/// then updates the displayed information
	/// </summary>
	/// <param name="q">The new quantity.</param>
	public void SetPurchaseQuantity(int q){
		if(q>=0){
			PurchaseQuantity = q;
		}
		GetNode<QuantityScript>("HBoxContainer/LabelQuantite").SetText(PurchaseQuantity.ToString());
		if(PurchaseSaleControlScript.SelectedCell is not null){
			DisplayPurchaseTotalPrice();
		}
	}

	public int GetPurchaseQuantity(){
		return PurchaseQuantity;
	}

	//SIGNAUX __________________________________________________________________________________________

	/// <summary>
	/// Calls the validation function of the purchase when OK is pressed
	/// </summary>
	public void OnOkPressed(){
		if(PurchaseSaleControlScript.CellType){
			Label DialogText = GetNode<Label>("../Dialogue/Texte");
			string CurrentText = PurchaseValidation();
			DialogText.SetText(CurrentText);
			PurchaseSale.LoadSale();
		}
	}

	/// <summary>
	/// Verifies if the purchase can be done or not
	/// Updates money and quantity if so, and returns the dialog of the merchant
	/// </summary>
	public string PurchaseValidation(){
			if(PurchaseSaleControlScript.SelectedCell is not null){

				string Product = PurchaseSaleControlScript.SelectedCell.GetProduct();
				int ExistingQuantity = PurchaseSaleControlScript.SelectedCell.GetQuantity();
				InventoryScript	Inventory = Global.Inventory;
				double PlayerMoney = Inventory.GetPlayerMoney();

				if(PlayerMoney-PurchaseTotalPrice>=0){ //[A] The player has enough money
					if(PurchaseQuantity>0){ //[B] He wants more than 0 quantity

						if(Inventory.ContainsProduct(Product)){ //[C]The inventory has that product

							if((ExistingQuantity-PurchaseQuantity)>0){ //[D] There'll still be some in the shop
								Inventory.ModifyProductQuantity(Product,PurchaseQuantity);
								Inventory.ModifyMoney(-PurchaseTotalPrice);
								PurchaseSale.RefreshMoney();
								PurchaseSaleControlScript.SelectedCell.SetQuantity(ExistingQuantity-PurchaseQuantity);
								PurchaseSaleControlScript.SelectedCell.RefreshCell();

								return "Merci pour cet achat !";
							}
							if((ExistingQuantity-PurchaseQuantity)==0){ //[D] This will cause a shortage in the shop
								Inventory.ModifyProductQuantity(Product,PurchaseQuantity);
								Inventory.ModifyMoney(-PurchaseTotalPrice);
								PurchaseSale.RefreshMoney();
								PurchaseSaleControlScript.SelectedCell.SetQuantity(ExistingQuantity-PurchaseQuantity);
								PurchaseSaleControlScript.SelectedCell.RefreshCell();

								return "Merci pour cet achat ! \nCe produit est maintenant en rupture de stock.";
							}
							if((ExistingQuantity-PurchaseQuantity)<0){ //[D] The player wants more than there is in the shop
								return "Il n'y a pas autant de ce produit en stock !";
							}


						}else{ //[C] The inventory doesn't have this product

							if(!(Inventory.IsFull())){ //[E] The inventory still has place
							
								if((ExistingQuantity-PurchaseQuantity)>0){ //[D] There'll still be some in the shop
									Inventory.AddProduct(Product,PurchaseQuantity);
									Inventory.ModifyMoney(-PurchaseTotalPrice);
									PurchaseSale.RefreshMoney();
									PurchaseSaleControlScript.SelectedCell.SetQuantity(ExistingQuantity-PurchaseQuantity);
									PurchaseSaleControlScript.SelectedCell.RefreshCell();

									return "Merci pour cet achat !";
								}
								if((ExistingQuantity-PurchaseQuantity)==0){ //[D] This will cause a shortage in the shop
									Inventory.AddProduct(Product,PurchaseQuantity);
									Inventory.ModifyMoney(-PurchaseTotalPrice);
									PurchaseSale.RefreshMoney();
									PurchaseSaleControlScript.SelectedCell.SetProduit(Product+" : rupture de stock",0);
									PurchaseSaleControlScript.SelectedCell.RefreshCell();
									return "Merci pour cet achat ! \nCe produit est maintenant en rupture de stock.";
								}
								if((ExistingQuantity-PurchaseQuantity)<0){ //[D] The player wants more than there is in the shop
									return "Il n'y a pas autant de ce produit en stock !";
								}

							}else{ //[E] The inventory's full
								return "Vous ne pouvez pas acheter ce produit. \nVotre inventaire est plein.";
							}

						}

					}else{ //[B] The player wants to buy 0 quantity of that product
						return "Vous ne m'avez pas dit quelle quantité vous vouliez acheter !";
					}

				}else{ //[A] The player doesn't have enough money for the purchase

					return "Il semblerait que vous n'ayez pas l'argent pour faire cet achat...";
				}
			}else{
				return "Dites-moi d'abord quel produit vous voulez acheter !";
			}
		
		    return "J'ai dû m'emmêler les pinceaux avec votre commande... \nVous pouvez me répéter ce que vous vouliez ?";

	}

}
