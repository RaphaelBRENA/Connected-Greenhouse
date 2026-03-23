using Godot;
using System;
using System.Collections.Generic;

public partial class PotInventoryCellScript : Button
{

	//ATTRIBUTS _______________________________________________________________________________________

	private string Item;
	private int Quantity;
	private Label DisplayNode;
	private Texture2D Image; //Image de la Node
	Node2D Node2D;
	static int Counter = 0;


	//READY ___________________________________________________________________________________________

	public override void _Ready(){
		Item = "";
		Quantity = 0;
		DisplayNode = GetNode<Label>("../../Selection");
		Image = null;
		Node2D = new Node2D();
	}

	//PROCESS __________________________________________________________________________________________

	public override void _Process(double delta){}

	//METHODES _________________________________________________________________________________________

	public void LoadCell(string i, int q){
			Item = i;
			Quantity = q;
		if(Item != ""){
			if(IsInstanceValid(Node2D))
			{
				Node2D.QueueFree();
			}
			if(Item.Length>8 && Item.Substring(0,7)=="Graines"){
				Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+(Item.Substring(8,Item.Length-8))+"Graine.png");
			}
			else{
				Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+Item+".png");
			}
		}
	}

	public void VideCase(){
		Item = "";
		Quantity = 0;
		if(IsInstanceValid(Node2D))
		{
			Node2D.QueueFree();
		}
	}

	public string GetItem(){
		return Item;
	}

	public int GetQuantity(){
		return Quantity;
	}


	public void Affichage(){
		if(Item!=""){
			DisplayNode.SetText(Item+" : "+Quantity.ToString());
		}else{
			DisplayNode.SetText("");
		}
	}


	public void ChargerNode2D(int Position){
		
		if(Item.Substring(0,7)=="Graines"){

			Counter++;
			PackedScene Scene = (PackedScene)GD.Load("res://Scenes/Interface/PlanDeTerre/DragAndDrop/Graines.tscn");
			Node2D = (Node2D)Scene.Instantiate();

			if(Position < 6)
			{
				Node2D.Position = new Vector2(944+Position*65, 353);
			}
			else
			{
				Node2D.Position = new Vector2(944+(Position-6)*65, 412);
			}
			Sprite2D sprite = new Sprite2D();
            Texture2D CurrentTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/"+Item.Substring(8)+"Graine.png");
            sprite.Texture = CurrentTexture;
			sprite.Scale = new Vector2((float)0.14, (float)0.14);
			Node2D.AddChild(sprite);
			Node2D.Name = Item+Counter;
			Node2D.AddToGroup("Graines");
			GetParent().GetParent().GetParent().GetParent().GetParent().GetParent().GetNode<Node2D>("DragAndDrop").AddChild(Node2D);
		}
		else
		{
			Counter++;
			PackedScene Scene = (PackedScene)GD.Load("res://Scenes/Interface/PlanDeTerre/DragAndDrop/Capteurs.tscn");
			Node2D = (Node2D)Scene.Instantiate();
			if(Position<6){
				Node2D.Position = new Vector2(944+Position*65, 506);
			}
			
			Sprite2D sprite = new Sprite2D();
            Texture2D CurrentTexture = (Texture2D)GD.Load("res://Assets/Images/ImagesObjets/"+Item+".png");
            sprite.Texture = CurrentTexture;
			sprite.Scale = new Vector2((float)0.14, (float)0.14);
			Node2D.AddChild(sprite);
			Node2D.Name = Item+Counter;
			Node2D.AddToGroup("Capteurs");
			GetParent().GetParent().GetParent().GetParent().GetParent().GetParent().GetNode<Node2D>("DragAndDrop").AddChild(Node2D);
		}
	}

	//SIGNAUX _________________________________________________________________________________________

	public void OnCellPressed(){
		Affichage();
	}

}

	