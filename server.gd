extends LineEdit
var cfg = ConfigFile.new()
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_button_pressed() -> void:
	cfg.set_value("System", "server", $".".text)
	cfg.set_value("System", "trainNumber", $"../TrainNumber".text)
	cfg.save("res://config.cfg")
