# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.
extends OptionButton

var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("General", "UpdateMirror") == "GitHub":self.selected = 0
	elif cfg.get_value("General", "UpdateMirror") == "GitLab":self.selected = 1
	elif cfg.get_value("General", "UpdateMirror") == "GitCode":self.selected = 2
	$RichTextLabel.text = tr("UPDATE_MIRROR")
	


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
		cfg.set_value("General", "UpdateChannel", "stable")
	else:
		cfg.set_value("General", "UpdateChannel", "beta")
	cfg.save("res://config.cfg")
