using Godot;
using System;

	/// <summary>
	/// Manages the short and long presses of the buttons that change the price of what is sold.
	/// </summary>
public partial class PriceScript : Label
{

	//ATTRIBUTS _______________________________________________________________________________________

	private SaleScript ParentNode; 
	private bool PlusIsPressed; 
	private bool MinusIsPressed; 
	private bool UniquePlus; //true if + is only pressed once (not a long press)
	private bool UniqueMinus;//true if - is only pressed once (not a long press)
	private int Cooldown; //cooldown before being considered a long press
	private int ElapsedTime = 0; //time compared to the cooldown



	//READY ____________________________________________________________________________________________

	public override void _Ready(){
		ParentNode = GetNode<SaleScript>("../../");
		PlusIsPressed = false;
		MinusIsPressed = false;
		UniquePlus = true;
	    UniqueMinus = true;
		Cooldown = 200;
	}

	
	//PROCESS _________________________________________________________________________________________

	/// <summary>
	/// Called many times a second in order to detect the short or long press of the + and -
	/// </summary>
	public override void _Process(double delta){
		if(PurchaseSaleControlScript.SelectedCell is not null){
			if(PurchaseSaleControlScript.CellType==false){
				
				if(UniquePlus == true){
					UniquePlus = false;
					if(PurchaseSaleControlScript.SelectedCell is not null){
					if(PurchaseSaleControlScript.CellType==false){
								double Price = ParentNode.GetTotalSalePrice()+0.5;
								Price = ((int)(Price*100))/100.0;
								ParentNode.SetTotalSalePriceLabel(Price);
							}
					}
					Cooldown = 50;
					ElapsedTime=0;
				}
				else if(UniqueMinus == true){
					UniqueMinus = false;
					if(PurchaseSaleControlScript.SelectedCell is not null){
						if(PurchaseSaleControlScript.CellType==false){
							double Price = ParentNode.GetTotalSalePrice()-0.5;
							Price = ((int)(Price*100))/100.0;
							if(Price>=0){ //Can't sell a negative price
								ParentNode.SetTotalSalePriceLabel(Price);
							}
						}
					}
					Cooldown = 50;
					ElapsedTime=0;
				}
				
				else if(ElapsedTime%Cooldown==0){
					Cooldown = 10;
					
					if(PlusIsPressed){
						double Price = ParentNode.GetTotalSalePrice()+0.5;
						Price = ((int)(Price*100))/100.0;
						ParentNode.SetTotalSalePriceLabel(Price);
					}	
					
					if(MinusIsPressed){
						double Price = ParentNode.GetTotalSalePrice()-0.5;
						Price = ((int)(Price*100))/100.0;
						if(Price>=0){ //Can't sell a negative price
							ParentNode.SetTotalSalePriceLabel(Price);
						}
					}
				
				}
				ElapsedTime+=1;
			}
		}

	}

	//SIGNAUX _________________________________________________________________________________________

	public void OnPlusDown(){
		PlusIsPressed = true;
		UniquePlus = true;
		
		}

	public void OnPlusUp(){
		PlusIsPressed = false;
		Cooldown = 400;
	}

	public void OnMinusDown(){ 
		MinusIsPressed = true;
	    UniqueMinus = true;
		
	}

	public void OnMinusUp(){
		MinusIsPressed = false;
		Cooldown = 400;
	}

}
