using Godot;
using System;

public partial class InventoryViewScript: VBoxContainer
{

	//ATTRIBUTS _______________________________________________________________________________________

	public const int NBCASESPARLIGNE = 6;

	//READY ___________________________________________________________________________________________


	public override void _Ready(){
		LoadView();
	}

	//METHODES _________________________________________________________________________________________

	public void LoadView(){
		Godot.Collections.Array<string> AllKeys = Global.Inventory.GetInventoryKeys();
		int DernierIndiceGraine = 0;
		int DernierIndiceMateriel = 0;
		Godot.Collections.Array<string> SensorsArray = new Godot.Collections.Array<string>()
			{"Capteur de température visuel","Capteur d'humidité visuel",
			"Luxmètre","Hygromètre","Thermomètre"};
		for(int i=0; i<AllKeys.Count;i++){
			//Chargement des graines
			if(AllKeys[i].Length>7 && AllKeys[i].Substring(0,7)=="Graines"){
				if(DernierIndiceGraine<NBCASESPARLIGNE){
					GetNode<PotInventoryCellScript>("Ligne1/Case"+DernierIndiceGraine.ToString()).LoadCell(AllKeys[i], Global.Inventory.GetProductQuantity(AllKeys[i]));
					GetNode<PotInventoryCellScript>("Ligne1/Case"+DernierIndiceGraine.ToString()).ChargerNode2D(DernierIndiceGraine);
					DernierIndiceGraine++;
				}
				else if(DernierIndiceGraine>=NBCASESPARLIGNE && DernierIndiceGraine<(NBCASESPARLIGNE*2)){
					GetNode<PotInventoryCellScript>("Ligne2/Case"+DernierIndiceGraine.ToString()).LoadCell(AllKeys[i], Global.Inventory.GetProductQuantity(AllKeys[i]));
					GetNode<PotInventoryCellScript>("Ligne2/Case"+DernierIndiceGraine.ToString()).ChargerNode2D(DernierIndiceGraine);
					DernierIndiceGraine++;
				}
			}
			//Chargement du matériel
			if(SensorsArray.Contains(AllKeys[i])){
				if(DernierIndiceMateriel<NBCASESPARLIGNE){
					GetNode<PotInventoryCellScript>("Ligne3/Case"+DernierIndiceMateriel.ToString()).LoadCell(AllKeys[i], Global.Inventory.GetProductQuantity(AllKeys[i]));
					GetNode<PotInventoryCellScript>("Ligne3/Case"+DernierIndiceMateriel.ToString()).ChargerNode2D(DernierIndiceMateriel);
					DernierIndiceMateriel++;
				}
			}
		}		
		// A verifier : 
		for(int j=DernierIndiceGraine;j<(NBCASESPARLIGNE*2);j++){
			if(DernierIndiceGraine<NBCASESPARLIGNE){
					GetNode<PotInventoryCellScript>("Ligne1/Case"+DernierIndiceGraine.ToString()).VideCase();
					DernierIndiceGraine++;
				}
			else if(DernierIndiceGraine>=NBCASESPARLIGNE && DernierIndiceGraine<(NBCASESPARLIGNE*2)){
				GetNode<PotInventoryCellScript>("Ligne2/Case"+DernierIndiceGraine.ToString()).VideCase();
				DernierIndiceGraine++;
			}
		}

		for(int j=DernierIndiceMateriel;j<NBCASESPARLIGNE;j++){
				GetNode<PotInventoryCellScript>("Ligne3/Case"+DernierIndiceMateriel.ToString()).VideCase();
				DernierIndiceMateriel++;
		}
	}

	public int FindCell(string Product){
		int CellIndex = -1;
		for(int i=0; i<NBCASESPARLIGNE*2;i++){
			if(i<NBCASESPARLIGNE){
				if(GetNode<PotInventoryCellScript>("Ligne1/Case"+i.ToString()).GetItem()==Product){
					CellIndex = i;
				}
			}
			else if(i>=NBCASESPARLIGNE && i<(NBCASESPARLIGNE*2)){
				if(GetNode<PotInventoryCellScript>("Ligne2/Case"+i.ToString()).GetItem()==Product){
					CellIndex = i;
				}					
			}
		}
		for(int j=NBCASESPARLIGNE*2; j<NBCASESPARLIGNE*3;j++){
			if(GetNode<PotInventoryCellScript>("Ligne3/Case"+(j-(NBCASESPARLIGNE*2)).ToString()).GetItem()==Product){
					CellIndex = j;
			}
		}
		return CellIndex;
	}

	public void RefreshCell(int CellIndex){
		Godot.Collections.Array<string> AllKeys = Global.Inventory.GetInventoryKeys();
		if(CellIndex>=0 && CellIndex<18){
			if(CellIndex<6){
				GetNode<PotInventoryCellScript>("Ligne1/Case"+CellIndex.ToString()).Affichage();
			}
			if(CellIndex>=6 && CellIndex<12){
				GetNode<PotInventoryCellScript>("Ligne2/Case"+CellIndex.ToString()).Affichage();		
			}
			if(CellIndex>=12 && CellIndex<18){
				GetNode<PotInventoryCellScript>("Ligne3/Case"+(CellIndex-12).ToString()).Affichage();
			}
		}
	}
}
