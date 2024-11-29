# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends TextureButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_on_ats_reset_cover_open_pressed()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass



func _on_pressed() -> void:
	print("Open reset cover")
	self.visible = false
	$"../ATS reset cover open".visible = true
	$"../ATS Reset".disabled = false
	$"../ATS Reset".visible = true


func _on_ats_reset_cover_open_pressed() -> void:
	$"../ATS reset cover open".visible = false
	self.visible = true
	$"../ATS Reset".disabled = true
	$"../ATS Reset".visible = false
	print("Close reset cover")