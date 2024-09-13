using Godot;
using System;

public partial class Back : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Pressed()
    {
        SceneTree st = this.GetTree();
		st.ChangeSceneToFile("res://main.tscn");
    }
}
