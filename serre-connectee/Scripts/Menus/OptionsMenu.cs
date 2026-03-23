using Godot;
using System;
using System.Collections.Generic;
public partial class OptionsMenu : Control
{
	List<Label> VolumeValues;
	List<HSlider> SlidersVolume;
	OptionButton ResolutionChoice;
	VBoxContainer General;
	HBoxContainer KeyboardKeys;
	private Button WaitingButton = null;
	CheckButton ButtonFullScreen;
	private ConfigFile Config;
	Dictionary<string, string> DictionaryKeys;
	bool FullscreenPleinEcran = false;

	/// <summary>
	/// Called when entering the OptionsMenu scene. Calls
	/// OpenConfigFile, InitialiseVolume, StartingResolution,
	/// InitialisePanels, InitialiseButtons, InitialiseDictionary,
	/// InitialiseInputMap, and ModifyVolume.
	/// </summary>
	public override void _Ready()
	{
		OpenConfigFile();
		InitialiseVolume();
		StartingResolution();
		InitialisePanels();
		InitialiseButtons();
		InitialiseDictionary();
		InitialiseInputMap();
		ModifyVolume();
	}

	/// <summary>
	/// Called every frame. Updates the displayed volume levels.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	public override void _Process(double delta)
	{
		ModifyVolume();
	}

	/// <summary>
	/// Toggles the fullscreen mode of the window based on the given parameter.
	/// Updates the configuration to reflect the fullscreen setting and saves it.
	/// Also resets the starting resolution to ensure the changes take effect.
	/// </summary>
	/// <param name="IsActive">Indicates whether the fullscreen mode should be activated.</param>
	public void OnFullScreenToggle(bool IsActive)
	{
		if (IsActive)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		Config.SetValue("Player1", "PleinEcran", IsActive.ToString());
		Config.Save("user://Save/ConfigOptions.cfg");
		StartingResolution();
	}

	/// <summary>
	/// Initializes the fullscreen mode based on the current configuration.
	/// If the configuration indicates that the game should be in fullscreen mode,
	/// sets the window to fullscreen mode. Otherwise, sets the window to windowed mode.
	/// </summary>
	public void InitFullScreen()
	{
		if (Config.GetValue("Player1", "PleinEcran").ToString() == "True")
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}

	/// <summary>
	/// Called when the user selects a resolution option in the resolution popup menu.
	/// Sets the window size to the selected resolution, and resets the starting resolution
	/// to ensure that the changes take effect. Also resets the fullscreen mode to windowed mode.
	/// </summary>
	/// <param name="Index">The index of the selected resolution option in the popup menu.</param>
	public void OnResolutionPressed(int Index)
	{
		string Item = ResolutionChoice.GetItemText(Index);
		string[] WidthAndHeight = Item.Split(" x ");
		if (int.TryParse(WidthAndHeight[0], out int Width) && int.TryParse(WidthAndHeight[1], out int Height))
		{
			ButtonFullScreen.SetPressed(false);
			DisplayServer.WindowSetSize(new Vector2I(Width, Height));
		}
		StartingResolution();
	}

	/// <summary>
	/// Initializes the resolution selection in the options menu by comparing the current window size
	/// to available resolution options. It selects the resolution option that closely matches the 
	/// current window size, allowing for a small margin of difference in width and height.
	/// </summary>
	public void StartingResolution()
	{
		ResolutionChoice = GetNode<OptionButton>("Panel/PartieGeneral/VBoxContainer5/HBoxContainer/OptionButton");
		Vector2I CurrentResolution = DisplayServer.WindowGetSize();
		for (int i = 0; i < ResolutionChoice.ItemCount; i++)
		{
			string Item = ResolutionChoice.GetItemText(i);
			string[] WidthAndHeight = Item.Split(" x ");
			if (WidthAndHeight.Length == 2 &&
				int.TryParse(WidthAndHeight[0], out int Width) &&
				int.TryParse(WidthAndHeight[1], out int Height))
			{
				if ((Mathf.Abs(Width - CurrentResolution.X) < 150) && (Mathf.Abs(Height - CurrentResolution.Y) < 85))
				{
					ResolutionChoice.Selected = i;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Called when the user presses the "General" button in the options menu.
	/// Shows the general options panel and hides the keyboard options panel.
	/// </summary>
	public void OnGeneralPressed()
	{
		General.Visible = true;
		KeyboardKeys.Visible = false;
	}

	/// <summary>
	/// Called when the user presses the "KeyboardKeys" button in the options menu.
	/// Hides the general options panel and shows the keyboard options panel.
	/// </summary>
	public void OnKeyboardKeyPressed()
	{
		General.Visible = false;
		KeyboardKeys.Visible = true;
	}

	/// <summary>
	/// Called when the user presses a button in the options menu.
	/// If WaitingButton is not null, it restores the text of WaitingButton to the
	/// key label associated with WaitingButton's name in the DictionaryKeys dictionary.
	/// Then it sets WaitingButton to the given Button, and changes the text of Button
	/// to "Appuyer sur une touche".
	/// </summary>
	/// <param name="Button">The button that was pressed.</param>
	private void OnButtonPressed(Button Button)
	{
		if (WaitingButton != null)
		{
			WaitingButton.SetText(DictionaryKeys[WaitingButton.Name]);
		}
		WaitingButton = Button;
		Button.Text = "Appuyer sur une touche";
	}

	/// <summary>
	/// Handles the input of the user in the OptionsMenu scene.
	/// If the escape key is pressed, the scene is changed to either the Gameplay scene if the user comes from the option menu,
	/// or the StartMenu scene in the other case.
	/// If the user presses a key while WaitingButton is not null, it sets the text of WaitingButton to the key label associated
	/// with the key, and adds the key to the DictionaryKeys dictionary.
	/// </summary>
	/// <param name="@event">The input event.</param>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Pressed && EventKey.Keycode == Key.Escape && Global.ComesFromOptionsMenu)
				GetTree().ChangeSceneToFile("Scenes/Gameplay.tscn");
			else if (EventKey.Pressed && EventKey.Keycode == Key.Escape && !Global.ComesFromOptionsMenu)
				GetNode<ActionTransition>("/root/Transition").ChangeToScene("Menu/Lancement/StartMenu.tscn");
		}
		if (WaitingButton != null && @event is InputEventKey KeyEvent && KeyEvent.Pressed)
		{
			string KeyName = KeyEvent.GetKeyLabel().ToString();
			if (string.IsNullOrEmpty(KeyName))
			{
				KeyName = KeyEvent.AsText();
			}
			bool Yes = true;
			foreach (var CurrentKey in DictionaryKeys.Keys)
			{
				if (DictionaryKeys[CurrentKey] == KeyName)
				{
					Yes = false;
				}
			}
			if (Yes)
			{
				WaitingButton.Text = KeyName;
				SetInputMap(KeyEvent);
				DictionaryKeys[WaitingButton.Name] = KeyName;
				Config.SetValue("Player1", WaitingButton.Name, KeyName);
				Config.Save("user://Save/ConfigOptions.cfg");
			}
			WaitingButton.SetText(DictionaryKeys[WaitingButton.Name]);
			WaitingButton = null;
		}
	}

	/// <summary>
	/// Initializes the volume values and sliders.
	/// The volume values and sliders are retrieved from the config file and are set to the current value.
	/// </summary>
	private void InitialiseVolume()
	{
		VolumeValues = new List<Label>();
		SlidersVolume = new List<HSlider>();
		for (int i = 0; i < 3; i++)
		{
			VolumeValues.Add(new Label());
			SlidersVolume.Add(new HSlider());
		}
		VolumeValues[0] = GetNode<Label>("Panel/PartieGeneral/VBoxContainer/HBoxContainer/Label2");
		VolumeValues[1] = GetNode<Label>("Panel/PartieGeneral/VBoxContainer2/HBoxContainer/Label2");
		VolumeValues[2] = GetNode<Label>("Panel/PartieGeneral/VBoxContainer3/HBoxContainer/Label2");
		SlidersVolume[0] = GetNode<HSlider>("Panel/PartieGeneral/VBoxContainer/VolumePrincipal");
		SlidersVolume[1] = GetNode<HSlider>("Panel/PartieGeneral/VBoxContainer2/Musique");
		SlidersVolume[2] = GetNode<HSlider>("Panel/PartieGeneral/VBoxContainer3/Bruits");

		for (int i = 0; i < 3; i++)
		{
			SlidersVolume[i].Value = (double)Config.GetValue("Player1", SlidersVolume[i].Name);
		}
	}

	/// <summary>
	/// Initializes all the buttons in the options menu.
	/// For each button in the "buttons" group, it adds an event handler to the button's Pressed event,
	/// and sets the text of the button to the value associated with the button's name in the config file.
	/// It also retrieves the full screen checkbox button and assigns it to the ButtonFullScreen field.
	/// </summary>
	private void InitialiseButtons()
	{
		foreach (Node Node in GetTree().GetNodesInGroup("buttons"))
		{
			if (Node is Button Button)
			{
				Button.Pressed += () => OnButtonPressed(Button);
				Button.Text = (String)Config.GetValue("Player1", Button.Name);
			}
		}
		ButtonFullScreen = GetNode<CheckButton>("Panel/PartieGeneral/VBoxContainer4/HBoxContainer/CheckButton");
	}



	/// <summary>
	/// Opens the configuration file and loads it into the Config object.
	/// Prints an error message to the console if the file cannot be loaded.
	/// </summary>
	private void OpenConfigFile()
	{
		Config = new ConfigFile();
		Error Error = Config.Load("user://Save/ConfigOptions.cfg");
		if (Error != Error.Ok)
		{
			GD.Print("fichier de Config non ouvert dans Options Menu. Création du fichier.");
			Config.SetValue("Player1", "VolumePrincipal", "23");
			Config.SetValue("Player1", "Musique", "63");
			Config.SetValue("Player1", "Bruits", "48");
			Config.SetValue("Player1", "Haut", "Z");
			Config.SetValue("Player1", "Bas", "S");
			Config.SetValue("Player1", "Gauche", "Q");
			Config.SetValue("Player1", "Droite", "D");
			Config.SetValue("Player1", "Interagir", "F");
			Config.SetValue("Player1", "Inventaire", "E");
			Config.SetValue("Player1", "PleinEcran", "False");
			
			Config.Save("user://Save/ConfigOptions.cfg");
		}
		
	}

	/// <summary>
	/// Initializes the panels in the options menu by retrieving the "General" and "KeyboardKeys" panels.
	/// </summary>
	private void InitialisePanels()
	{
		General = GetNode<VBoxContainer>("Panel/PartieGeneral");
		KeyboardKeys = GetNode<HBoxContainer>("Panel/PartieTouches");
	}

	/// <summary>
	/// Modifies the volume levels based on the values of the sliders,
	/// updates the displayed volume levels, and saves the configuration.
	/// </summary>
	private void ModifyVolume()
	{
		for (int i = 0; i < 3; i++)
		{
			VolumeValues[i].Text = SlidersVolume[i].Value.ToString();
			Config.SetValue("Player1", SlidersVolume[i].Name, SlidersVolume[i].Value.ToString());
		}
		Config.Save("user://Save/ConfigOptions.cfg");
	}

	/// <summary>
	/// Initializes the dictionary with the values of the config file.
	/// The dictionary is used to map the buttons' names to the keys' labels.
	/// </summary>
	private void InitialiseDictionary()
	{
		DictionaryKeys = new Dictionary<string, string>();
		DictionaryKeys["Haut"] = (string)Config.GetValue("Player1", "Haut");
		DictionaryKeys["Bas"] = (string)Config.GetValue("Player1", "Bas");
		DictionaryKeys["Gauche"] = (string)Config.GetValue("Player1", "Gauche");
		DictionaryKeys["Droite"] = (string)Config.GetValue("Player1", "Droite");
		DictionaryKeys["Interagir"] = (string)Config.GetValue("Player1", "Interagir");
		DictionaryKeys["Inventaire"] = (string)Config.GetValue("Player1", "Inventaire");
	}

	/// <summary>
	/// Sets the input map for the given key and action.
	/// </summary>
	/// <param name="cle">The key that was pressed.</param>
	private void SetInputMap(InputEventKey cle)
	{
		string ActionName = "";
		if (WaitingButton.Name == "Droite")
		{
			ActionName = "MoveRight";
		}
		else if (WaitingButton.Name == "Gauche")
		{
			ActionName = "MoveLeft";
		}
		else if (WaitingButton.Name == "Haut")
		{
			ActionName = "MoveForward";
		}
		else if (WaitingButton.Name == "Bas")
		{
			ActionName = "MoveBackward";
		}
		else if (WaitingButton.Name == "Inventaire")
		{
			ActionName = "Inventaire";
		}
		else if (WaitingButton.Name == "Interagir")
		{
			ActionName = "Interact";
		}
		if (InputMap.HasAction(ActionName))
		{
			InputMap.EraseAction(ActionName);
			InputMap.AddAction(ActionName);
			var InputEvent = new InputEventKey
			{
				Keycode = cle.GetKeyLabel(),
				Pressed = false
			};
			InputMap.ActionAddEvent(ActionName, InputEvent);
		}
	}

	/// <summary>
	/// Initializes the input map with key bindings for player actions.
	/// Opens the configuration file and retrieves key values for actions such as
	/// "MoveRight", "MoveLeft", "MoveForward", "MoveBackward", "Interact", and "Inventaire".
	/// For each action, the existing input mapping is erased and a new one is added
	/// based on the configuration values.
	/// </summary>
	public void InitialiseInputMap()
	{
		OpenConfigFile();
		if (InputMap.HasAction("MoveRight"))
		{
			InputMap.EraseAction("MoveRight");
			InputMap.AddAction("MoveRight");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Droite"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("MoveRight", InputEvent);
		}
		if (InputMap.HasAction("MoveLeft"))
		{
			InputMap.EraseAction("MoveLeft");
			InputMap.AddAction("MoveLeft");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Gauche"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("MoveLeft", InputEvent);
		}
		if (InputMap.HasAction("MoveForward"))
		{
			InputMap.EraseAction("MoveForward");
			InputMap.AddAction("MoveForward");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Haut"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("MoveForward", InputEvent);
		}
		if (InputMap.HasAction("MoveBackward"))
		{
			InputMap.EraseAction("MoveBackward");
			InputMap.AddAction("MoveBackward");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Bas"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("MoveBackward", InputEvent);
		}
		if (InputMap.HasAction("Interact"))
		{
			InputMap.EraseAction("Interact");
			InputMap.AddAction("Interact");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Interagir"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("Interact", InputEvent);
		}
		if (InputMap.HasAction("Inventaire"))
		{
			InputMap.EraseAction("Inventaire");
			InputMap.AddAction("Inventaire");
			var InputEvent = new InputEventKey
			{
				Keycode = (Key)Enum.Parse(typeof(Key), (string)Config.GetValue("Player1", "Inventaire"), ignoreCase: true),
				Pressed = false
			};
			InputMap.ActionAddEvent("Inventaire", InputEvent);
		}
	}
}
