extends Sprite2D
var tmp = ConfigFile.new()
var passedSignal
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".alarm = true
	$"ATS Alarm".play()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	tmp.load("res://data.tmp")
	#var velocity:float = float(tmp.get_value("TrainData", "Velocity"))
	#var distanceToSignalInFront:float = float(tmp.get_value("TrainData", "DistanceToSignalInFront"))
	#var signalInFrontSpeed:float = float(tmp.get_value("TrainData", "SignalInFrontSpeed"))
	#var signalInFront:String = String(tmp.get_value("TrainData", "SignalInFront"))
	var velocity:float = float($"..".Velocity)
	var distanceToSignalInFront:float = float($"..".DistanceToSignalInFront)
	var signalInFrontSpeed:float = float($"..".SignalInFrontSpeed)
	var signalInFront:String = String($"..".SignalInFront)
	passedSignal = $"..".signalInFront
	#print(signalInFront)
	#print(distanceToSignalInFront)
	#print(passedSignal != signalInFront)
	if velocity > 0 && distanceToSignalInFront < 600 && signalInFrontSpeed < 32767 &&  passedSignal != signalInFront:
		visible = true
		$"..".alarm == true
		$"ATS Alarm".play()
		$"../AtsNormalOff/ATS Chime".play()
		$"..".signalInFront = signalInFront
	if $"..".alarm == true:
		visible = true
	else:
		visible = false
	if Input.is_action_just_pressed("ATS confirm"):
		_on_ats_confirm_pressed()


func _on_ats_confirm_pressed() -> void:
	$"ATS Alarm".stop()
	$"..".alarm = false
