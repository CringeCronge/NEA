using Godot;
using System;

public partial class TestCat : CharacterBody3D
{
	
	private CharacterBody3D _player;
	private CharacterBody3D _catBody;
	
	public override void _Ready()
	{
		_player = GetNode<CharacterBody3D>("%fpsCamera");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var targetPos = _player.GetGlobalPosition();
		targetPos.Y = this.GetGlobalPosition().Y; //rotation lock would be better...
		var targetTransform = this.Transform.LookingAt(targetPos, Vector3.Up);
		this.Transform = this.Transform.InterpolateWith(targetTransform, 5 * (float)delta);
		
		/*Velocity = MoveToward(_player.GetGlobalPosition(), delta);
		MoveAndSlide();*/
	}
}
