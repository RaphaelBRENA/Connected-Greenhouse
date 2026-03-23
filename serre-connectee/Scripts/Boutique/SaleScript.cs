using Godot;
using System;

public partial class SaleScript : Panel
{
	//ATTRIBUTS _______________________________________________________________________________________

	public PurchaseSaleControlScript PurchaseSale; //Parent node
	private int SaleQuantity;
	private double TotalSalePrice;
	

	//READY ___________________________________________________________________________________________

	public override void _Ready(){

		PurchaseSale = GetNode<PurchaseSaleControlScript>("/root/BoutiqueInterface/Boutique/ControlAchatVente");
		SaleQuantity = 0; 
		TotalSalePrice = 0;

	}


	//METHODES ________________________________________________________________________________________

	/// <summary>
	/// Defines the last cell that has been clicked by the player
	/// </summary>
	/// <param name="c">The cell that has been the last one to be clicked.</param>
	public void SetSelectedCell(ShopCell c){
		PurchaseSale.SetSelectedCell(c,false);
		SetSelectedProductLabel();
		SetTotalSalePriceLabel(0);
	}

	/// <summary>
	/// Updates the selected product displayed name
	/// </summary>
	public void SetSelectedProductLabel(){
		GetNode<Label>("LigneProduit/Produit").SetText(PurchaseSaleControlScript.SelectedCell.GetProduct()+" : "+PurchaseSaleControlScript.SelectedCell.GetQuantity());
	}

	/// <summary>
	/// Updates the displayed total sale price
	/// </summary>
	public void SetTotalSalePriceLabel(double Price){
		TotalSalePrice = Price;
		GetNode<Label>("HBoxContainer/LabelPrix").SetText(PurchaseSaleControlScript.MoneyFormat(TotalSalePrice));
	}



	/// <summary>
	/// Changes to quantity to sell
	/// </summary>
	public void SetSaleQuantity(int q){
		//Change la quantité que le joueur veut vendre
		if(q>=0){
			SaleQuantity = q;
		}
		GetNode<QuantityScript>("HBoxContainer/LabelQuantite").SetText(SaleQuantity.ToString());
	}

	public int GetSaleQuantity(){
		return SaleQuantity;
	}

	public double GetTotalSalePrice(){
		return TotalSalePrice;
	}


	//SIGNAUX __________________________________________________________________________________________

	/// <summary>
	/// Calls the validation function of the sale if OK is pressed
	/// </summary>
	public void OnOkPressed(){
		if(!PurchaseSaleControlScript.CellType){
			Label TexteDialogue = GetNode<Label>("../Dialogue/Texte");
			string CurrentText = ValidationVente();
			TexteDialogue.SetText(CurrentText);
		}
	}

	/// <summary>
	/// Verifies if the sale can be done or not,
	/// Updates the money and quantity if so, and returns the dialog of the merchant
	/// </summary>
	private string ValidationVente(){
		if(PurchaseSaleControlScript.SelectedCell is not null){

			string Product = PurchaseSaleControlScript.SelectedCell.GetProduct();
			int ExistingQuantity = PurchaseSaleControlScript.SelectedCell.GetQuantity();

				if(SaleQuantity!=0){

					if(PurchaseSale.AcceptPayment(Product, SaleQuantity, TotalSalePrice)==1){
						
						InventoryScript Inventory = Global.Inventory;

						if((ExistingQuantity-SaleQuantity)>0){ // There'll still be some in the inventory
							Inventory.ModifyProductQuantity(Product,-SaleQuantity);
							Inventory.ModifyMoney(TotalSalePrice);
							PurchaseSale.RefreshMoney();
							PurchaseSaleControlScript.SelectedCell.SetQuantity(ExistingQuantity-SaleQuantity);
							PurchaseSaleControlScript.SelectedCell.RefreshCell();
							return "Merci pour cette offre !";
						}
						if((ExistingQuantity-SaleQuantity)==0){ // This will cause a shortage
							Inventory.ModifyProductQuantity(Product,-SaleQuantity);
							Inventory.ModifyMoney(TotalSalePrice);
							PurchaseSale.RefreshMoney();
							PurchaseSaleControlScript.SelectedCell.SetQuantity(ExistingQuantity-SaleQuantity);
							PurchaseSaleControlScript.SelectedCell.RefreshCell();
							PurchaseSaleControlScript.SelectedCell.DisplayImage();
							return "Merci pour cette offre ! \nIl ne vous reste plus de ce produit.";
						}
						if((ExistingQuantity-SaleQuantity)<0){ // The players tries to sell more than he owns
							return "Vous n'avez pas autant de ce produit en stock !";
						}

					}else{
						if(PurchaseSale.AcceptPayment(Product, SaleQuantity, TotalSalePrice)==0){
							return "C'est trop cher !";
						}else{
							return "Je n'ai pas besoin de ce produit.";
						}
					}
				
				}else{
					return "Dites-moi d'abord, combien voulez-vous m'en vendre ?";
				}
		}else{
			return "Dites-moi d'abord quel produit vous voulez me vendre !";
		}
		
		return "J'ai dû m'emmêler les pinceaux avec votre offre... \nVous pouvez me répéter ce que vous vouliez me vendre ?";

	}

}
