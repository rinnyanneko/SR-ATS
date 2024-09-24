extends OptionButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_item_selected(index: int) -> void:
	if self.get_selected_id() == 0:
		TranslationServer.set_locale("en")
	if self.get_selected_id() == 1:
		TranslationServer.set_locale("jp")
	if self.get_selected_id() == 2:
		TranslationServer.set_locale("cmn")
	if self.get_selected_id() == 3:
		TranslationServer.set_locale("zh")
