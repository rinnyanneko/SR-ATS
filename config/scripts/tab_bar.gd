# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends TabBar


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_tab_changed(tab: int) -> void:
	match tab:
		0:
			$"../General".visible = true
			$"../Debug".visible = false
			$"../Credits".visible = false
		1:
			$"../General".visible = false
			$"../Debug".visible = true
			$"../Credits".visible = false
		2:
			$"../General".visible = false
			$"../Debug".visible = false
			$"../Credits".visible = true
