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
	atsmode = self.atsmode
	self.title = tr("CONFIRM_TITLE")
	self.dialog_text = tr("CONFIRM_TEXT").format({server = cfg.get_value("System", "server"), trainNumber = cfg.get_value("System", "trainNumber")})
	self.ok_button_text = tr("CONFIRM")
	self.cancel_button_text = tr("CANCEL")
	self.visible = true

func _on_confirmed() -> void:
	match self.atsmode:
		"s":
			get_tree().change_scene_to_file("res://ATS-S/ats_s_scene.tscn")
		"ps":
			get_tree().change_scene_to_file("res://ATS-Ps/ats_ps_scene.tscn")
		"p":
			get_tree().change_scene_to_file("res://ATS-P/ats_p_scene.tscn")


func _on_canceled() -> void:
	get_tree().change_scene_to_file("res://main.tscn")
