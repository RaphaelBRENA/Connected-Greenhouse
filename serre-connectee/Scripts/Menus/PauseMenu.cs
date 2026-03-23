using Godot;
using System;
using Newtonsoft.Json;

public partial class PauseMenu : Control
{
	// READY ___________________________________________________________________________________________

	public override void _Ready()
    {
		GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
		this.Hide();
    }

	// INPUT ___________________________________________________________________________________________

	public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey EventKey) {
			if (EventKey.Pressed && EventKey.Keycode == Key.Escape && GetTree().Paused == false && !GetNode<InventoryInterface>("/root/Gameplay/Inventaire/InventaireInterface").GetIsOpen())
				Pause();
			else if (EventKey.Pressed && EventKey.Keycode == Key.Escape && GetTree().Paused == true && !GetNode<InventoryInterface>("/root/Gameplay/Inventaire/InventaireInterface").GetIsOpen())
				Restart();
		}
    }

	// METHODES ___________________________________________________________________________________________

	public void Restart()
	{
		GetNode<Button>("Panel/VBoxContainer/Restart").Disabled = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
		this.Hide();
	}
	public void Pause()
	{
		GetNode<Button>("Panel/VBoxContainer/Restart").Disabled = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;
		this.Show();
		GetNode<AnimationPlayer>("AnimationPlayer").Play("blur");
	}
	public void OnRestartPressed()
	{
		GetNode<Button>("Panel/VBoxContainer/Restart").Disabled = true;
		Restart();
	}
	public void OnOptionsPressed()
	{
		Global.ComesFromOptionsMenu = true;
		GetTree().Paused = false;
		GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Options/OptionsMenu.tscn"); 
	}
	// Script PauseMenu

    public void OnSaveAndQuitPressed()
    {
		/*String SaveFilePath = $"user://Save/{Global.CurrentSaveName}.json"";
		Godot.File CurrentFile = new Godot.File();
		bool DoesSaveFileExists = CurrentFile.FileExists(SaveFilePath);
		if(!DoesSaveFileExists){

		}*/
        using var SaveFile = FileAccess.Open($"user://Save/{Global.CurrentSaveName}.json", FileAccess.ModeFlags.Write);




        Godot.Collections.Array<Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>> Data = new Godot.Collections.Array<Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>>();
        Data.Add(new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>{ 
            {"Generaux", new Godot.Collections.Array<string>(){"true", Time.GetDateStringFromSystem().ToString(), PopupManager.TutoIsDone().ToString()}}
        });
        foreach (string CurrentKey in Global.SaveInteract.Keys)
            Data.Add(Global.SaveInteract[CurrentKey].SendData());
        Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Inventory = SaveInventory();
        Data.Add(Inventory);
        string jsonLine = JsonConvert.SerializeObject(Data);
        SaveFile.StoreLine(jsonLine);

        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
        GetTree().Paused = false;
        GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Lancement/StartMenu.tscn"); 
    
        Global.FirstConnection = true;
		Global.PlayerStatus = "";
	    GetNode<InteractionComponent>("../../World/World/Etagere/CasquetteInfo/InteractionComponent").ShowCaps();
    }

    public Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> SaveInventory()
    {
        Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
        Godot.Collections.Array[,] CurrentArray = Global.Inventory.GetArray();
        for (int i = 0; i < InventoryScript.INVENTORYLINESNUMBER; i++)
        {
            Godot.Collections.Array<string> Line = new Godot.Collections.Array<string>();
            for (int j = 0; j < (InventoryScript.MAXINVENTORYSIZE/InventoryScript.INVENTORYLINESNUMBER); j++)
            {
                string Value = CurrentArray[i, j][0] + "," + CurrentArray[i, j][1];
                Line.Add(Value);
            }
            Data.Add("Ligne " + (i+1), Line);
        }
        return Data;
    }

	public void OnDictionaryPressed()
	{
		Global.ComesFromBook = true;
		GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
		GetTree().Paused = false;
		GetNode<ActionTransition>("/root/Transition").ChangeToScene("Interface/EncyclopedieInterface.tscn"); 
	}
}
