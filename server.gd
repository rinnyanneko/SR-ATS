# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends LineEdit
var cfg = ConfigFile.new()
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	await get_tree().create_timer(0.1).timeout
	$"../Server".placeholder_text = tr("SERVER")
	$"../TrainNumber".placeholder_text = tr("TRAIN_NUMBER")
	$"../Save".text = tr("SAVE")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_button_pressed() -> void:
	cfg.set_value("System", "server", $".".text)
	cfg.set_value("System", "trainNumber", $"../TrainNumber".text)
	cfg.save("res://config.cfg")


func _on_language_pressed() -> void:
	await get_tree().create_timer(0.1).timeout
	$"../Server".placeholder_text = tr("SERVER")
	$"../TrainNumber".placeholder_text = tr("TRAIN_NUMBER")
	$"../Save".text = tr("SAVE")
