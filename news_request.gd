extends HTTPRequest
var cfg = ConfigFile.new()
var json:Dictionary

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var value = request("https://gitlab.com/rinnyanneko/SR-ATS/-/raw/main/news/news.json")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	cfg.load("res://config.cfg")
	json = JSON.parse_string(body.get_string_from_utf8())
	$"../news".title = json["title"]
	$"../news".dialog_text = json["text"]
	$"../news".cancel_button_text = tr("CLOSE")
	$"../news".ok_button_text = tr("DO_NOT_SHOW")
	if json["image"] != "false":
		$"../news".dialog_text += "[img]" + json["image"] + "[/img]"
	print(json)
	if json["type"] != "" and json["number"] != cfg.get_value("News", "NeverShow"):
		$"../news".visible = true


func _on_news_confirmed() -> void:
	cfg.set_value("News", "NeverShow", String(json["number"]))
	cfg.save("res://config.cfg")
	print("Never show this news")
	$"../news".visible = false

func _on_news_canceled() -> void:
	$"../news".visible = false
	print("News closed")
