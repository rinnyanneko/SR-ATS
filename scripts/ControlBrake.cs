// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

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
			var prt = "";
			joystick = new vJoy();
			GD.Print("vJoyEnabled: " + joystick.vJoyEnabled());
			// Get the driver attributes (Vendor ID, Product ID, Version Number)
			if (!joystick.vJoyEnabled())
			{
				GD.PrintErr("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
				return;
			}
			else
				GD.Print("Vendor: "+joystick.GetvJoyManufacturerString()+"\nProduct: "+joystick.GetvJoyProductString()+"\nVersion Number: "+joystick.GetvJoySerialNumberString()+"\n");
			// Test if DLL matches the driver
			UInt32 DllVer = 0, DrvVer = 0;
			bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
			if (match)
				GD.Print("Version of Driver Matches DLL Version ("+DllVer.ToString("X")+")\n");
			else
				GD.PrintErr("Version of Driver ("+DrvVer.ToString("X")+") does NOT match DLL Version ("+DllVer.ToString("X")+")\n");
			// Get the state of the requested device
			VjdStat status = joystick.GetVJDStatus(id);
			switch (status)
			{
			case VjdStat.VJD_STAT_OWN:
				GD.PrintErr("vJoy Device "+id+" is already owned by this feeder\n");
				break;
			case VjdStat.VJD_STAT_FREE:
				GD.Print("vJoy Device "+id+" is free\n");
				break;
			case VjdStat.VJD_STAT_BUSY:
				GD.PrintErr("vJoy Device "+id+" is already owned by another feeder\nCannot continue\n");
				return;
			case VjdStat.VJD_STAT_MISS:
				GD.PrintErr("vJoy Device "+id+" is not installed or disabled\nCannot continue\n");
				return;
			default:
				GD.Print("vJoy Device "+id+" general error\nCannot continue\n");
				return;
			};
			///// vJoy Device properties
			int nBtn = joystick.GetVJDButtonNumber(id);
			int nDPov = joystick.GetVJDDiscPovNumber(id);
			int nCPov = joystick.GetVJDContPovNumber(id);
			bool X_Exist = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
			bool Y_Exist = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
			bool Z_Exist = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
			bool RX_Exist = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
			prt = String.Format("Device[{0}]: Buttons={1}; DiscPOVs:{2}; ContPOVs:{3}", id, nBtn, nDPov, nCPov);
			GD.Print(prt);
			// Acquire the target
			if ((status == VjdStat.VJD_STAT_OWN) ||
					((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
				prt = String.Format("Failed to acquire vJoy device number {0}.", id);
			else
				prt = String.Format("Acquired: vJoy device number {0}.", id);
			GD.Print(prt);

			EmitSignal(SignalName.BrakeReady);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}
    }
	public void _on_tree_exiting(){
		joystick.RelinquishVJD(id);
		GD.Print("Exiting Tree");
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