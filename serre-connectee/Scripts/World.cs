using Godot;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public partial class World : Node3D
{
	private CompressedTexture2D Sun = (CompressedTexture2D)ResourceLoader.Load("res://Assets/Images/BackgroundSky/soleil.exr");
	private CompressedTexture2D Cloudy = (CompressedTexture2D)ResourceLoader.Load("res://Assets/Images/BackgroundSky/nuageux.exr");
	private CompressedTexture2D Snow = (CompressedTexture2D)ResourceLoader.Load("res://Assets/Images/BackgroundSky/neige.exr");
	// Called when the Node enters the scene tree for the first time.
	public override void _Ready()
	{
        Input.MouseMode = Input.MouseModeEnum.Captured;
		//Fonction d'initialisation de l'environnement 3D en fonction de la météo et de la sauvegarde
		if(Global.FirstConnection){
			LoadSaveStrategy();
			Global.FirstConnection=false;
		}

		Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = Global.SaveInteract["Calendrier"].SendData();
		string Weather = Data["Weather"][0];
		if (Weather.Find("Soleil") != (-1)) {
			GetNode<WorldEnvironment>("WorldEnvironment").Environment.VolumetricFogEnabled = false;
			PanoramaSkyMaterial CurrentSkyMaterial = (PanoramaSkyMaterial) GetNode<WorldEnvironment>("WorldEnvironment").Environment.Sky.SkyMaterial;
			CurrentSkyMaterial.Panorama = Sun;
		}
		if (Weather.Find("Nuageux") != (-1)) {
			GetNode<WorldEnvironment>("WorldEnvironment").Environment.VolumetricFogEnabled = true;
			PanoramaSkyMaterial CurrentSkyMaterial = (PanoramaSkyMaterial) GetNode<WorldEnvironment>("WorldEnvironment").Environment.Sky.SkyMaterial;
			CurrentSkyMaterial.Panorama = Cloudy;
			GpuParticles3D CurrentParticles3D = GetNode<GpuParticles3D>("GPUParticles3D");
			CurrentParticles3D.Emitting = false;
		}
		if (Weather.Find("Pluie") != (-1)) {
			GetNode<WorldEnvironment>("WorldEnvironment").Environment.VolumetricFogEnabled = false;
			GetNode<GpuParticles3D>("GPUParticles3D").Emitting = true;
			GetNode<GpuParticles3D>("GPUParticles3D").DrawPass1 = (RibbonTrailMesh) ResourceLoader.Load("res://Assets/Material/pluie.tres");
		}
		if (Weather.Find("Neige et Gel") != (-1)) {
			GetNode<WorldEnvironment>("WorldEnvironment").Environment.VolumetricFogEnabled = false;
			PanoramaSkyMaterial CurrentSkyMaterial = (PanoramaSkyMaterial) GetNode<WorldEnvironment>("WorldEnvironment").Environment.Sky.SkyMaterial;
			CurrentSkyMaterial.Panorama = Snow;
			GpuParticles3D CurrentParticles3D = GetNode<GpuParticles3D>("GPUParticles3D");
			CurrentParticles3D.Emitting = true;
			CurrentParticles3D.DrawPass1 = (SphereMesh) ResourceLoader.Load("res://Assets/Material/snow.tres");
		}
		for (int i = 1; i <= 6; i++) {
			if (Global.SaveInteract["Plan de Terre " + i].SendData() != null) {
				int Counter = 1;
				Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> DirtSlot = Global.SaveInteract["Plan de Terre " + i].SendData();
				foreach (string CurrentKey in DirtSlot.Keys) {
					if (DirtSlot[CurrentKey][9] != "0") {
                        if (Plant.StagesArray[DirtSlot[CurrentKey][10].ToInt()] == "Graine") {
                            GetNode<MeshInstance3D>("Plan de Terre " + i + "/Plante" + Counter).Mesh = (Mesh) ResourceLoader.Load("res://Assets/Models/Plants/Graines.res");
                        } else {
                            String path = $"res://Assets/Models/Plants/{DirtSlot[CurrentKey][9].Substring(8,DirtSlot[CurrentKey][9].Length-8)}{Plant.StagesArray[DirtSlot[CurrentKey][10].ToInt()]}";
                            if (DirtSlot[CurrentKey][14] == "True")
                                path += "Morte";
                            path += ".res";
						    GetNode<MeshInstance3D>("Plan de Terre " + i + "/Plante" + Counter).Mesh = (Mesh) ResourceLoader.Load(path);
                        }
					}
					Counter++;
				}
			}
		}

        if (Global.PlayerStatus == "INFO") GetNode<StaticBody3D>("Etagere/CasquetteInfo").Visible = false;
        if (Global.PlayerStatus == "BIO") GetNode<StaticBody3D>("Etagere/CasquetteBio").Visible = false;
        if (Global.PlayerStatus == "GEA") GetNode<StaticBody3D>("Etagere/CasquetteGea").Visible = false;
        if (Global.PlayerStatus == "MT2E") GetNode<StaticBody3D>("Etagere/CasquetteMT2E").Visible = false;
	}
    private void LoadSaveStrategy()
    {
        String SaveFilePath = $"user://Save/{Global.CurrentSaveName}.json";
		bool DoesSaveFileExists = FileAccess.FileExists(SaveFilePath);
		if(!DoesSaveFileExists){
            var TempFile = FileAccess.Open($"user://Save/{Global.CurrentSaveName}.json", FileAccess.ModeFlags.Write);
            TempFile.Close();
        }
        using var SaveFile = FileAccess.Open($"user://Save/{Global.CurrentSaveName}.json", FileAccess.ModeFlags.Read);

        Godot.Collections.Array<Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>> Data = JsonConvert.DeserializeObject<Godot.Collections.Array<Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>>>(SaveFile.GetLine());
        if (Data != null) {
            for (int i = 0; i < Data.Count; i++) {
                switch (i) {
                    case int j when j>=7 && j<13:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Plan de Terre " + (i-6)].KeepData(Data[i]);
                    break;
                    case 0:
                    if (Data[i].Count != 0) {
                        if (Data[i]["Generaux"][2].ToString() == "False")
                        {
                            List<KeyValuePair<string, bool>> Tuto = PopupManager.GetTutorialActionsArray();
                            for (int j = 0; j < Tuto.Count; j++)
                            {
                                PopupManager.SetBoolAction(Tuto[j].Key, false);
                            }
                        } else {
                            List<KeyValuePair<string, bool>> Tuto = PopupManager.GetTutorialActionsArray();
                            for (int j = 0; j < Tuto.Count; j++)
                            {
                                PopupManager.SetBoolAction(Tuto[j].Key, true);
                            }
                        }
                    }
                    break;
                    case 1:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Calendrier"].KeepData(Data[i]);
                    break;
                    case 2:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Coffre"].KeepData(Data[i]);
                    break;
                    case 3:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Boutique"].KeepData(Data[i]);
                    break;
                    case 4:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Programmation"].KeepData(Data[i]);
                    break;
                    case 5:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Thermostat"].KeepData(Data[i]);
                    break;
					case 6:
                    if (Data[i].Count != 0)
                        Global.SaveInteract["Cout Energetique"].KeepData(Data[i]);
                    break;
                    case 13:
                    if (Data[i].Count != 0) {
                        int Counter = 1;
                        Godot.Collections.Array[,] CurrentArray = new Godot.Collections.Array[InventoryScript.INVENTORYLINESNUMBER, (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER)];
                        Godot.Collections.Dictionary<string, int> Inventory = new Godot.Collections.Dictionary<string, int>();
                        for (int j = 0; j < InventoryScript.INVENTORYLINESNUMBER; j++)
                        {
                            for (int k = 0; k < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); k++)
                            {
                                string[] TmpArray = Data[i]["Ligne " + Counter][k].Split(",");
                                CurrentArray[j, k] = new Godot.Collections.Array{ TmpArray[0], TmpArray[1].ToInt()};
                                if (CurrentArray[j, k][0].ToString() != "")
                                    Inventory.Add(TmpArray[0], TmpArray[1].ToInt());
                            }
                            Counter++;
                        }
                        Global.Inventory.SetPositionArray(CurrentArray);
                        Global.Inventory.SetInventory(Inventory);
                    }
                    break;
                }
            }
        } else {
            List<KeyValuePair<string, bool>> Tuto = PopupManager.GetTutorialActionsArray();
            for (int i = 0; i < Tuto.Count; i++)
            {
                PopupManager.SetBoolAction(Tuto[i].Key, false);
            }
        }
    }
}
