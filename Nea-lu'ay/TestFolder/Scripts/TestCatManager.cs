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
			Vector3 currentPos = boidArray[i].GetGlobalPosition() + new Vector3(0, 0.5f, 0);
			//GD.Print(currentPos);
			
			Vector3 centerCorrection = Rule1(currentPos, targetPos);
			//GD.Print("Rule 1, Boid:" + i + " ," + centerCorrection);
			Vector3 objectAvoidence = Rule2(boidArray[i], currentPos, targetPos);
			//GD.Print("Rule 2, Boid:" + i + " ," + objectAvoidence);
			Vector3 velocityCorrection = Rule3(boidArray[i], targetVelocity);
			//GD.Print("Rule 3, Boid:", i + " ," + velocityCorrection);
			
			finalVelocity =  centerCorrection + objectAvoidence + velocityCorrection;
			boidArray[i].Velocity = finalVelocity;
			
		}
	}
	
	///<summary>This provides an updated postion for centre correction.</summary>
	public Vector3 Rule1(Vector3 boidPos, Vector3 targetPos)
	{
		return new Vector3 (((targetPos.X-boidPos.X)*0.5f), ((targetPos.Y-boidPos.Y)), ((targetPos.Z-boidPos.Z)*0.5f));
	}
	
	/// <summary>
	/// This checks if another boid is within 25cm, and will provide an updated boid to avoid them.
	/// </summary>
	/// <param name="boid"></param>
	/// <param name="boidPos"></param>
	/// <param name="playerPos"></param>
	/// <returns></returns>
	public Vector3 Rule2(CharacterBody3D boid, Vector3 boidPos, Vector3 playerPos) //I don't think this does enough...
	{
		Vector3 avoidance = Vector3.Zero;
		
		for(int i = 0; i < boidArray.Count; i++)
		{
			if(boid != boidArray[i])
			{
				Vector3 temp = boidArray[i].GetGlobalPosition();
				
				if(Mathf.Sqrt(Mathf.Pow((boidPos.X-temp.X), 2)+Mathf.Pow((boidPos.Z-temp.Z), 2)) < 0.6f)
				{
					avoidance = avoidance - boidPos - temp;
				}
			}
			
		}
		
		if(Mathf.Sqrt(Mathf.Pow((boidPos.X-playerPos.X), 2)+Mathf.Pow((boidPos.Z-playerPos.Z), 2)) < 2)
		{
			avoidance = 0.3f*(avoidance - boidPos - playerPos);
		}
		
		avoidance.Y = 0;
		
		return avoidance;
	}
	
	/// <summary>
	/// This matches boid velocity with other boid velocities.
	/// </summary>
	/// <param name="boid"></param>
	/// <param name="playerVelocity"></param>
	/// <returns></returns>
	public Vector3 Rule3(CharacterBody3D boid, Vector3 playerVelocity)
	{
		Vector3 newVelocity = new Vector3 (0f, playerVelocity.Y, 0f);
		
		for(int i = 0; i < boidArray.Count; i++)
		{
			if(boid != boidArray[i])
			{
				newVelocity += boidArray[i].Velocity;
			}
		}
		
		return ((newVelocity/(boidArray.Count-1))-boid.Velocity)/8.0f;
	}

	/// <summary>
	/// This is just a place holder.
	/// </summary>
	/// <param name="boid"></param>
	/// <param name="playerPos"></param>
	/// <returns></returns>
	public Vector3 Rule4(CharacterBody3D boid, Vector3 playerPos)
	{
		
		
		return Vector3.Zero;
	}
}
