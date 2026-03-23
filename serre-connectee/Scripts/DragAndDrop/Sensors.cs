using Godot;
using System;
using System.Collections.Generic;

public partial class Sensors : Node2D
{
	private bool Draggable = false;
	private bool IsInsideDropable = false;
	private bool IsInsideSmallCollider = false;
	private StaticBody2D BodyRef;
	private Vector2 Offset;
	private Vector2 InitialPos;
	private GridControlScript Control;
	private Node2D Node;

	public static List<LargeCellsSlots> GreatCollidersArray;


	/// <summary>
	/// Called when entering the SerreInterface scene. Gets the GridControlScript from the Control node
	/// in the parent of the parent, and sets the Node to the current node. Then calls
	/// InitialiseGreatCollidersArray with the Node as the argument.
	/// </summary>
	public override void _Ready()
	{
		Control = (GridControlScript)GetParent().GetParent().GetNode<Control>("ControlCarre");
		Node = this;
		InitialiseGreatCollidersArray(Node);
	}

	/// <summary>
	/// Called every frame. If the sensor is draggable, it listens for left-click events.
	/// When the left-click button is pressed, it sets the sensor to be dragged and sets its initial position.
	/// When the left-click button is released, it sets the sensor to not be dragged and checks if the sensor is inside a dropable area.
	/// If it is, it adds the sensor to the area and removes it from the inventory.
	/// If it is not, it moves the sensor back to its initial position.
	/// </summary>
	public override void _Process(double delta)
	{
		if (Draggable)
		{
			if (Input.IsActionJustPressed("Click"))
			{
				this.ZIndex = 1;
				InitialPos = GlobalPosition;
				Offset = GetGlobalMousePosition() - GlobalPosition;
				Global.IsDragging = true;
			}
			if (Input.IsActionPressed("Click"))
			{
				if (Global.IsDragging == true)
				{
					GlobalPosition = GetGlobalMousePosition() - Offset;
				}

			}
			else if (Input.IsActionJustReleased("Click"))
			{
				Draggable = false;
				this.ZIndex = 0;
				Global.IsDragging = false;
				LargeCellsSlots GreatCollider = BodyRef as LargeCellsSlots;
				int LastNonDigitIndex = this.Name.ToString().LastIndexOfAny("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZéàû-".ToCharArray());
				if (IsInsideDropable && GreatCollider != null && GreatCollider.GetSensorsArraySize() < 3 && !GreatCollider.IsItemAlreadyThere(this.Name.ToString().Substring(0, LastNonDigitIndex + 1)))
				{
					if (Control.GetSelectedCell() == null)
					{
						Global.Inventory.ModifyProductQuantity(this.Name.ToString().Substring(0, LastNonDigitIndex + 1), -1);
						GreatCollider.AddItem(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
					}
					else
					{
						GreatCollider.AddItemWithoutRefreshingView(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
						GetProvenance().Delete(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
					}
				}
				else if (IsInsideSmallCollider && Control.GetSelectedCell() != null)
				{
					Global.Inventory.ModifyProductQuantity(this.Name.ToString().Substring(0, LastNonDigitIndex + 1), 1);
					Global.Inventory.AddProduct(this.Name.ToString().Substring(0, LastNonDigitIndex + 1), 1);
					GetProvenance().Delete(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
					InventoryViewScript Node = GetNode<InventoryViewScript>("../../ControlGeneral/Control/Panneau/VueInventaire");
					int CellIndex = Node.FindCell(this.Name.ToString().Substring(0, LastNonDigitIndex + 1));
					Node.LoadView();
					Node.RefreshCell(CellIndex);
				}
				else
				{
					if (Control.GetSelectedCell() == null)
					{
						var CurrentTween = GetTree().CreateTween();
						CurrentTween.TweenProperty(this, "global_position", InitialPos, 0f).SetEase(Tween.EaseType.Out);
					}
				}
				DisplaySensors();
				Control.SetSelectedCell(null);
			}
		}
	}

	/// <summary>
	/// Gets the SlotsGrandesCases that this sensor comes from, by looking at the currently selected cell.
	/// </summary>
	/// <returns>The SlotsGrandesCases that this sensor comes from.</returns>
	public LargeCellsSlots GetProvenance()
	{
		return (LargeCellsSlots)GetParent().GetNode<StaticBody2D>("GrossesCase" + Control.GetSelectedCell().Name.ToString().Substring(11));
	}

	/// <summary>
	/// Called when the mouse pointer enters the area of the sensor. If no dragging operation is in progress,
	/// it sets the draggable status to true and clears the currently selected cell in the control.
	/// </summary>
	public void OnMouseEntered()
	{
		if (!Global.IsDragging)
		{
			Control.SetSelectedCell(null);
			Draggable = true;
		}
	}

	/// <summary>
	/// Called when the mouse exits the sensor. If the object is not being dragged
	/// (i.e. Global.IsDragging is false), then the object is no longer draggable.
	/// </summary>
	public void OnMouseExited()
	{
		if (!Global.IsDragging)
		{
			Draggable = false;
		}
	}

	/// <summary>
	/// Called when the sensor enters a body. If the body is in the "GrossesCases" group, 
	/// it sets the sensor as inside a dropable area. If the body is in the "PetitesCases" 
	/// group, it sets the object as inside a small collider. The reference to the body 
	/// is stored in BodyRef.
	/// </summary>
	public void OnBodyEntered(StaticBody2D Body)
	{
		if (Body.IsInGroup("GrossesCases"))
		{
			IsInsideDropable = true;
			BodyRef = Body;
		}
		else if (Body.IsInGroup("PetitesCases"))
		{
			IsInsideSmallCollider = true;
			BodyRef = Body;
		}
	}

	/// <summary>
	/// Called when the sensor exits a body. If the body is in the "GrossesCases" group,
	/// it sets the sensor as outside a dropable area. If the body is in the "PetitesCases" 
	/// group and no other areas are overlapping, it sets the object as outside a small collider.
	/// </summary>
	public void OnBodyExited(StaticBody2D Body)
	{
		Area2D OverLappingBodies = GetNode<Area2D>("Area2D");
		if (Body.IsInGroup("GrossesCases"))
		{
			IsInsideDropable = false;
		}
		else if (Body.IsInGroup("PetitesCases"))
		{
			if (!Global.IsDragging && OverLappingBodies.GetOverlappingAreas().Count == 0)
			{
				IsInsideSmallCollider = false;
			}
		}
	}

	/// <summary>
	/// Initialises the GreatCollidersArray by adding a new SlotsGrandesCases to the list
	/// and setting it to the corresponding StaticBody2D node in the parent of the given Node.
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
	/// Deletes all the sensors nodes in the GreatCollidersArray, gets a new list of sensors nodes
	/// for each element in the array, displays the sensors, and sets the seed status to false.
	/// It also changes the text of the button in the inventory panel to "Cacher Capteurs".
	/// </summary>
	public void DisplaySensors()
	{
		for (int i = 0; i < GreatCollidersArray.Count; i++)
		{
			GreatCollidersArray[i].DeleteSensorsNodes();
			GreatCollidersArray[i].GetSensorsNodesArray();
			GreatCollidersArray[i].DisplaySensors();
			GreatCollidersArray[i].SetIsOnSeed(false);
		}
		GetParent().GetParent().GetNode<Button>("ControlGeneral/Control/Panneau/VueInventaire/Button").SetText("Cacher Capteurs");
	}
}
