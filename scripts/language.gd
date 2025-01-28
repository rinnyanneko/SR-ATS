# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends TextureButton
var cfg = ConfigFile.new()
var sel = 0
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("user://config.cfg")
	if cfg.get_value("System", "lang", "null") == "en":
		TranslationServer.set_locale("en")
		print("English(en)!")
		sel = 1
	elif cfg.get_value("System", "lang", "null") == "jp":
		TranslationServer.set_locale("jp")
		print("Japanese(jp)!")
		sel = 2
	elif cfg.get_value("System", "lang", "null") == "cmn":
		TranslationServer.set_locale("cmn")
		print("Tradition Chinese(cmn)!")
		sel = 3
	elif cfg.get_value("System", "lang", "null") == "zh":
		TranslationServer.set_locale("zh")
		print("Simplified Chinese(zh)!")
		sel = 4
	elif cfg.get_value("System", "lang", "null") == "ko":
		TranslationServer.set_locale("ko")
		print("Korean(ko)!")
		sel = 0
	else:
		sel = 1
		TranslationServer.set_locale("en")
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass



func _on_pressed() -> void:
	if sel == 0:
		cfg.set_value("System", "lang", "en")
		TranslationServer.set_locale("en")
		print("English(en)!")
	if sel == 1:
		cfg.set_value("System", "lang", "jp")
		TranslationServer.set_locale("jp")
		print("Japanese(jp)!")
	if sel == 2:
		cfg.set_value("System", "lang", "cmn")
		TranslationServer.set_locale("cmn")
		print("Tradition Chinese(cmn)!")
	if sel == 3:
		cfg.set_value("System", "lang", "zh")
		TranslationServer.set_locale("zh")
		print("Simplified Chinese(zh)!")
	if sel == 4:
		cfg.set_value("System", "lang", "ko")
		TranslationServer.set_locale("ko")
		print("Korean(ko)!")
	cfg.save("user://config.cfg")
	sel += 1
	if sel > 4:sel = 0
	self.disabled = true
	await get_tree().create_timer(0.1).timeout
	self.disabled = false
