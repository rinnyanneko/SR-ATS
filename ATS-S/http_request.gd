extends HTTPRequest
var cfg = ConfigFile.new()
var err = cfg.load("res://config.cfg")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	while(true):
		var value = self.request("https://panel.simrail.eu:8084/trains-open?serverCode=" + cfg.get_value("System", "server"))
		if value != 44:
			print(value)
		await get_tree().create_timer(3).timeout 


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	var json = JSON.parse_string(body.get_string_from_utf8())
	
