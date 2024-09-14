extends HTTPRequest
var cfg = ConfigFile.new()
var err = cfg.load("res://config.cfg")
var tmp = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	while(true):
		var value = self.request("https://panel.simrail.eu:8084/trains-open?serverCode=" + cfg.get_value("System", "server"))
		await get_tree().create_timer(5).timeout


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	var json:Dictionary = JSON.parse_string(body.get_string_from_utf8())
	var dataArray:Array = json["data"]
	var data = readArray(dataArray)
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

func readArray(array:Array):
	for data in array:
		if data["TrainNoLocal"] == cfg.get_value("System", "trainNumber"):
			return data["TrainData"]
