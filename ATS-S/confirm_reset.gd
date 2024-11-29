# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

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
	$"../ATS reset back/ATS reset cover"._on_ats_reset_cover_open_pressed()


func _on_confirmed() -> void:
	$"..".alarm = false
	$"..".alarmHard = false
	$"../AtsAlarmOn/ATS Alarm".stop()
	print("ATS Reset!")
	self.visible = false


func _on_canceled() -> void:
	self.visible = false
