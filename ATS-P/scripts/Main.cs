// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

using Godot;
using System;
using System.Threading.Tasks;

public partial class Main : Node{
    private Indicators indicators;
    private Scene parent;
    [Signal]
    public delegate void ATSReadyEventHandler();
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
            while (parent.Velocity < 10){
                await Task.Delay(6000);
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
        ControlBrake controlBrake = GetNode<ControlBrake>("../ControlBrake");
        //TODO: Implement the logic for the brake system and indicators
    }
}
