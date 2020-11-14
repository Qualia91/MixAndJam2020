using Godot;
using System;

public class Menu : Node2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	private void _on_Button_pressed()
	{
		GetTree().ChangeScene("res://scenes/MainScene.tscn");
	}
	
	private void _on_Quit_pressed()
	{
		GetTree().Quit();
	}
}





