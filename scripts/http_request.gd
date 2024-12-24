# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends HTTPRequest
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("System", "server") != "127":
		while(true):
			print("getting data from server...")
			var value = request("https://panel.simrail.eu:8084/trains-open?serverCode=" + cfg.get_value("System", "server"))
			await get_tree().create_timer(2).timeout
	else:
		pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
	var dataArray:Array = json["data"]
	var data = readArray(dataArray)
	print(data)
	if data != null:
		$"../ErrorMsg".visible = false
		if data["ControlledBySteamID"] != null:
			$"..".ControlledBySteamID = data["ControlledBySteamID"]
		else:
			$"..".ControlledBySteamID = "null"
		$"..".InBorderStationArea = data["InBorderStationArea"]
		$"..".Latititute = data["Latititute"]
		$"..".Longitute = data["Longitute"]
		$"..".Velocity = data["Velocity"]
		$"..".SignalInFront = data["SignalInFront"]
		$"..".DistanceToSignalInFront = data["DistanceToSignalInFront"]
		$"..".SignalInFrontSpeed = data["SignalInFrontSpeed"]
		$"..".VDDelayedTimetableIndex = data["VDDelayedTimetableIndex"]
	else:
		$"../ErrorMsg".visible = true

func readArray(array:Array):
	for data in array:
		if data["TrainNoLocal"] == cfg.get_value("System", "trainNumber"):
			return data["TrainData"]