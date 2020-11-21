using Godot;
using System;
using System.Collections;

public class HighScoreSaveData : Node2D
{
	
	private int pitHighScore = 0;
	private int snakeHighScore = 0;
	private int mountainHighScore = 0;

	public override void _Ready()
	{
		
	}
	
	public void AddScore(int score, int levelIndex) {
		
		switch (levelIndex) {
			// pit
			case 0:
				if (score > pitHighScore) {
					this.pitHighScore = score;
				}
				break;
			// snake
			case 1:
				if (score > snakeHighScore) {
					this.snakeHighScore = score;
				}
				break;
			// mountain
			case 2:
				if (score > mountainHighScore) {
					this.mountainHighScore = score;
				}
				break;
		}
	}

	public Godot.Collections.Dictionary<string, object> Save()
	{
		return new Godot.Collections.Dictionary<string, object>()
		{
			{ "Filename", GetFilename() },
			{ "Parent", GetParent().GetPath() },
			{ "PIT_HIGH_SCORE", pitHighScore },
			{ "SNAKE_HIGH_SCORE", snakeHighScore },
			{ "MOUNTAIN_HIGH_SCORE", mountainHighScore },
		};
	}
}
