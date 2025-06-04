# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends TextureButton
func _on_pressed() -> void:
	get_tree().change_scene_to_file("res://ATS-P/ats_p_scene.tscn")
