extends HTTPRequest
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var value = request("https://gitlab.com/rinnyanneko/SR-ATS/-/raw/main/news/news.json")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	cfg.load("res://config.cfg")
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
	var title = json["title"]
	$"../news".title = self.title
	$"../news".text = json["text"]
	if json["type"] != "" and json["number"] != cfg.get_value():
		
