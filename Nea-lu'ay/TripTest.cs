using Godot;
using System;

public partial class TripTest : CsgMesh3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	[Signal]
	public delegate void TripChanceEventHandler(bool isSprinting);
	
	/*public void TripCalc(bool isSprinting)
	{
		if(isSprinting)
		{
			CharacterBody3D Player = GetNode<CharacterBody3D>("fpsCamera");
			Player.Trip();
		}
		else
		{
			GD.Print("chance");
		}
	}*/
}
