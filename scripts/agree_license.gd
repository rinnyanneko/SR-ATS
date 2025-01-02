# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends Button
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.disabled = true
	await get_tree().create_timer(0.2).timeout
	cfg.load("res://config.cfg")
	$"../Title".text = tr("READ_LICENSE")
	$"../License".text = FileAccess.get_file_as_string("res://license/LICENSE_" + TranslationServer.get_locale() + ".txt")
	if $"../License".text == "":
		$"../License".text = FileAccess.get_file_as_string("res://license/LICENSE_en.txt")
	$"../DoNotShow".text = tr("DO_NOT_SHOW")
	self.text = tr("AGREE_LICENSE")
	timer()
	if cfg.get_value("License", "DoNotShow") and cfg.get_value("License", "agree"):
		get_tree().change_scene_to_file("res://main.tscn")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_pressed() -> void:
	if $"../DoNotShow".button_pressed:
		cfg.set_value("License", "DoNotShow", true)
	else:
		cfg.set_value("License", "DoNotShow", false)
	cfg.set_value("License", "agree", true)
	cfg.save("res://config.cfg")
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
