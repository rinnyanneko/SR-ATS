# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Sprite2D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".chime = true
	$"ATS Chime".play()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if $"..".alarm == true:
		$"../AtsNormalOn".visible = false
		$"..".chime = true
	else:
		$"../AtsNormalOn".visible = true


func _on_chime_off_pressed() -> void:
	if $"..".alarm == false and $"..".alarmHard == false:
		$"..".chime = false
		$"ATS Chime".stop()
		print("Chime off!")
