using Godot;
using System;

public partial class TestCat : CharacterBody3D
{
	
	private CharacterBody3D _player;
	private CharacterBody3D _catBody;
	private Vector3 velocity;
	
	public override void _Ready()
	{
		_player = GetNode<CharacterBody3D>("%fpsCamera");
		//velocity = Vector3.Zero;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var targetPos = _player.GetGlobalPosition();
		targetPos.Y = this.GetGlobalPosition().Y; //rotation lock would be better...
		var targetTransform = this.Transform.LookingAt(targetPos, Vector3.Up);
		this.Transform = this.Transform.InterpolateWith(targetTransform, 5 * (float)delta);
		
		/*velocity.X = Mathf.MoveToward(this.GetGlobalPosition().X, _player.GetGlobalPosition().X, (float)delta);
		velocity.Z = Mathf.MoveToward(this.GetGlobalPosition().Z, _player.GetGlobalPosition().Z, (float)delta);
		velocity.Y = Mathf.MoveToward(this.GetGlobalPosition().Y, _player.GetGlobalPosition().Y, (float)delta);
		
		Velocity = velocity;
		MoveAndSlide();*/
	}
}
