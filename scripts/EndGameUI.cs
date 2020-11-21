using Godot;
using System;

public class EndGameUI : Node2D
{
	private Label roundLabel;
	private Label killCountLabel;
	private Label timeLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.roundLabel = GetNode<Label>("RoundLabel");
		this.killCountLabel = GetNode<Label>("KillCountLabel");
		this.timeLabel = GetNode<Label>("TimeLabel");
	}
	
	public void Show(int round, int kills, float time) {
		
		this.roundLabel.Text = "Round Survived: " + round;
		this.killCountLabel.Text = "Kill Count: " + kills;
		this.timeLabel.Text = "Time Survuved: " + (int) (time/60.0) + "m";
		
		Visible = true;
	}
	
	
	private void _on_MainMenu_pressed()
	{
		GetTree().ChangeScene("res://scenes/Menu.tscn");
	}
}



