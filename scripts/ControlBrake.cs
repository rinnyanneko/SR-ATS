// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

#nullable enable

using Godot;
using System.Text;

public partial class ControlBrake: Node {
	private readonly WebSocketPeer socket = new WebSocketPeer();
	private readonly ConfigFile cfg = new ConfigFile();
	private readonly Godot.Collections.Array<string> queuedCommands = [];
	private double reconnectInSeconds;
	private string currentUrl = UpdateMirrorOptionButton.DefaultSimRailConnectUrl;

	public override void _Ready() {
		ConnectToSimRailConnect();
	}

	public override void _Process(double delta) {
		socket.Poll();
		WebSocketPeer.State state = socket.GetReadyState();
		if (state == WebSocketPeer.State.Open) {
			while (socket.GetAvailablePacketCount() > 0) {
				string packet = Encoding.UTF8.GetString(socket.GetPacket());
				if (packet.Contains("\"type\":\"error\"")) {
					GD.PrintErr("SimRailConnect command error: " + packet);
				}
			}

			FlushQueuedCommands();
		} else if (state == WebSocketPeer.State.Closed) {
			reconnectInSeconds -= delta;
			if (reconnectInSeconds <= 0) {
				ConnectToSimRailConnect();
			}
		}
	}

	public void Release(){
		SendCommand("setBrake", 0.0);
	}

	public void Brake(){
		SendCommand("setBrake", 1.0);
	}

	public void EmergencyBrake(){
		SendCommand("emergencyBrake", true);
	}

	public void release(){
		Release();
	}

	public void brake(){
		Brake();
	}

	public void emergency_brake(){
		EmergencyBrake();
	}

	private void ConnectToSimRailConnect() {
		if (socket.GetReadyState() == WebSocketPeer.State.Connecting
			|| socket.GetReadyState() == WebSocketPeer.State.Open) {
			return;
		}

		cfg.Load("user://config.cfg");
		currentUrl = cfg.GetValue("SimRailConnect", "url", UpdateMirrorOptionButton.DefaultSimRailConnectUrl).AsString();
		Error error = socket.ConnectToUrl(currentUrl);
		reconnectInSeconds = 2.5;
		if (error != Error.Ok) {
			GD.PrintErr("SimRailConnect command WebSocket connect failed: " + error);
		}
	}

	private void SendCommand(string command, Variant value) {
		string id = "sr-ats-" + command + "-" + Time.GetTicksMsec();
		string message = Json.Stringify(new Godot.Collections.Dictionary {
			["type"] = "command",
			["id"] = id,
			["command"] = command,
			["value"] = value
		});

		if (socket.GetReadyState() == WebSocketPeer.State.Open) {
			socket.SendText(message);
		} else {
			queuedCommands.Add(message);
		}
	}

	private void FlushQueuedCommands() {
		while (queuedCommands.Count > 0) {
			socket.SendText(queuedCommands[0]);
			queuedCommands.RemoveAt(0);
		}
	}

	public string DebugWsUrl => currentUrl;
	public string DebugWsState => socket.GetReadyState().ToString();
	public int DebugQueuedCommandCount => queuedCommands.Count;
	public double DebugReconnectInSeconds => reconnectInSeconds;
}
