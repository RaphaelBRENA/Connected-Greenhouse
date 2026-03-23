using Godot;
using System;

public partial class QuantityScript : Label
{

	//ATTRIBUTS _______________________________________________________________________________________

	private Panel ParentNode;

	//READY ____________________________________________________________________________________________

	public override void _Ready(){
		ParentNode = GetNode<Panel>("../../");
	}
	
	//SIGNAUX _________________________________________________________________________________________


	/// <summary>
	/// Increase the quantity to purchase or sell
	/// </summary>
	public void OnPlusPressed(){
		int Max = 0;
		if(PurchaseSaleControlScript.SelectedCell is not null){
			Max = PurchaseSaleControlScript.SelectedCell.GetQuantity();
		}

		if(ParentNode is PurchaseScript && PurchaseSaleControlScript.CellType==true){ 
			int CurrentPurchaseQuantity =((PurchaseScript)ParentNode).GetPurchaseQuantity();
			if(CurrentPurchaseQuantity<Max){ 
				((PurchaseScript)ParentNode).SetPurchaseQuantity(CurrentPurchaseQuantity += 1) ;
				}
		}
		if(ParentNode is SaleScript && PurchaseSaleControlScript.CellType==false){
			int CurrentSaleQuantity = ((SaleScript)ParentNode).GetSaleQuantity();
			if(CurrentSaleQuantity<Max){ 
				((SaleScript)ParentNode).SetSaleQuantity(CurrentSaleQuantity += 1) ;
				}
		}						
		
	}

	/// <summary>
	/// Decreases the quantity to purchase or sell
	// </summary>
	public void OnMinusPressed(){ 
				
			if(ParentNode is PurchaseScript &&  PurchaseSaleControlScript.CellType==true){ 
				int CurrentPurchaseQuantity =((PurchaseScript)ParentNode).GetPurchaseQuantity();
				if(CurrentPurchaseQuantity>0){ //On peut encore diminuer la quantité
					((PurchaseScript)ParentNode).SetPurchaseQuantity(CurrentPurchaseQuantity -= 1) ;
				}
			}
			if(ParentNode is SaleScript && PurchaseSaleControlScript.CellType==false){
				int CurrentSaleQuantity = ((SaleScript)ParentNode).GetSaleQuantity();
				if(CurrentSaleQuantity>0){ //On peut encore diminuer la quantité
					((SaleScript)ParentNode).SetSaleQuantity(CurrentSaleQuantity -= 1) ;
				}
			}
	}



}
