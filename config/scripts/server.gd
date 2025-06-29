# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends OptionButton
var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	$RichTextLabel.text = tr("SERVER")
	$"../TrainNumber/RichTextLabel".text = tr("TRAIN_NUMBER")
	$"../Save".text = tr("SAVE")
	if cfg.get_value("Train Data", "server") != null:$".".text = cfg.get_value("Train Data", "server")
	if cfg.get_value("Train Data", "trainNumber") != null:$"../TrainNumber".text = cfg.get_value("Train Data", "trainNumber")
	if cfg.get_value("Train Data", "brakingRatio") != null:$"../Braking Ratio".text = cfg.get_value("Train Data", "brakingRatio")
	if cfg.get_value("Train Data", "Vmax") != null:$"../Vmax".text = str(cfg.get_value("Train Data", "Vmax"))


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_save_button_pressed() -> void:
	if $"../TrainNumber" != null and $".".get_selected_id() != -1:
		cfg.set_value("Train Data", "server", $"../../RequestServers".codes[$".".get_selected_id()])
		cfg.set_value("Train Data", "trainNumber", $"../TrainNumber".text)
	if $"../Braking Ratio".text != null:
		cfg.set_value("Train Data", "brakingRatio", $"../Braking Ratio".text)
		cfg.set_value("Train Data", "brakeDistance", 480/(float($"../Braking Ratio".text)))
		cfg.set_value("Train Data", "decelRate", 0.805*(float($"../Braking Ratio".text)**2))
		cfg.set_value("Train Data", "Vmax", int($"../Vmax".text))
	cfg.save("user://config.cfg")
