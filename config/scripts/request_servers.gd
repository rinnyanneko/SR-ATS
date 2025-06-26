# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends HTTPRequest

var cfg = ConfigFile.new()
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
	cfg.load("user://config.cfg")
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
	var selected = cfg.get_value("Train Data", "server")
	if selected != null:
		for i in range(codes.size()):
			if codes[i] == selected:
				$"../Train Data/Server".selected = i
