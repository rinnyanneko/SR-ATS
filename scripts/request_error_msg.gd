# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends AcceptDialog


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.dialog_text = tr("S_ERROR_MSG")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_confirmed() -> void:
	get_tree().change_scene_to_file("res://main.tscn")
