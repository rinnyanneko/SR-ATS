// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

using Godot;

public partial class DebugWord : RichTextLabel{
    private ConfigFile _cfg = new ConfigFile();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        if (_cfg.GetValue("System", "Debug", false).AsBool()){
            Visible = true;
        }
        else{
            Visible = false;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("System", "Debug", false).AsBool()){
            Visible = true;
            Scene parent = GetNode<Scene>("..");

            Text = "UpdateTime = " + parent.UpdateTime.ToString() + "\n" +
                   "pattern_speed = " + parent.PatternSpeed.ToString() + "\n" +
                   "Brake = " + parent.Brake.ToString() + "\n" +
                   "EmBrake = " + parent.EmBrake.ToString() + "\n" +
                   "BrakeOpen = " + parent.BrakeOpen.ToString() + "\n" +
                   "PatternSpeed = " + parent.PatternSpeed.ToString() + "\n" +
                   "Fail = " + parent.Fail.ToString() + "\n" +
                   "SignalInFront = " + parent.SignalInFront + "\n" +
                   "VDDelayedTimetableIndex = " + parent.VDDelayedTimetableIndex.ToString() + "\n" +
                   "SignalInFrontSpeed = " + parent.SignalInFrontSpeed.ToString() + "\n" +
                   "DistanceToSignalInFront = " + parent.DistanceToSignalInFront.ToString() + "\n" +
                   "Velocity = " + parent.Velocity.ToString() + "\n" +
                   "Longitute = " + parent.Longitute.ToString() + "\n" +
                   "Latititute = " + parent.Latititute.ToString() + "\n" +
                   "InBorderStationArea = " + parent.InBorderStationArea.ToString() + "\n" +
                   "ControlledBySteamID = " + parent.ControlledBySteamID;
        }
        else{
            Visible = false;
        }
    }
}