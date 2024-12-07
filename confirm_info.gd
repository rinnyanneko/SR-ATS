# SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends ConfirmationDialog
var cfg = ConfigFile.new()
var atsmode = ""
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func run(atsmode: String):
	cfg.load("res://config.cfg")
	self.atsmode = atsmode
	var tmp = tr(tr("CONFIRM_TEXT").format({"server" : cfg.get_value("System", "server")}))
	self.dialog_text = tmp.format({"trainNumber" : cfg.get_value("System", "trainNumber")})
	print(self.dialog_text)
	print("Confirm?")
	self.ok_button_text = tr("CONFIRM")
	self.cancel_button_text = tr("CANCEL")
	self.visible = true

func _on_confirmed() -> void:
	print("Confirmed, enter ", self.atsmode, " mode")
	match self.atsmode:
		"s":
			get_tree().change_scene_to_file("res://ATS-S/ats_s_scene.tscn")
		"ps":
			get_tree().change_scene_to_file("res://ATS-Ps/ats_ps_scene.tscn")
		"p":
			get_tree().change_scene_to_file("res://ATS-P/ats_p_scene.tscn")


func _on_canceled() -> void:
	print("Canceled")
	self.visible = false


func _on_banner_pressed() -> void:
	OS.shell_open("https://github.com/rinnyanneko/SR-ATS")
