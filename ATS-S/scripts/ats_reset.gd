# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends TextureButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	$"../..".alarm = false
	$"../..".alarmHard = false
	$"../../AtsAlarmOn/ATS Alarm".stop()
	print("ATS Reset!")
