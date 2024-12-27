# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends TabBar
var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	$"../Train Data".visible = true
	$"../Debug".visible = false
	$"../Credits".visible = false
	self.add_tab(tr("TRAIN_DATA"))
	self.add_tab("Debug")
	if cfg.get_value("System", "DevSetting"):
		self.set_tab_disabled(1, false)
	else:
		self.set_tab_disabled(1, true)
	self.add_tab("Credits")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_tab_changed(tab: int) -> void:
	match tab:
		0:
			$"../Train Data".visible = true
			$"../Debug".visible = false
			$"../Credits".visible = false
		1:
			$"../Train Data".visible = false
			$"../Debug".visible = true
			$"../Credits".visible = false
		2:
			$"../Train Data".visible = false
			$"../Debug".visible = false
			$"../Credits".visible = true
