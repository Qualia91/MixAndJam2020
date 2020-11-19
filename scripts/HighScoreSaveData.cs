using Godot;
using System;
using System.Collections;

public class HighScoreSaveData : Node2D
{
	
	private int highScore = 0;

	public override void _Ready()
	{
		
	}
	
	public void AddScore(int score) {
		if (score > highScore) {
			this.highScore = score;
		}
	}

	public Godot.Collections.Dictionary<string, object> Save()
	{
		return new Godot.Collections.Dictionary<string, object>()
		{
			{ "Filename", GetFilename() },
			{ "Parent", GetParent().GetPath() },
			{ "HIGH_SCORE", highScore },
		};
	}
}
