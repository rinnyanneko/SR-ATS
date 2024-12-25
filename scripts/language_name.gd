extends RichTextLabel


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.text = tr("LANG_NAME")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_language_pressed() -> void:
	await get_tree().create_timer(0.1)
	self.text = tr("LANG_NAME")
