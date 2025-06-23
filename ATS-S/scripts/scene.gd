# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Node2D
var alarm = false
var chime = false
var alarmHard = false
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


func _on_timer_timeout() -> void:
	if alarm:
		self.alarmHard = true
		print("Hard alarm!")

func _notification(what: int) -> void:
	if what == NOTIFICATION_APPLICATION_FOCUS_IN:
		Engine.max_fps = 0 # Zero means uncapped
		OS.low_processor_usage_mode = false
		get_tree().paused = false
	if what == NOTIFICATION_APPLICATION_FOCUS_OUT:
		Engine.max_fps = 20
		OS.low_processor_usage_mode = true
		get_tree().paused = true
