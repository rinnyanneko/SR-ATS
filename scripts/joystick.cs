using Godot;
using System;
using vJoy.Wrapper;

public partial class joystick : Node
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var joystick = new VirtualJoystick(1);
		joystick.Aquire();
		GD.print(joystick);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void brake(){
		GD.print("brake");
	}
	public void release(){
		GD.print("release");
	}
	public void emergency_brake(){
		GD.print("emergency brake");
	}
}