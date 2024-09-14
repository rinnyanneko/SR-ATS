extends LineEdit
var cfg = ConfigFile.new()
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_text_changed(new_text: String) -> void:
	cfg.set_value("System", "server", new_text)

func _on_train_number_text_changed(new_text: String) -> void:
	cfg.set_value("System", "trainNumber", new_text)
	cfg.save("res://config.cfg")
