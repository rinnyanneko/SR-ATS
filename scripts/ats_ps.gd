# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends TextureButton
func _on_pressed() -> void:
	get_tree().change_scene_to_file("res://ATS-Ps/ats_ps_scene.tscn")
