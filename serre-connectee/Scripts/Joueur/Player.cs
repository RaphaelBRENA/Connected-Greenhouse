using Godot;
using System;
using System.Linq;

public partial class Player : CharacterBody3D
{
	// Export Variables
	[Export] public float InteractDistance = 2.0f;

	// Classic Variables
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 3.0f;
	public Node InteractCastResult = new Node();
	public Camera3D Camera;


	// Fonctions
    public override void _Ready()
    {
		PopupManager.ShowPopup("Bienvenue dans votre serre ! Vous allez pouvoir faire pousser un tas de plantes ! Commencez par explorer les différentes parties de la serre !");
        
		base._Ready();
		//Input.MouseMode = Input.MouseModeEnum.Captured;
		GetViewport().WarpMouse(GetViewport().GetVisibleRect().Size / 2);
		
		Rotation = Global.RotationPlayer;

		Camera = GetNode<Camera3D>("Camera3D");
		Camera.Rotation = Global.RotationCamera;

		this.GlobalPosition = Global.Coordinates;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 InputDirection = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 Direction = (Transform.Basis * new Vector3(InputDirection.X, 0, InputDirection.Y)).Normalized();
		if (Direction != Vector3.Zero)
		{
			velocity.X = Direction.X * Speed;
			velocity.Z = Direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		InteractCast();
		Global.Coordinates = this.Position;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if(@event is InputEventMouseMotion)
		{
			InputEventMouseMotion Motion = @event as InputEventMouseMotion;
			Rotation = new Vector3(Rotation.X, Rotation.Y - Motion.Relative.X / 1000 * Sensitivity, Rotation.Z);
			Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X - Motion.Relative.Y / 1000 * Sensitivity, -1.5f, 1.5f), Camera.Rotation.Y, Camera.Rotation.Z);
			Global.RotationPlayer = Rotation;
			Global.RotationCamera = Camera.Rotation;
		}
		if(@event.IsActionPressed("Interact")) {
			if(InteractCastResult is not null && InteractCastResult.HasUserSignal("interacted"))
				InteractCastResult.EmitSignal("interacted");
		}
	}
	
	public void InteractCast()
	{
		
		Camera3D Camera = GetNode<Camera3D>("Camera3D");
		var SpaceState = Camera.GetWorld3D().DirectSpaceState;
		var ScreenCenter = GetViewport().GetVisibleRect().Size / 2;
		var Origin = Camera.ProjectRayOrigin(ScreenCenter);
		var End = Origin + Camera.ProjectRayNormal(ScreenCenter) * InteractDistance;
		var Query = PhysicsRayQueryParameters3D.Create(Origin, End);
		Query.CollideWithBodies = true;
		var Result = SpaceState.IntersectRay(Query);
		Godot.Node CurrentCastResult = new Node();

		if (Result.Count > 0){
			CurrentCastResult = (Godot.Node) Result["collider"];
			if (CurrentCastResult.ToString() != InteractCastResult.ToString())
				InteractCastResult = (Godot.Node) CurrentCastResult;
				if (InteractCastResult is not null && InteractCastResult.HasUserSignal("focused")){
					InteractCastResult.EmitSignal("focused");
					GridControlScript.SetColliderName((string)Result["collider"]);
				}
			else {
				Global.UiContext.Reset();
				InteractCastResult = new Node();
			}
		} else {
			Global.UiContext.Reset();
			InteractCastResult = new Node();
		}
	}

}
