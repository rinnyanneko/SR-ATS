# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.
extends OptionButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$RichTextLabel.text = tr("UPDATE_MIRROR")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
