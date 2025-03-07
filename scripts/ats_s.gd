# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends TextureButton
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	if cfg.get_value("Train Data", "server") != null and cfg.get_value("Train Data", "trainNumber") != null:
		$"../ATS-S".disabled = false
		#$"../ATS-P".disabled = false
		#$"../ATS-Ps".disabled = false
		if cfg.get_value("System", "DevSetting", false):
			$"../ATS-P".disabled = false
			#$"../ATS-Ps".disabled = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	get_tree().change_scene_to_file("res://ATS-P/ats_p_scene.tscn")
