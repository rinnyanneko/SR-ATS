extends Sprite2D
var tmp = ConfigFile.new()
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".alarm = true
	$"ATS Alarm".play()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	var err = tmp.load("res://ATS-S/data.tmp")
	var velocity:float = tmp.get_value("TrainData", "Velocity")
	var distanceToSignalInFrount:float = tmp.get_value("TrainData", "DistanceToSignalInFront")
	var signalInFrontSpeed:float = tmp.get_value("TrainData", "SignalInFrontSpeed")
	var atsActivated:bool = $"..".activated
	if velocity > 0 && distanceToSignalInFrount < 600 && signalInFrontSpeed != 32767.0 && !$"..".activated:
		$"..".alarm == true
		$"ATS Alarm".play()
		$"../AtsNormalOff/ATS Chime".play()
	if $"..".alarm == true:
		visible = true
		$"..".activated = true
	else:
		visible = false
	if Input.is_action_just_pressed("ATS confirm"):
		_on_ats_confirm_pressed()


func _on_ats_confirm_pressed() -> void:
	$"ATS Alarm".stop()
	$"..".alarm = false
	await get_tree().create_timer(10).timeout
	$"..".activated = false
