// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. All rights reserved.

using Godot;

public partial class HttpReq : HttpRequest{
    private ConfigFile _cfg = new ConfigFile();

    public async void _on_main_ats_ready(){
        var parent = GetNode<Scene>("..");
        var indicators = GetNode<Indicators>("../Indicators");
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("Train Data", "server", "null").AsString() != "127"){
            while (true){
                GD.Print("getting data from server...");
                var value = Request("https://panel.simrail.eu:8084/trains-open?serverCode=" + _cfg.GetValue("Train Data", "server", "null").AsString());

                if (value == Error.Timeout){
                    GD.PrintErr("REQUEST TIMEOUT");
                    GetNode<AcceptDialog>("../ErrorMsg").Call("ConnectionTimeout");
                    if (!parent.Fail){
                        parent.Fail = true;
                        indicators.Fail(true);
                        indicators.PlayBell();
                    }
                }else if (value == Error.Busy)
                    GD.PrintErr("[HTTP REQUEST ERROR]BUSY");
                else if (value == Error.Unavailable)
                    GD.PrintErr("[HTTP REQUEST ERROR]UNAVAILABLE");
                else if (value != Error.Ok)
                    GD.PrintErr("[HTTP REQUEST ERROR]" + value);
                else if (parent.Fail){
                    parent.Fail = false;
                    indicators.Fail(false);
                    indicators.PlayBell();
                }
                else
                    GetNode<AcceptDialog>("../ErrorMsg").Visible = false;

                await ToSignal(GetTree().CreateTimer(2.5f), "timeout");
            }
        }
    }

    private void _OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body){
        var parent = GetNode<Scene>("..");
        var indicators = GetNode<Indicators>("../Indicators");
        if (responseCode != 200){
            GD.PrintErr("HTTP response code:" + responseCode);
            GetNode<AcceptDialog>("../ErrorMsg").Call("ConnectionErr", (int)responseCode);
            if (!parent.Fail){
                parent.Fail = true;
                indicators.Fail(true);
                indicators.PlayBell();
            }
            return;
        }

        var json = Json.ParseString(System.Text.Encoding.UTF8.GetString(body)).AsGodotDictionary();

        if (json["count"].AsInt32() == 0){
            GetNode<AcceptDialog>("../ErrorMsg").Call("ServerNotFound");
        }

        var dataArray = json["data"].AsGodotArray();
        var data = ReadArray(dataArray);

        if (data != null){
            GetNode<AcceptDialog>("../ErrorMsg").Visible = false;


            if (data.ContainsKey("ControlledBySteamID") && data["ControlledBySteamID"].VariantType != Variant.Type.Nil)
                parent.ControlledBySteamID = data["ControlledBySteamID"].AsString();
            else
                parent.ControlledBySteamID = "null";

            parent.InBorderStationArea = data["InBorderStationArea"].AsBool();
            parent.Latititute = data["Latititute"].AsDouble();
            parent.Longitute = data["Longitute"].AsDouble();
            parent.Velocity = data["Velocity"].AsInt32();
            parent.SignalInFront = data["SignalInFront"].AsString();
            parent.DistanceToSignalInFront = data["DistanceToSignalInFront"].AsDouble();
            parent.SignalInFrontSpeed = data["SignalInFrontSpeed"].AsInt32();
            parent.VDDelayedTimetableIndex = data["VDDelayedTimetableIndex"].AsDouble();

            var time = Time.GetTimeDictFromSystem();
            parent.UpdateTime = $"{time["hour"].AsString().PadLeft(2, '0')}:{time["minute"].AsString().PadLeft(2, '0')}:{time["second"].AsString().PadLeft(2, '0')}";
        }else{
            GetNode<AcceptDialog>("../ErrorMsg").Call("TrainNotFound");
        }
    }

    private Godot.Collections.Dictionary ReadArray(Godot.Collections.Array array){
        foreach (Godot.Collections.Dictionary data in array){
            if (data["TrainNoLocal"].AsString() == _cfg.GetValue("Train Data", "trainNumber").AsString()){
                return data["TrainData"].AsGodotDictionary();
            }
        }
        return null;
    }
}