# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends Button


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.text = tr("OPEN_APP_USR_DIR")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	OS.shell_open(ProjectSettings.globalize_path("user://"))
