# SR-ATS
# https://github.com/rinnyanneko/SR-ATS
# Copyright © 2025 rinnyanneko. All rights reserved.

extends Node2D
const PpowerON = preload("res://ATS-P/assets/ATSp.png")
const PpowerOFF = preload("res://ATS-P/assets/ATSp_dim.png")
const ApproachPatternON = preload("res://ATS-P/assets/ApproachPattern.png")
const ApproachPatternOFF = preload("res://ATS-P/assets/ApproachPattern_dim.png")
const BrakeON = preload("res://ATS-P/assets/Brake.png")
const BrakeOFF = preload("res://ATS-P/assets/Brake_dim.png")
const BrakeOpenON = preload("res://ATS-P/assets/BrakeOpen.png")
const BrakeOpenOFF = preload("res://ATS-P/assets/BrakeOpen_dim.png")
const ATSpON = preload("res://ATS-P/assets/ATSp.png")
const ATSpOFF = preload("res://ATS-P/assets/ATSp_dim.png")

func Ppower(status:bool):
	if status:
		$Ppower.texture = PpowerON
	else:
		$Ppower.texture = PpowerOFF

func ApproachPattern(status:bool):
	if status:
		$ApproachPattern.texture = ApproachPatternON
	else:
		$ApproachPattern.texture = ApproachPatternOFF

func Brake(status:bool):
	if status:
		$Brake.texture = BrakeON
	else:
		$Brake.texture = BrakeOFF

func BrakeOpen(status:bool):
	if status:
		$BrakeOpen.texture = BrakeOpenON
	else:
		$BrakeOpen.texture = BrakeOpenOFF

func ATSp(status:bool):
	if status:
		$ATSp.texture = ATSpON
	else:
		$ATSp.texture = ATSpOFF

func fail(status:bool):
	pass
