# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Sprite2D
var tmp = ConfigFile.new()
var passedSignal
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".alarm = true
	$"ATS Alarm".play()
	print("Test Alarm!")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	var velocity:float = float($"..".Velocity)
	var distanceToSignalInFront:float = float($"..".DistanceToSignalInFront)
	var signalInFrontSpeed:float = float($"..".SignalInFrontSpeed)
	var signalInFront:String = str($"..".SignalInFront)
	passedSignal = $"..".signalInFront
	#print(signalInFront)
	#print(distanceToSignalInFront)
	#print(passedSignal != signalInFront)
	if velocity > 0 && distanceToSignalInFront < 600 && signalInFrontSpeed <= 100 &&  passedSignal != signalInFront:
		self.visible = true
		self.top_level
		$"..".alarm == true
		$"ATS Alarm".play()
		$"../AtsNormalOff/ATS Chime".play()
		$"..".signalInFront = signalInFront
		print("Alarm Triggered!")
		$"../Timer".start()
	if $"..".alarm == true:
		visible = true
	else:
		visible = false
	if Input.is_action_just_pressed("ATS confirm"):
		_on_ats_confirm_pressed()
	if velocity > 0 && distanceToSignalInFront < 10 && signalInFrontSpeed == 0:
		$"..".alarmHard = true
		print("Hard alarm!")
		self.visible = true
		self.top_level
		$"..".alarm == true
		$"ATS Alarm".play()
		$"../AtsNormalOff/ATS Chime".play()
		$"..".signalInFront = signalInFront


func _on_ats_confirm_pressed() -> void:
	if not $"..".alarmHard:
		$"ATS Alarm".stop()
		$"..".alarm = false
		$"../Timer".stop()
		print("Alarm confirmed!")
