extends Button


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.text = tr("PLEASE_ENTER_TRAIN_DATA")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_language_pressed() -> void:
	self.text = tr("PLEASE_ENTER_TRAIN_DATA")


func _on_pressed() -> void:
	get_tree().change_scene_to_file("res://config/config.tscn")
