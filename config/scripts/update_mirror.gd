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
	if cfg.get_value("General", "UpdateMirror", "GitHub") == "GitHub":self.selected = 0
	elif cfg.get_value("General", "UpdateMirror") == "GitLab":self.selected = 1
	elif cfg.get_value("General", "UpdateMirror") == "GitCode":self.selected = 2
	if cfg.get_value("General", "vJoy", false):$"../vJoy".button_pressed = true
	else:$"../vJoy".button_pressed = false
	$RichTextLabel.text = tr("UPDATE_MIRROR")
	if cfg.get_value("System", "DevSetting", false):$"../vJoy".disabled = false
	else:$"../vJoy".disabled = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_save_pressed() -> void:
	if self.get_selected_id() == 0:
		cfg.set_value("General", "UpdateMirror", "GitHub")
	elif self.get_selected_id() == 1:
		cfg.set_value("General", "UpdateMirror", "GitLab")
	else:
		cfg.set_value("General", "UpdateMirror", "GitCode")
	if $"../UpdateChannel".get_selected_id() == 0:
		cfg.set_value("General", "UpdateChannel", "Stable")
	else:
		cfg.set_value("General", "UpdateChannel", "Beta")
	if $"../vJoy".button_pressed:
		cfg.set_value("General", "vJoy", true)
	else:
		cfg.set_value("General", "vJoy", false)
	cfg.save("user://config.cfg")
