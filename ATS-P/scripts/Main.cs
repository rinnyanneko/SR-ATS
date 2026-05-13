// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node {
	private ATSIndicators indicators;
	private Scene parent;
	private ControlBrake controlBrake;
	private CancellationTokenSource startupCancellation = new CancellationTokenSource();
	
	private bool isReady = false;

	[Signal]
	public delegate void ATSReadyEventHandler();

	
	public override void _Notification(int what) {
		if (what == NotificationApplicationFocusIn)
			Engine.MaxFps = 0; // Zero means uncapped
		if (what == NotificationApplicationFocusOut)
			Engine.MaxFps = 20;
	}

	public override async void _Ready() {
		try {
			parent = GetNode<Scene>("..");
			indicators = GetNode<ATSIndicators>("../Indicators");
			controlBrake = GetNode<ControlBrake>("../ControlBrake");
			indicators.Ppower(true);
			parent.Ppower = true;
			indicators.Fail(true);
			parent.Fail = true;
			indicators.PlayBell();
			await Task.Delay(3000, startupCancellation.Token);
			if (!IsInstanceValid(this) || !IsInstanceValid(parent) || !IsInstanceValid(indicators)) {
				return;
			}

			indicators.Fail(false);
			parent.Fail = false;
			indicators.PlayBell();
			EmitSignal(SignalName.ATSReady);
			await Task.Delay(3000, startupCancellation.Token);
			if (!IsInstanceValid(this) || !IsInstanceValid(parent) || !IsInstanceValid(indicators)) {
				return;
			}

			while (IsInstanceValid(parent) && parent.DistanceToSignalInFront > 500) {
				await Task.Delay(2000, startupCancellation.Token);
				if (!IsInstanceValid(this) || !IsInstanceValid(parent) || !IsInstanceValid(indicators)) {
					return;
				}
			}

			indicators.ATSp(true);
			parent.ATSp = true;
			indicators.PlayBell();
			this.isReady = true;
		}
		catch (TaskCanceledException) {
		}
		catch (Exception e) {
			GD.PrintErr(e);
		}
	}

	public override void _ExitTree() {
		isReady = false;
		startupCancellation.Cancel();
		startupCancellation.Dispose();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (!isReady) {
			return; // Exit if ATS is not ready
		}
		double patternSpeed = CalculateBrakePatternSpeed();
		parent.PatternSpeed = patternSpeed;

		if (patternSpeed < 0 || parent.Velocity < 0) {
			ClearPatternAndBrake();
			return;
		}

		if (parent.Velocity > patternSpeed && !parent.Brake) {
			parent.Brake = true;
			indicators.Brake(true);
			indicators.PlayBell();
			controlBrake.Brake();
		}
		else if (parent.Velocity < patternSpeed - 5 && parent.Brake) {
			parent.Brake = false;
			indicators.Brake(false);
			indicators.PlayBell();
			controlBrake.Release();
		}

		if (parent.Velocity >= patternSpeed - 5 && !parent.ApproachPattern) {
			parent.ApproachPattern = true;
			indicators.ApproachPattern(true);
			indicators.PlayBell();
		}
		else if (parent.Velocity < patternSpeed - 5 && parent.ApproachPattern) {
			parent.ApproachPattern = false;
			indicators.ApproachPattern(false);
			indicators.PlayBell();
		}
	}

	private double CalculateBrakePatternSpeed() {
		double distance = parent.DistanceToSignalInFront;
		double effectiveDecel = parent.EffectiveDecel;
		int vmax = parent.Vmax;
		double signalSpeed = parent.SignalInFrontSpeed;

		if (distance < 0 || signalSpeed < 0 || effectiveDecel <= 0) {
			return -1;
		}

		// Calculate the maximum speed at which the train can stop before the signal
		// v^2 = 2 * a * d + v_signal^2
		double patternSpeed = Math.Sqrt(Math.Max(0, 2 * effectiveDecel * distance + Math.Pow(signalSpeed, 2)));

		// Clamp to vmax + 5 if signal speed is above vmax
		if (signalSpeed > vmax) {
			return vmax + 5;
		}

		// Clamp to vmax
		patternSpeed = Math.Min(patternSpeed, vmax);

		return patternSpeed;
	}

	private void ClearPatternAndBrake() {
		if (parent.Brake) {
			parent.Brake = false;
			indicators.Brake(false);
			controlBrake.Release();
			indicators.PlayBell();
		}

		if (parent.ApproachPattern) {
			parent.ApproachPattern = false;
			indicators.ApproachPattern(false);
			indicators.PlayBell();
		}
	}
}
