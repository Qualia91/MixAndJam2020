using Godot;
using System;

public class HealthBar : Node2D
{
	
	private TextureProgress textureProgress;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textureProgress = GetNode<TextureProgress>("TextureProgress");
	}

	public void UpdateHealthBarPercentage(float percent) {
		textureProgress.Value = percent;
	}
}
