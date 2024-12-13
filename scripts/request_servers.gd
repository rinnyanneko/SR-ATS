# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.
extends HTTPRequest

var codes:Array

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var value = request("https://panel.simrail.eu:8084/servers-open")

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
#	$"../Server".item_count = json["count"]
	for i in json["count"]:
		var data = json["data"][i]
		if data["IsActive"]:
			$"../Server".add_item(data["ServerName"], i)
			codes.append(data["ServerCode"])
