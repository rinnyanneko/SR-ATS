// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node {
    private ATSIndicators indicators;
    private Scene parent;
    private ControlBrake controlBrake;
    private ConfigFile _cfg = new ConfigFile();
    
    private bool isReady = false;

    [Signal]
    public delegate void ATSReadyEventHandler();

    
    public override void _Notification(int what) {
        if (what == NotificationApplicationFocusIn)
            Engine.MaxFps = 0; // Zero means uncapped
        if (what == NotificationApplicationFocusOut)
            Engine.MaxFps = 20;
    }

    public override async void _Ready() {
        try {
            parent = GetNode<Scene>("..");
            indicators = GetNode<ATSIndicators>("../Indicators");
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
            while (parent.DistanceToSignalInFront > 500 && parent.DistanceToSignalInFront > 0) {
                await Task.Delay(2000);
            }
            indicators.BrakeOpen(true); // REMOVE WHEN BRAKE IMPLEMENTED
            indicators.ATSp(true);
            indicators.PlayBell();
            this.isReady = true;
        }
        catch (Exception e) {
            GD.PrintErr(e);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        if (!isReady) {
            return; // Exit if ATS is not ready
        }
        parent = GetNode<Scene>("..");
        indicators = GetNode<ATSIndicators>("../Indicators");
        controlBrake = GetNode<ControlBrake>("../ControlBrake");
        double PatternSpeed = CalculateBrakePatternSpeed();
        if (parent.Velocity < PatternSpeed - 5 && parent.Brake) {
            controlBrake.release();
            indicators.Brake(false);
            indicators.PlayBell();
            indicators.ApproachPattern(false);
            parent.ApproachPattern = false;
            parent.Brake = false;
        }
        else if (parent.Velocity < PatternSpeed - 5 && parent.ApproachPattern) {
            parent.ApproachPattern = false;
            indicators.ApproachPattern(false);
            indicators.PlayBell();
        }

        if (parent.Velocity >= PatternSpeed - 5 && !parent.ApproachPattern) {
            parent.ApproachPattern = true;
            indicators.ApproachPattern(true);
            indicators.PlayBell();
        }
        else if (parent.Velocity > PatternSpeed && !parent.Brake) {
            parent.Brake = true;
            indicators.Brake(true);
            indicators.PlayBell();
            controlBrake.brake();
        }
        //TODO: Implement the logic for the brake system and indicators
    }

    private double CalculateBrakePatternSpeed() {
        parent = GetNode<Scene>("..");
        _cfg.Load("user://config.cfg");
        double speed = parent.Velocity;
        double distance = parent.DistanceToSignalInFront;
        double decelRate = _cfg.GetValue("Train Data", "decelRate", 0.5).AsDouble();
        int vmax = _cfg.GetValue("Train Data", "Vmax", 100).AsInt16();
        double brakingRatio = _cfg.GetValue("Train Data", "brakingRatio", 1.0).AsDouble();
        double signalSpeed = parent.SignalInFrontSpeed;

        // Adjust deceleration by braking ratio
        double effectiveDecel = decelRate * brakingRatio;

        // Calculate the maximum speed at which the train can stop before the signal
        // v^2 = 2 * a * d + v_signal^2
        double patternSpeed = Math.Sqrt(Math.Max(0, 2 * effectiveDecel * distance + Math.Pow(signalSpeed, 2)));

        // Clamp to vmax + 5 if signal speed is above vmax
        if (signalSpeed > vmax) {
            return vmax + 5;
        }

        // Clamp to vmax
        patternSpeed = Math.Min(patternSpeed, vmax);

        return patternSpeed;
    }
}