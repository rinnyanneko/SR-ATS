# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.
extends HTTPRequest

var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	check_for_update()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func check_for_update():
	cfg.load("res://config.cfg")
	if cfg.get_value("General", "Update Mirror") == "GitHub":
		var value = request("https://raw.githubusercontent.com/rinnyanneko/SR-ATS/refs/heads/main/news/news.json")
	elif cfg.get_value("General", "Update Mirror") == "GitCode":
		var value = request("https://raw.gitcode.com/rinnyanneko/SR-ATS/raw/main/news/news.json")
	else:
		var value = request("https://gitlab.com/rinnyanneko/SR-ATS/-/raw/main/news/news.json")

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	if response_code == 200:
		$"..".visible = true
		var json = JSON.parse_string(body.get_string_from_utf8())
		if cfg.get_value("General", "Update Channel") == "beta":
			if json["latest-beta"] != $"../../version".text:
				$"..".dialog_text = tr("NEW_BETA_UPDATE").format({"version" : json["latest-beta"]})
			else:
				$"..".dialog_text = tr("NO_UPDATE")
				await get_tree().create_timer(3)
				$"..".visible = false
		else:
			if json["latest-stable"] != $"../../version".text:
				$"..".dialog_text = tr("NEW_STABLE_UPDATE").format({"version" : json["latest-stable"]})
			else:
				$"..".dialog_text = tr("NO_UPDATE")
				await get_tree().create_timer(3)
				$"..".visible = false
