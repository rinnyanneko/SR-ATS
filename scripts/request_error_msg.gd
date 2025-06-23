# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. Some Rights Reserved.
# Licensed under the Apache License, Version 2.0
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0

extends AcceptDialog


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.get_child(1, true).horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_confirmed() -> void:
	get_tree().change_scene_to_file("res://main.tscn")

func train_not_found():
	self.dialog_text = tr("TRAIN_NOT_FOUND")
	self.visible = true

func server_not_found():
	self.dialog_text = tr("SERVER_NOT_FOUND")
	self.visible = true

func connection_err(code:int):
	self.dialog_text = tr("CONNECTION_ERROR")+"\n"
	match code:
		400:
			self.dialog_text+="400 Bad Request"
		401:
			self.dialog_text+="401 Unauthorized"
		402:
			self.dialog_text+="402 Payment Required"
		403:
			self.dialog_text+="403 Forbidden"
		404:
			self.dialog_text+="404 Not Found"
		405:
			self.dialog_text+="405 Method Not Allowed"
		406:
			self.dialog_text+="406 Not Acceptable"
		407:
			self.dialog_text+="407 Proxy Authentication Required"
		408:
			self.dialog_text+="408 Request Timeout"
		425:
			self.dialog_text+="409 Too Early"
		451:
			self.dialog_text+="451 Unavailable For Legal Reasons"
		500:
			self.dialog_text+="500 Internal Server Error"
		501:
			self.dialog_text+="501 Not Implemented"
		502:
			self.dialog_text+="502 Bad Gateway"
		503:
			self.dialog_text+="503 Service Unavailable"
		504:
			self.dialog_text+="504 Gateway Timeout"
		505:
			self.dialog_text+="505 HTTP Version Not Supported"
		508:
			self.dialog_text+="508 Loop Detected"
		511:
			self.dialog_text+="511 Network Authentication Required"
		444:
			self.dialog_text+="444 No Response"
		450:
			self.dialog_text+="450 Blocked by Windows Parental Controls"
		_:
			self.dialog_text+=str(code)
	self.visible = true

func connection_timeout():
	self.dialog_text = tr("CONNECTION_TIMEOUT")
	self.visible = true
