using Godot;
using System;

public partial class ShopCell : Button
{


	//ATTRIBUTS _______________________________________________________________________________________

	private Panel ParentNode; 
	private string Product; 
	private int Quantity; 
	private Texture2D Image; 


	//READY ____________________________________________________________________________________________

	public override void _Ready(){
		ParentNode = GetNode<Panel>("../../../");
		Product = "Aucun produit sélectionné";
		Quantity = 0;
		Image = null;
	}

	//SIGNAUX __________________________________________________________________________________________

	
	public void OnCellPressed(){
		RefreshCell();
		if(Quantity==0){
			GetNode<Button>("../../../HBoxContainer/Ok").Disabled=true;
		}else{
			GetNode<Button>("../../../HBoxContainer/Ok").Disabled=false;
		}
	}


	//METHODES _______________________________________________________________________________________

	/// <summary>
	/// Reinitialize the selected quantity and changes the selected cell
	/// </summary>
	public void RefreshCell(){
		if(ParentNode is PurchaseScript){
			((PurchaseScript)ParentNode).SetPurchaseQuantity(0);
			((PurchaseScript)ParentNode).SetSelectedCell(this);
		}
		if(ParentNode is SaleScript){
			((SaleScript)ParentNode).SetSaleQuantity(0);
			((SaleScript)ParentNode).SetSelectedCell(this);
		}
	}


	/// <summary>
	/// Set the product stored on this cell
	/// </summary>
	/// <param name="p">The product.</param>
	/// <param name="q">The quantity available.</param>
	public void SetProduit(string p,int q){
		Product=p;
		Quantity=q;
		if(Quantity<1){
			Image = null;
		}else{
			if(Product.Length>8 && Product.Substring(0,7)=="Graines"){
			Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+(Product.Substring(8,Product.Length-8))+"Graine.png");
			}
			else{
				Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+Product+".png");
			}
		}
		DisplayImage();
		
	}

	/// <summary>
	/// Changes the quantity available on this cell
	/// </summary>
	/// <param name="Number">The new quantity.</param>
	public void SetQuantity(int Number){
		if(Number>0){
			Quantity = Number;
		}else{
			Quantity = Number;
			Product = "Aucun produit sélectionné";
			Image = null;
		}
	}

	public string GetProduct(){
		return Product;
	}

	public int GetQuantity(){
		return Quantity;
	}

	public void DisplayImage(){
		GetNode<TextureRect>("TextureRect").Texture = Image;
	}

}
