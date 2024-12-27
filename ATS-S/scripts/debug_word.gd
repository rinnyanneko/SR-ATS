# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2024 rinnyanneko. All rights reserved.

extends RichTextLabel
var cfg = ConfigFile.new()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if cfg.get_value("System", "Debug"):
		self.visible = true
	else:
		self.visible = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	cfg.load("res://config.cfg")
	if cfg.get_value("System", "Debug"):
		self.visible = true
		self.text="UpdateTime = "+str($"..".UpdateTime)+"\nalarm = "+str($"..".alarm)+"\nchime = "+str($"..".chime)+"\nalarmHard = "+str($"..".alarmHard)+"\nsignalInFront = "+str($"..".signalInFront)+"\nVDDelayedTimetableIndex = "+str($"..".VDDelayedTimetableIndex)+"\nSignalInFrontSpeed = "+str($"..".SignalInFrontSpeed)+"\nDistanceToSignalInFront = "+str($"..".DistanceToSignalInFront)+"\nSignalInFront = "+str($"..".SignalInFront)+"\nVelocity = "+str($"..".Velocity)+"\nLongitute = "+str($"..".Longitute)+"\nLatititute = "+str($"..".Latititute)+"\nInBorderStationArea = "+str($"..".InBorderStationArea)+"\nControlledBySteamID = "+str($"..".ControlledBySteamID)
