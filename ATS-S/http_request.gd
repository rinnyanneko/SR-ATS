extends HTTPRequest
var cfg = ConfigFile.new()
var err = cfg.load("res://config.cfg")
var tmp = ConfigFile.new()
var errors = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	while(true):
		var value = request("https://panel.simrail.eu:8084/trains-open?serverCode=" + cfg.get_value("System", "server"))
		await get_tree().create_timer(7).timeout


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
	var dataArray:Array = json["data"]
	var data = readArray(dataArray)
	if data != null and data["ControlledBySteamID"] != null and data["InBorderStationArea"] != null and data["Latititute"] != null and data["Longitute"] != null and data["Velocity"] != null and data["SignalInFront"] != null and data["DistanceToSignalInFront"] != null and data["SignalInFrontSpeed"] != null and data["VDDelayedTimetableIndex"] != null:
		tmp.set_value("TrainData", "ControlledBySteamID", data["ControlledBySteamID"])
		tmp.set_value("TrainData", "InBorderStationArea", data["InBorderStationArea"])
		tmp.set_value("TrainData", "Latititute", data["Latititute"])
		tmp.set_value("TrainData", "Longitute", data["Longitute"])
		tmp.set_value("TrainData", "Velocity", data["Velocity"])
		tmp.set_value("TrainData", "SignalInFront", data["SignalInFront"])
		tmp.set_value("TrainData", "DistanceToSignalInFront", data["DistanceToSignalInFront"])
		tmp.set_value("TrainData", "SignalInFrontSpeed", data["SignalInFrontSpeed"])
		tmp.set_value("TrainData", "VDDelayedTimetableIndex", data["VDDelayedTimetableIndex"])
		tmp.save("res://ATS-S/data.tmp")
	elif errors < 10:
		errors += 1
	else:
		$"../AcceptDialog".visible = true
		get_tree().change_scene_to_file("res://main.tscn")
		errors = 0

func readArray(array:Array):
	for data in array:
		if data["TrainNoLocal"] == cfg.get_value("System", "trainNumber"):
			return data["TrainData"]
