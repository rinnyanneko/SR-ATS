// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

using Godot;

public partial class Scene : Node2D{
    public bool Brake = false;
    public bool EmBrake = false;
    public bool BrakeOpen = false;
    public double PatternSpeed = 32767;
    public bool Fail = false;
    public string SignalInFront = "";
    public double VDDelayedTimetableIndex = -1.0;
    public int SignalInFrontSpeed = -1;
    public double DistanceToSignalInFront = -1.0;
    public int Velocity = -1;
    public double Longitute = -1.0;
    public double Latititute = -1.0;
    public bool InBorderStationArea = false;
    public string ControlledBySteamID = "";
    public string UpdateTime;

    public bool ApproachPattern = false;
    public bool ATSp = false;
    public bool Ppower = false;
}