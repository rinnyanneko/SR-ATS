# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.
extends OptionButton

var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("General", "UpdateChannel") == "Stable":self.selected = 0
	elif cfg.get_value("General", "UpdateChannel") == "Beta":self.selected = 1
	$RichTextLabel.text = tr("UPDATE_CHANNEL")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
