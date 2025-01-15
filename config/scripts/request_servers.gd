# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends HTTPRequest

var codes:Array

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var value = request("https://panel.simrail.eu:8084/servers-open")
	if value == ERR_TIMEOUT:
		printerr("REQUEST TIMEOUT")
		$ErrorMsg.connection_timeout()
	elif value == ERR_BUSY:
		printerr("[HTTP REQUEST ERROR]BUSY")
	elif value != OK:
		printerr("[HTTP REQUEST ERROR]"+str(value))

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	if response_code != 200:
		printerr("HTTP response code:" + str(response_code))
		$"../ErrorMsg".connection_err(response_code)
		return
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
#	$"../Server".item_count = json["count"]
	for i in json["count"]:
		var data = json["data"][i]
		if data["IsActive"]:
			$"../Train Data/Server".add_item(data["ServerName"], i)
			codes.append(data["ServerCode"])
