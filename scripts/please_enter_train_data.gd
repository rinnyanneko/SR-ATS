# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Button
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	self.text = tr("PLEASE_ENTER_TRAIN_DATA")
	if cfg.get_value("Train Data", "server", "null") != "null" and cfg.get_value("Train Data", "trainNumber", "null") != "null":
		self.text = tr("ENTERED_TRAIN_DATA").format({server = cfg.get_value("Train Data", "server", "null"), trainNumber = cfg.get_value("Train Data", "trainNumber", "null")})


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_language_pressed() -> void:
	self._ready()


func _on_pressed() -> void:
	get_tree().change_scene_to_file("res://config/config.tscn")
