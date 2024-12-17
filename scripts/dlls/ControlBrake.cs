using Godot;
using System;
using vJoyInterfaceWrap;

public partial class ControlBrake: Node
{
    [Signal]
    public delegate void BrakeReadyEventHandler();

    vJoy joystick = new vJoy();
	uint id = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		try
		{
			GD.Print("vJoyEnabled: " + joystick.vJoyEnabled());
			/////	Write access to vJoy Device - Basic
			VjdStat status;
			var prt = "";
			status = joystick.GetVJDStatus(id);
			// Acquire the target
			if ((status == VjdStat.VJD_STAT_OWN) ||
					((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
				prt = String.Format("Failed to acquire vJoy device number {0}.", id);
			else
				prt = String.Format("Acquired: vJoy device number {0}.", id);
			GD.Print(prt);
			GD.Print("finished _Ready()");
			EmitSignal(SignalName.BrakeReady);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}
        }
	public void _on_tree_exiting(){
		GD.Print("Exiting");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void brake(){
		GD.Print("brake");
	}
	public void release(){
		GD.Print("release");
	}
	public void emergency_brake(){
		GD.Print("emergency brake");
	}
}