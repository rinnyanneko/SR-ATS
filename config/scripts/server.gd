# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends OptionButton
var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	$"../Server".text = tr("SERVER")
	$"../TrainNumber".placeholder_text = tr("TRAIN_NUMBER")
	$"../Save".text = tr("SAVE")
	if cfg.get_value("System", "DevSetting"):
		$"../Debug Switch".disabled = false
		$"../../TabBar".set_tab_disabled(1, false)
	else:
		$"../../TabBar".set_tab_disabled(1, true)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_save_button_pressed() -> void:
	if $"../TrainNumber" != null:
		cfg.set_value("System", "server", $"../../RequestServers".codes[$".".get_selected_id()])
		cfg.set_value("System", "trainNumber", $"../TrainNumber".text)
	cfg.set_value("System", "Debug", $"../Debug Switch".button_pressed)
	cfg.save("res://config.cfg")
