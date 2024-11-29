extends LinkButton
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	cfg.load("res://config.cfg")
	cfg.set_value("License", "DoNotShow", "false")
	cfg.save("res://config.cfg")
	get_tree().change_scene_to_file("res://EULA.tscn")
