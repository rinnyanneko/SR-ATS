extends Sprite2D
var tmp = ConfigFile.new()
var passedSignal
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".alarm = true
	$"ATS Alarm".play()
	print("Test Alarm!")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
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
		print("Alarm Triggered!")
		$"../Timer".start()
	if $"..".alarm == true:
		visible = true
	else:
		visible = false
	if Input.is_action_just_pressed("ATS confirm"):
		_on_ats_confirm_pressed()


func _on_ats_confirm_pressed() -> void:
	if not $"..".alarmHard:
		$"ATS Alarm".stop()
		$"..".alarm = false
		$"../Timer".stop()
		print("Alarm confirmed!")
