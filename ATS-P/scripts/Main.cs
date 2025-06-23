// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node{
    private Indicators indicators;
    private Scene parent;
    private ControlBrake controlBrake;
    private ConfigFile _cfg = new ConfigFile();

    [Signal]
    public delegate void ATSReadyEventHandler();
     public override void _Notification(int what){
        if (what == 2016){ //NOTIFICATION_APPLICATION_FOCUS_IN
            Engine.MaxFps = 0; // Zero means uncapped
            OS.LowProcessorUsageMode = false;
            GetTree().Paused = false;
        }
        if (what == 2017){ //NOTIFICATION_APPLICATION_FOCUS_OUT
            Engine.MaxFps = 20; // Zero means uncapped
            OS.LowProcessorUsageMode = true;
            GetTree().Paused = true;
        }
    }
    public override async void _Ready(){
        try{
            parent = GetNode<Scene>("..");
            indicators = GetNode<Indicators>("../Indicators");
            indicators.Ppower(true);
            indicators.Fail(true);
            indicators.PlayBell();
            parent.Fail = true;
            indicators.PlayBell();
            await Task.Delay(3000);
            indicators.Fail(false);
            parent.Fail = false;
            indicators.PlayBell();
            EmitSignal(SignalName.ATSReady);
            await Task.Delay(3000);
            while (parent.DistanceToSignalInFront > 500 && parent.DistanceToSignalInFront > 0){
                await Task.Delay(2000);
            }
            indicators.ATSp(true);
            indicators.PlayBell();
        }catch(Exception e){
            GD.PrintErr(e);
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        parent = GetNode<Scene>("..");
        indicators = GetNode<Indicators>("../Indicators");
        controlBrake = GetNode<ControlBrake>("../ControlBrake");
        double PatternSpeed = CalculateBrakePatternSpeed();
        if (parent.Velocity < PatternSpeed - 5 && parent.Brake){
            controlBrake.release();
            indicators.Brake(false);
            indicators.PlayBell();
            indicators.ApproachPattern(false);
            parent.ApproachPattern = false;
            parent.Brake = false;
        }
        else if (parent.Velocity < PatternSpeed - 5 && parent.ApproachPattern){
            parent.ApproachPattern = false;
            indicators.ApproachPattern(false);
            indicators.PlayBell();
        }
        if (parent.Velocity >= PatternSpeed - 5 && parent.Velocity <= PatternSpeed){
            parent.ApproachPattern = true;
            indicators.ApproachPattern(true);
            indicators.PlayBell();
        }
        else if (parent.Velocity > PatternSpeed && !parent.Brake){
            parent.Brake = true;
            indicators.Brake(true);
            indicators.PlayBell();
            controlBrake.brake();
        }
        //TODO: Implement the logic for the brake system and indicators
    }
    private double CalculateBrakePatternSpeed(){
        parent = GetNode<Scene>("..");
        _cfg.Load("user://config.cfg");
        double speed = parent.Velocity;
        double distance = parent.DistanceToSignalInFront;
        double decelRate = _cfg.GetValue("Train Data", "decelRate", 0.5).AsDouble();
        int vmax = _cfg.GetValue("Train Data", "Vmax", 100).AsInt16();
        double brakeDistance = Math.Pow(speed, 2) / (2 * decelRate);
        if (parent.SignalInFrontSpeed > vmax){
            return vmax + 5;
        }
        //TODO: Implement the logic for the brake pattern speed
        return 32767;
    }
}
