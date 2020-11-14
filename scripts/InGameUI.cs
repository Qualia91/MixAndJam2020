using Godot;
using System;

public class InGameUI : Node2D
{
	
	private Label enemiesLeftLabel;
	private Label roundLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.enemiesLeftLabel = GetNode<Label>("EnemiesLeftLabel");
		this.roundLabel = GetNode<Label>("RoundLabel");
	}
	
	public void SetEnemiesLeft(int enemiesLeft) {
		this.enemiesLeftLabel.Text = "Enemies Left: " + enemiesLeft;
	}
	
	public void SetRound(int round) {
		this.roundLabel.Text = "Round: " + round;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
