using Godot;
using System;

public partial class CellScript : Button
{
	//ATTRIBUTS _______________________________________________________________________________________

	public bool IsPressed;
	private GpuParticles2D WaterParticles; //Noeud qui génère les particules d'eau
	private GpuParticles2D ChemicalsParticules; //Noeud qui génère les particules de pesticides
	private GpuParticles2D BordeauxMixtureParticules; //Noeud qui génère les particules de bouillie bordelaise
	public static Vector2 Adjustment; //Adjustment de la position des particules en fonction du mouvement de caméra

	//READY _______________________________________________________________________________________
	
	public override void _Ready()
	{
		IsPressed = false;
		WaterParticles = GetNode<GpuParticles2D>("../../../ControlParticules/GpuParticlesEau");
		ChemicalsParticules = GetNode<GpuParticles2D>("../../../ControlParticules/GpuParticlesPesticide");
	    BordeauxMixtureParticules = GetNode<GpuParticles2D>("../../../ControlParticules/GpuParticlesBouillie");
		Adjustment = new Vector2(0,0);
	}

	//PROCESS _______________________________________________________________________________________
	
	public override void _Process(double delta){
		if(WaterParticles is not null && WaterParticles.Emitting){
			WaterParticles.Position = GetViewport().GetMousePosition()+Adjustment;
		}
		else if(ChemicalsParticules is not null && ChemicalsParticules.Emitting){
			ChemicalsParticules.Position = GetViewport().GetMousePosition()+Adjustment;
		}
		else if(BordeauxMixtureParticules is not null && BordeauxMixtureParticules.Emitting){
			BordeauxMixtureParticules.Position = GetViewport().GetMousePosition()+Adjustment;
		}
	}

	//SIGNAUX _________________________________________________________________________________________

	public void OnButtonDown(){
		IsPressed = true;
		if(WaterParticles is not null && ControlScript.ActionCursor=="arrosoir"){
			WaterParticles.Emitting = true;
		}
		if(ChemicalsParticules is not null && ControlScript.ActionCursor=="pesticide"){
			ChemicalsParticules.Emitting = true;
		}
		if(BordeauxMixtureParticules is not null && ControlScript.ActionCursor=="bouillie"){
			BordeauxMixtureParticules.Emitting = true;
		}
	}
	public void OnButtonUp(){
		IsPressed = false;
		if(WaterParticles is not null && WaterParticles.Emitting){
			WaterParticles.Emitting = false;
		}
		else if(ChemicalsParticules is not null && ChemicalsParticules.Emitting){
			ChemicalsParticules.Emitting = false;
		}
		else if(BordeauxMixtureParticules is not null && BordeauxMixtureParticules.Emitting){
			BordeauxMixtureParticules.Emitting = false;
		}
	}
}
