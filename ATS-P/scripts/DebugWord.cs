// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;

public partial class DebugWord : RichTextLabel
{
    private ConfigFile _cfg = new ConfigFile();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("System", "Debug", false).AsBool())
            Visible = true;
        else
            Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("System", "Debug", false).AsBool())
        {
            var parent = GetParent();
            if (parent != null)
            {
                Text = $"UpdateTime = {parent.Get("UpdateTime")}\n" +
                       $"Brake = {parent.Get("Brake")}\n" +
                       $"EmBrake = {parent.Get("EmBrake")}\n" +
                       $"BrakeOpen = {parent.Get("BrakeOpen")}\n" +
                       $"PatternSpeed = {parent.Get("PatternSpeed")}\n" +
                       $"Fail = {parent.Get("Fail")}\n" +
                       $"SignalInFront = {parent.Get("SignalInFront")}\n" +
                       $"VDDelayedTimetableIndex = {parent.Get("VDDelayedTimetableIndex")}\n" +
                       $"SignalInFrontSpeed = {parent.Get("SignalInFrontSpeed")}\n" +
                       $"DistanceToSignalInFront = {parent.Get("DistanceToSignalInFront")}\n" +
                       $"Velocity = {parent.Get("Velocity")}\n" +
                       $"Longitude = {parent.Get("Longitude")}\n" +
                       $"Latitude = {parent.Get("Latitude")}\n" +
                       $"InBorderStationArea = {parent.Get("InBorderStationArea")}\n" +
                       $"ControlledBySteamID = {parent.Get("ControlledBySteamID")}";
            }
        }
    }
}