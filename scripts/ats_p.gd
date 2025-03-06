# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends TextureButton
func _on_pressed() -> void:
	$"../ConfirmationDialog".run("p")
