using Godot;
using System;

public partial class FpsCamera : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	
	[Export]
	public float CameraSensitivity = 0.006f;
	private bool mouseCaptured = true;
	
	[Export]
	public float SprintMultiplier = 1.4f;
	private bool _sprinting = false;
	private bool _tripping = false;
	public float eventChance = 0.0f;
	Godot.Collections.Array<ulong> stumbleQueue = [0, 0, 0,];
	
	private bool hasController = false;
	
	private Node3D _cameraPivot;
	private Camera3D _playerCamera;
	private CollisionShape3D _player;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_cameraPivot = GetNode<Node3D>("Pivot");
		_playerCamera = GetNode<Camera3D>("Pivot/PlayerCamera");
		_player = GetNode<CollisionShape3D>("CollisionShape3D");
		_playerCamera.Fov = 85;
	}
	
	//Mouse input and sprinting.
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseMotion m)
		{
			/*_cameraPivot.*/RotateY(-m.Relative.X * CameraSensitivity);
			_playerCamera.RotateX(-m.Relative.Y * CameraSensitivity);
			
			Vector3 cameraRotation = _playerCamera.Rotation;
			cameraRotation.X = Mathf.Clamp(cameraRotation.X, -((4.0f * Mathf.Pi)/9.0f), ((4.0f * Mathf.Pi)/9.0f));
			_playerCamera.Rotation = cameraRotation;
		}
		else if(@event is InputEventKey k && k.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			mouseCaptured = false;
			GD.Print("Uncaptured");
		}
		else if(!mouseCaptured && @event is InputEventKey)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured; mouseCaptured = true; GD.Print("Recaptured");
		}
		
		//Joystick:
		if(!hasController)
		{
			if(@event is InputEventJoypadMotion || @event is InputEventJoypadButton)
			{
				hasController = true;
			}
		}
		
		if(Input.IsActionJustPressed("sprint"))
		{
			Tween t = GetTree().CreateTween();
			switch(_sprinting) //this allows for togglable sprinting!
			{
				case true: 
					_sprinting = false;
					t.TweenProperty(_playerCamera, "fov", 75, 0.2f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
					break;
				case false:
					_sprinting = true; 
					t.TweenProperty(_playerCamera, "fov", 90, 0.2f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
					break;
			}
		}
	}

//On every frame update, currently right joystick movement.
	public override void _Process(double delta)
	{
		if(hasController)//this might break tripping...
		{
			Vector2 inputDir = Input.GetVector("look_left", "look_right", "look_up", "look_down");
			_cameraPivot.RotateY(-inputDir.X * 0.01f);
			/*_playerCamera.*/RotateX(-inputDir.Y * 0.01f);
			
			Vector3 cameraRotation = _playerCamera.Rotation;
			cameraRotation.X = Mathf.Clamp(cameraRotation.X, -((4.0f * Mathf.Pi)/9.0f), ((4.0f * Mathf.Pi)/9.0f));
			_playerCamera.Rotation = cameraRotation;
		}
	}
	
	public void Trip()
	{
		GD.Print("Trip!");
		_tripping = true;
		_sprinting = false;
		Velocity += new Vector3(0,3,0);
		RotateObjectLocal(new Vector3(1, 0, 0), -Mathf.Pi/2.0f);
	}
	
	public void Stumble()
	{
		stumbleQueue.RemoveAt(0);
		stumbleQueue.Resize(3);// why does't append work??
		stumbleQueue[2] = Time.GetTicksMsec();
		GD.Print(stumbleQueue[2]-stumbleQueue[0]);
		
		eventChance = GD.Randf();
		if( ((stumbleQueue[2] - stumbleQueue[0]) <= (ulong)30000) && (eventChance >= 0.7f) )
		{
			Trip();
			GD.Print("Stumble caused trip!");
		}
	}
	
	//every physics frame
	public override void _PhysicsProcess(double delta)
	{
		/*movement*/
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 1.3f;// fine tune gravity.
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			if(_tripping)
			{
				_tripping = false;
				Tween t = GetTree().CreateTween();
				t.TweenProperty(_playerCamera, "fov", 75, 0.2f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
				RotateObjectLocal(new Vector3(1, 0, 0), Mathf.Pi/2.0f);
			}
		}

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (_cameraPivot.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		//float speed = _sprinting ? Speed * SprintMultiplier: Speed;
		float speed = _tripping? 0:(!IsOnFloor() ? (_sprinting ? 0.4f * SprintMultiplier * Speed :0.45f * Speed ):(_sprinting ? Speed * SprintMultiplier: Speed));
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}
		
		

		Velocity = velocity;
		MoveAndSlide();
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			if(((Node)collision.GetCollider()).IsInGroup("TripHazard") && !_tripping)
			{
				if(_sprinting)
				{
					Trip();
				}
				else if(Velocity != new Vector3(0,0,0))
				{
					eventChance = GD.Randf();
					if(eventChance > 0.95)
					{
						GD.Print(eventChance+","+Velocity);
						Trip();
					}
				}
			}
			else if(((Node)collision.GetCollider()).IsInGroup("StumbleHazard"))
			{
				eventChance = GD.Randf();
				if(_sprinting && eventChance >= 0.97)
				{
					GD.Print(eventChance+", sprinting");
					Stumble();
				}
				else if(Velocity != new Vector3(0,0,0) && eventChance >= 0.99)//the vector may not include negative velocity...
				{
					GD.Print(eventChance+","+Velocity);
					Stumble();
				}
			}
		}
	}
}
