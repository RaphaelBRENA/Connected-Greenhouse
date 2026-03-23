using Godot;
using System;

public partial class InventoryPanelScript : Panel
{

	//ATTRIBUTS _______________________________________________________________________________________

	InventoryScript Inventory;

	//READY ___________________________________________________________________________________________

	public override void _Ready(){
		Inventory = GetNode<InventoryScript>("../../Inventaire");
		Inventory._Ready();
	}

	//PROCESS __________________________________________________________________________________________

	public override void _Process(double delta){}

	//METHODES _________________________________________________________________________________________

	private void ChargerInventaire(){
		Godot.Collections.Array<string> AllKeys = Inventory.GetInventoryKeys();
		int LineSize = (InventoryScript.MAXINVENTORYSIZE)/(InventoryScript.INVENTORYLINESNUMBER); 
		for(int i=0; i<AllKeys.Count;i++){
			if(i<LineSize){
				GetNode<InventoryCell>("VBoxContainer/Ligne1/Case"+i.ToString()).SetItem(AllKeys[i],Inventory.GetProductQuantity(AllKeys[i]));
			}
			if(i>=LineSize && i<(2*LineSize)){
				GetNode<InventoryCell>("VBoxContainer/Ligne2/Case"+i.ToString()).SetItem(AllKeys[i],Inventory.GetProductQuantity(AllKeys[i]));
			}
			if(i>=(2*LineSize) && i<(3*LineSize)){
				GetNode<InventoryCell>("VBoxContainer/Ligne3/Case"+i.ToString()).SetItem(AllKeys[i],Inventory.GetProductQuantity(AllKeys[i]));
			}
		}
	}

}
