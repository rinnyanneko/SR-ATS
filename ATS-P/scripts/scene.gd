# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends Node2D
var brake = false
var em_brake = false
var brake_open = false
var pattern_speed = 32767
var signalInFront = ""
var fail:bool

var VDDelayedTimetableIndex = -1.0
var SignalInFrontSpeed = -1
var DistanceToSignalInFront = -1.0
var SignalInFront = ""
var Velocity = -1
var Longitute = -1.0
var Latititute = -1.0
var InBorderStationArea = false
var ControlledBySteamID:String = ""
var UpdateTime

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
