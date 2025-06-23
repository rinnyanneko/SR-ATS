# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends Button
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	self.disabled = true
	if cfg.get_value("License", "DoNotShow", false) and cfg.get_value("License", "agree", false):
		get_tree().change_scene_to_file("res://main.tscn")
	await get_tree().create_timer(0.2).timeout
	$"../Title".text = tr("READ_LICENSE")
	match TranslationServer.get_locale():
		"cmn":
			$"../License".text = FileAccess.get_file_as_string("res://license/LICENSE_cmn.txt")
		"ja":
			$"../License".text = FileAccess.get_file_as_string("res://license/LICENSE_ja.txt")
		_:
			$"../License".text = FileAccess.get_file_as_string("res://license/LICENSE.txt")
	if $"../License".text == "":
		$"../License".text = "ERROR: CAN NOT SHOW LICENSE"
		return
	$"../DoNotShow".text = tr("DO_NOT_SHOW")
	self.text = tr("AGREE_LICENSE")
	timer()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	if $"../DoNotShow".button_pressed:
		cfg.set_value("License", "DoNotShow", true)
	else:
		cfg.set_value("License", "DoNotShow", false)
	cfg.set_value("License", "agree", true)
	cfg.save("user://config.cfg")
	get_tree().change_scene_to_file("res://main.tscn")


func _on_language_pressed() -> void:
	await get_tree().create_timer(0.2).timeout
	get_tree().change_scene_to_file("res://EULA.tscn")

func timer():
	self.disabled = true
	for i in range(5, -1, -1):
		self.text = (tr("AGREE_LICENSE") + "...(" + String.num_int64(i) + ")")
		await get_tree().create_timer(1).timeout
	self.text = tr("AGREE_LICENSE")
	self.disabled = false
