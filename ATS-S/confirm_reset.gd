extends ConfirmationDialog


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func run():
	self.visible = true
	self.dialog_text = tr("S_CONFIRM_RESET")
	self.ok_button_text = tr("CONFIRM")
	self.cancel_button_text = tr("CANCEL")


func _on_confirmed() -> void:
	$"..".alarm = false
	$"..".alarmHard = false
	$"../AtsAlarmOn/ATS Alarm".stop()
	print("ATS Reset!")
	self.visible = false


func _on_canceled() -> void:
	self.visible = false
