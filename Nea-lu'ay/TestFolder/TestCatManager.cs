using Godot;
using System;

public partial class TestCatManager : Node
{
	private CharacterBody3D targetBoid;
	private Godot.Collections.Array<CharacterBody3D> boidArray = [null, null, null, null, null];
	
	public override void _Ready()
	{
		targetBoid = GetNode<CharacterBody3D>("%fpsCamera");
		SetBoidArray();
		GD.Print("boidArray: " + boidArray);
	}
	
	public void SetBoidArray()
	{
		Godot.Collections.Array<Node> tempArray = GetTree().GetNodesInGroup("Cat");
		boidArray.Clear();
		GD.Print("tempArray: " + tempArray);
		for (int i = 0; i < tempArray.Count; i++)
		{
			string nodePath = (string)tempArray[i].GetPath();
			GD.Print("tempArray: " + tempArray[i] + ", node path: " + nodePath + ", i: " + i);
			boidArray.Add(GetNode<CharacterBody3D>(nodePath));
			GD.Print("boidArray: " + boidArray + ", i: " + i);
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 targetPos = targetBoid.GetGlobalPosition();
		Vector3 targetVelocity = targetBoid.Velocity;
		
		for (int i = 0; i < boidArray.Count; i++)
		{
			Vector3 centerCorrection = Rule1(boidArray[i].GetGlobalPosition(), targetPos);
			GD.Print("Rule 1, Boid:" + i + " ," + centerCorrection);
			Vector3 objectAvoidence = Rule2(boidArray[i]);
			GD.Print("Rule 2, Boid:" + i + " ," + objectAvoidence);
			Vector3 velocityCorrection = Rule3(boidArray[i], targetVelocity);
			GD.Print("Rule 3, Boid:", i + " ," + velocityCorrection);
		}
		
		/*Velocity = 
		MoveAndSlide();*/
	}
	
	public Vector3 Rule1(Vector3 boidPos, Vector3 targetPos)
	{
		//I might want to touch the Y magnatude, as this might cause them to fly...
		return new Vector3 ((targetPos.X-boidPos.X)/100, (targetPos.Y-boidPos.Y)/100, (targetPos.Z-boidPos.Z)/100);
	}
	
	public Vector3 Rule2(CharacterBody3D boid)
	{
		Vector3 avoidance = Vector3.Zero;
		
		for(int i = 0; i < boidArray.Count; i++)
		{
			if(boid != boidArray[i])
			{
				avoidance -= boid.GetGlobalPosition() - boidArray[i].GetGlobalPosition();
			}
		}
		
		return avoidance;
	}
	
	public Vector3 Rule3(CharacterBody3D boid, Vector3 targetVelocity)
	{
		Vector3 newVelocity = targetVelocity;
		
		for(int i = 0; i < boidArray.Count; i++)
		{
			if(boid != boidArray[i])
			{
				newVelocity += boidArray[i].Velocity;
			}
		}
		
		return ((newVelocity/(boidArray.Count-1))-boid.Velocity)/8.0f;
	}
}
