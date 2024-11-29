extends TextureButton
var cfg = ConfigFile.new()
var sel = 0
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("System", "lang") == "en":
		TranslationServer.set_locale("en")
		print("English(en)!")
		sel = 1
	elif cfg.get_value("System", "lang") == "jp":
		TranslationServer.set_locale("jp")
		print("Japanese(jp)!")
		sel = 2
	elif cfg.get_value("System", "lang") == "cmn":
		TranslationServer.set_locale("cmn")
		print("Tradition Chinese(cmn)!")
		sel = 3
	elif cfg.get_value("System", "lang") == "zh":
		TranslationServer.set_locale("zh")
		print("Simplified Chinese(zh)!")
		sel = 0
	else: sel = 0
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
	cfg.save("res://config.cfg")
	sel += 1
	if sel > 3:sel = 0
	self.disabled = true
	await get_tree().create_timer(0.1).timeout
	self.disabled = false
