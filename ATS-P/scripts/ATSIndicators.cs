// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;

public partial class ATSIndicators : Node2D{
	private readonly Texture2D _pPowerON = GD.Load<Texture2D>("res://ATS-P/assets/Ppower.png");
	private readonly Texture2D _pPowerOFF = GD.Load<Texture2D>("res://ATS-P/assets/Ppower_dim.png");
	private readonly Texture2D _approachPatternON = GD.Load<Texture2D>("res://ATS-P/assets/ApproachPattern.png");
	private readonly Texture2D _approachPatternOFF = GD.Load<Texture2D>("res://ATS-P/assets/ApproachPattern_dim.png");
	private readonly Texture2D _brakeON = GD.Load<Texture2D>("res://ATS-P/assets/Brake.png");
	private readonly Texture2D _brakeOFF = GD.Load<Texture2D>("res://ATS-P/assets/Brake_dim.png");
	private readonly Texture2D _brakeOpenON = GD.Load<Texture2D>("res://ATS-P/assets/BrakeOpen.png");
	private readonly Texture2D _brakeOpenOFF = GD.Load<Texture2D>("res://ATS-P/assets/BrakeOpen_dim.png");
	private readonly Texture2D _aTSpON = GD.Load<Texture2D>("res://ATS-P/assets/ATSp.png");
	private readonly Texture2D _aTSpOFF = GD.Load<Texture2D>("res://ATS-P/assets/ATSp_dim.png");
	private readonly Texture2D _failON = GD.Load<Texture2D>("res://ATS-P/assets/Fail.png");
	private readonly Texture2D _failOFF = GD.Load<Texture2D>("res://ATS-P/assets/Fail_dim.png");

	private Sprite2D _pPower;
	private Sprite2D _approachPattern;
	private Sprite2D _brake;
	private Sprite2D _brakeOpen;
	private Sprite2D _aTSp;
	private Sprite2D _fail;
	private AudioStreamPlayer _bell;

	public override void _Ready(){
		_pPower = GetNode<Sprite2D>("Ppower");
		_approachPattern = GetNode<Sprite2D>("ApproachPattern");
		_brake = GetNode<Sprite2D>("Brake");
		_brakeOpen = GetNode<Sprite2D>("BrakeOpen");
		_aTSp = GetNode<Sprite2D>("ATSp");
		_fail = GetNode<Sprite2D>("Fail");
		_bell = GetNode<AudioStreamPlayer>("Bell");
	}

	public void Ppower(bool status){
		_pPower.Texture = status ? _pPowerON : _pPowerOFF;
	}

	public void ApproachPattern(bool status){
		_approachPattern.Texture = status ? _approachPatternON : _approachPatternOFF;
	}

	public void Brake(bool status){
		_brake.Texture = status ? _brakeON : _brakeOFF;
	}

	public void BrakeOpen(bool status){
		_brakeOpen.Texture = status ? _brakeOpenON : _brakeOpenOFF;
	}

	public void ATSp(bool status){
		_aTSp.Texture = status ? _aTSpON : _aTSpOFF;
	}

	public void Fail(bool status){
		_fail.Texture = status ? _failON : _failOFF;
	}

	public void PlayBell(){
		_bell.Play();
	}
}
