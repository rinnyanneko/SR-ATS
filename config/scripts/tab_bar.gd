# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends TabBar
var cfg = ConfigFile.new()


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	$"../Train Data".visible = true
	$"../Debug".visible = false
	$"../Credits".visible = false
	self.clear_tabs()
	self.add_tab(tr("TRAIN_DATA"))
	self.add_tab(tr("GENERAL_SETTING"))
	self.add_tab("Debug")
	if cfg.get_value("System", "DevSetting"):
		self.set_tab_disabled(2, false)
	else:
		self.set_tab_disabled(2, true)
	self.add_tab("Credits")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_tab_changed(tab: int) -> void:
	match tab:
		0:
			$"../Train Data".visible = true
			$"../General".visible = false
			$"../Debug".visible = false
			$"../Credits".visible = false
		1:
			$"../Train Data".visible = false
			$"../General".visible = true
			$"../Debug".visible = false
			$"../Credits".visible = false
		2:
			$"../Train Data".visible = false
			$"../General".visible = false
			$"../Debug".visible = true
			$"../Credits".visible = false
		3:
			$"../Train Data".visible = false
			$"../General".visible = false
			$"../Debug".visible = false
			$"../Credits".visible = true
