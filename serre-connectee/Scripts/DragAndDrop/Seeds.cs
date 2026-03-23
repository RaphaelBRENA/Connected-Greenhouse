using Godot;
using System;
using System.Collections.Generic;

public partial class Seeds : Node2D
{
	bool Draggable = false;
	bool IsInsideDropable = false;
	StaticBody2D BodyRef;
	Vector2 Offset;
	Vector2 InitialPos;


	static List<LargeCellsSlots> GreatCollidersArray;
	Node2D Node;

	/// <summary>
	/// Called when entering the SerreInterface scene. Gets the current node and passes it as argument
	/// to InitialiseGreatCollidersArray.
	/// </summary>
	public override void _Ready()
	{
		Node = this;
		InitialiseGreatCollidersArray(Node);
	}

	/// <summary>
	/// Called every frame. If the seed is draggable, it listens for left-click events.
	/// When the left-click button is pressed, it sets the seed to be dragged and sets its initial position.
	/// When the left-click button is released, it sets the seed to not be dragged and checks if the seed is inside a dropable area.
	/// If it is, it adds the seed to the area and removes it from the inventory.
	/// If it is not, it moves the seed back to its initial position.
	/// </summary>
	public override void _Process(double delta)
	{
		if (Draggable)
		{
			if (Input.IsActionJustPressed("Click"))
			{
				this.ZIndex = 1;
				HideSensors();
				InitialPos = GlobalPosition;
				Offset = GetGlobalMousePosition() - GlobalPosition;
				Global.IsDragging = true;
			}
			if (Input.IsActionPressed("Click"))
			{
				GlobalPosition = GetGlobalMousePosition() - Offset;
			}
			else if (Input.IsActionJustReleased("Click"))
			{
				this.ZIndex = 0;
				Global.IsDragging = false;
				LargeCellsSlots GreatCollider = (LargeCellsSlots)BodyRef;
				if (IsInsideDropable && GreatCollider.GetItem() == "")
				{
					int LastNonDigitIndex = this.Name.ToString().LastIndexOfAny("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-".ToCharArray());
					Global.Inventory.ModifyProductQuantity(this.Name.ToString().Substring(0, LastNonDigitIndex + 1), -1);
					GreatCollider.SetItem(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
				}
				else
				{
					var CurrentTween = GetTree().CreateTween();
					CurrentTween.TweenProperty(this, "global_position", InitialPos, 0f).SetEase(Tween.EaseType.Out);
				}
				HideSensors();
			}
		}
	}

	/// <summary>
	/// Called when the mouse pointer enters the area of the seed. 
	/// If no dragging operation is in progress, it sets the draggable status to true.
	/// </summary>
	public void OnMouseEntered()
	{
		if (!Global.IsDragging)
		{
			Draggable = true;
		}
	}

	/// <summary>
	/// Called when the mouse exits the seed. If the seed is not being dragged
	/// (i.e. Global.IsDragging is false), then the seed is no longer draggable.
	/// </summary>
	public void OnMouseExited()
	{
		if (!Global.IsDragging)
		{
			Draggable = false;
		}
	}

	/// <summary>
	/// Called when the seed enters a body. If the body is in the "GrossesCases" group,
	/// it sets the seed as inside a dropable area. The reference to the body 
	/// is stored in BodyRef.
	/// </summary>
	public void OnBodyEntered(StaticBody2D Body)
	{
		if (Body.IsInGroup("GrossesCases"))
		{
			IsInsideDropable = true;
			BodyRef = Body;
		}
	}

	/// <summary>
	/// Called when the seed exits a body. If the body is in the "GrossesCases" group,
	/// it sets the seed as outside a dropable area.
	/// </summary>
	public void OnBodyExited(StaticBody2D Body)
	{
		Area2D OverLappingBodies = GetNode<Area2D>("Area2D");
		if (Body.IsInGroup("GrossesCases"))
		{
			IsInsideDropable = false;
		}
	}

	/// <summary>
	/// Initializes the GreatCollidersArray by creating a new list of SlotsGrandesCases objects.
	/// For each index from 0 to 8, it assigns a SlotsGrandesCases instance to the corresponding
	/// StaticBody2D node named "GrossesCase" followed by the index, found in the parent of the given Node.
	/// </summary>
	/// <param name="Node">The Node2D whose parent contains the GrossesCase nodes.</param>
	public static void InitialiseGreatCollidersArray(Node2D Node)
	{
		GreatCollidersArray = new List<LargeCellsSlots>();
		for (int i = 0; i < 9; i++)
		{
			GreatCollidersArray.Add(new LargeCellsSlots());
			GreatCollidersArray[i] = (LargeCellsSlots)Node.GetParent().GetNode<StaticBody2D>("GrossesCase" + i);
		}
	}

	/// <summary>
	/// Hides the sensors for each SlotsGrandesCases in the GreatCollidersArray,
	/// and sets the seed status to true. It also changes the text of the button in the
	/// inventory panel to "Voir Capteurs".
	/// </summary>
	public void HideSensors()
	{
		for (int i = 0; i < GreatCollidersArray.Count; i++)
		{
			GreatCollidersArray[i].HideSensors();
			GreatCollidersArray[i].SetIsOnSeed(true);
		}
		GetParent().GetParent().GetNode<Button>("ControlGeneral/Control/Panneau/VueInventaire/Button").SetText("Voir Capteurs");
	}
}

