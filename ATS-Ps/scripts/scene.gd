# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Node2D
var em_brake = false
var brake_disable = false
var pattern_generated = false
var pattern_speed = 32767
var signalInFront = ""

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
