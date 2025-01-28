# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends HTTPRequest
var cfg = ConfigFile.new()
var json:Dictionary
var data

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	request("https://gitlab.com/rinnyanneko/SR-ATS/-/raw/main/news/news.json")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	if response_code == 200:
		cfg.load("user://config.cfg")
		json = JSON.parse_string(body.get_string_from_utf8())
		data = json["data"]
		for i in data:
			if not i["draft"]:
				data = i
		$"../news".title = data["title"]
		$"../news/RichTextLabel".text = data["text"]
		$"../news".cancel_button_text = tr("CLOSE")
		$"../news".ok_button_text = tr("DO_NOT_SHOW")
		if data["type"] != "" and data["number"] > cfg.get_value("News", "NeverShow", 0)and not data["draft"]:
			$"../news".visible = true


func _on_news_confirmed() -> void:
	cfg.set_value("News", "NeverShow", data["number"])
	cfg.save("user://config.cfg")
	print("Never show this news")
	$"../news".visible = false

func _on_news_canceled() -> void:
	$"../news".visible = false
	print("News closed")


func _on_rich_text_label_meta_clicked(meta: Variant) -> void:
	OS.shell_open(str(meta))
