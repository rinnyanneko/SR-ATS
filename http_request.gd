extends HTTPRequest
var cfg = ConfigFile.new()
var tmp = ConfigFile.new()
var errors = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	while(true):
		print("getting data from server...")
		var value = request("https://panel.simrail.eu:8084/trains-open?serverCode=" + cfg.get_value("System", "server"))
		await get_tree().create_timer(7).timeout


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
		errors = 0
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
	elif errors < 2:
		errors += 1


func readArray(array:Array):
	for data in array:
		if data["TrainNoLocal"] == cfg.get_value("System", "trainNumber"):
			return data["TrainData"]


func _on_timer_timeout() -> void:
	if errors >= 2:
		$"../ErrorMsg".visible = true
	else:
		errors = 0
