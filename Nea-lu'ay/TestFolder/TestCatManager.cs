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
		//GD.Print("boidArray: " + boidArray);
	}
	
	public void SetBoidArray()
	{
		Godot.Collections.Array<Node> tempArray = GetTree().GetNodesInGroup("Cat");
		boidArray.Clear();
		for (int i = 0; i < tempArray.Count; i++)
		{
			string nodePath = (string)tempArray[i].GetPath();
			//GD.Print("tempArray: " + tempArray[i] + ", node path: " + nodePath + ", i: " + i);
			boidArray.Add(GetNode<CharacterBody3D>(nodePath));
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 targetPos = targetBoid.GetGlobalPosition();
		Vector3 targetVelocity = targetBoid.Velocity;
		Vector3 finalVelocity = Vector3.Zero;
		
		for (int i = 0; i < boidArray.Count; i++)
		{
			Vector3 currentPos = boidArray[i].GetGlobalPosition();
			//GD.Print(currentPos);
			
			Vector3 centerCorrection = Rule1(currentPos, targetPos);
			//GD.Print("Rule 1, Boid:" + i + " ," + centerCorrection);
			Vector3 objectAvoidence = Rule2(boidArray[i], currentPos);
			//GD.Print("Rule 2, Boid:" + i + " ," + objectAvoidence);
			Vector3 velocityCorrection = Rule3(boidArray[i], targetVelocity);
			//GD.Print("Rule 3, Boid:", i + " ," + velocityCorrection);
			
			finalVelocity = centerCorrection + objectAvoidence + velocityCorrection;
			//GD.Print("Boid ", i, "velocity: ", finalVelocity);
			boidArray[i].Velocity = finalVelocity;
		}
	}
	
	public Vector3 Rule1(Vector3 boidPos, Vector3 targetPos)
	{
		//I might want to touch the Y magnatude, as this might cause them to fly...
		return new Vector3 (((targetPos.X-boidPos.X)*0.7f), ((targetPos.Y-boidPos.Y)*0.7f), ((targetPos.Z-boidPos.Z)*0.7f));
	}
	
	public Vector3 Rule2(CharacterBody3D boid, Vector3 boidPos) //I don't think this does enough...
	{
		Vector3 avoidance = Vector3.Zero;
		
		for(int i = 0; i < boidArray.Count; i++)
		{
			if(boid != boidArray[i])
			{
				Vector3 temp = boidArray[i].GetGlobalPosition();
				if ((Mathf.Sqrt(Mathf.Pow((boidPos.X+temp.X), 2)+Mathf.Pow((boidPos.Z+temp.Z), 2))) < 0.25f)
				{
					avoidance -= boidPos - temp;
				}
			}
		}
		
		return avoidance;
	}
	
	public Vector3 Rule3(CharacterBody3D boid, Vector3 targetVelocity)
	{
		Vector3 newVelocity = Vector3.Zero;
		
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
