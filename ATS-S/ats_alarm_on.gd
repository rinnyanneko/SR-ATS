extends Sprite2D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$"..".alarm = true
	$"ATS Alarm".play()



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if $"..".alarm == true:
		visible = true
	else:
		visible = false


func _on_ats_confirm_pressed() -> void:
	$"ATS Alarm".stop()
	$"..".alarm = false
	$"..".activated = true
	OS.delay_msec(10000)
	$"..".activated = false
