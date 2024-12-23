# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends Button
var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("System", "server") != null and cfg.get_value("System", "trainNumber") != null:
		$"../ATS-S".disabled = false
		#$"../ATS-P".disabled = false
		#$"../ATS-Ps".disabled = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	cfg.load("res://config.cfg")
	if $"../Server".text != null and $"../TrainNumber".text != null:
		$"../ATS-S".disabled = false
		#$"../ATS-P".disabled = false
		#$"../ATS-Ps".disabled = false
