# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.
extends OptionButton

var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$RichTextLabel.text = tr("UPDATE_MIRROR")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_save_pressed() -> void:
	if self.get_selected_id() == 0:
		cfg.set_value("General", "Update Channel", "stable")
	else:
		cfg.set_value("General", "Update Channel", "beta")
