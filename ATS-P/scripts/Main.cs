// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

using Godot;
using System;

public partial class Main : Node{
    private Sprite2D indicators;
    public override void _Ready(){
        try{
            indicators = GetNode<Sprite2D>("../Indicators");
            indicators.Call("Ppower", true);
            indicators.Call("Fail", true);
            indicators.Call("playBell");
        }catch(Exception e){
            GD.PrintErr(e);
        }
    }
}
