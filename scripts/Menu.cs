using Godot;
using System;
using System.Collections.Generic;

public class Menu : Node2D
{
	
	private Label pitScoreLabel;
	private Label snakeScoreLabel;
	private Label mountainScoreLabel;
	private Node2D levelSelect;
	private Node2D startNode;
	private AnimationPlayer animationPlayer;
	
	public override void _Ready()
	{
		pitScoreLabel = GetNode<Label>("LevelSelect/PitScoreLabel");
		snakeScoreLabel = GetNode<Label>("LevelSelect/SnakeScoreLabel");
		mountainScoreLabel = GetNode<Label>("LevelSelect/MountainScoreLabel");
		levelSelect = GetNode<Node2D>("LevelSelect");
		startNode = GetNode<Node2D>("StartNode");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		LoadGame();
	}
	
	private void _on_FireSnake_pressed()
	{
		var levelInitVariable = (LevelInitVariable)GetNode("/root/LevelInitVariable");
		levelInitVariable.levelIndex = 1;
		GetTree().ChangeScene("res://scenes/GameScene.tscn");
	}
	
	private void _on_ThePitButton_pressed()
	{
		var levelInitVariable = (LevelInitVariable)GetNode("/root/LevelInitVariable");
		levelInitVariable.levelIndex = 0;
		GetTree().ChangeScene("res://scenes/GameScene.tscn");
	}
	
	private void _on_Mountain_pressed()
	{
		var levelInitVariable = (LevelInitVariable)GetNode("/root/LevelInitVariable");
		levelInitVariable.levelIndex = 2;
		GetTree().ChangeScene("res://scenes/GameScene.tscn");
	}
	
	private void _on_Quit_pressed()
	{
		GetTree().Quit();
	}
	
	private void _on_Play_pressed()
	{
		animationPlayer.Play("StartAnimation");
	}
	
	private void _on_Button_pressed()
	{
		animationPlayer.Play("BackAnimation");
	}
	
	public void LoadGame()
	{
		var saveGame = new File();
		if (!saveGame.FileExists("user://savegame.save"))
			return;

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
			saveNode.QueueFree();

		saveGame.Open("user://savegame.save", File.ModeFlags.Read);

		while (saveGame.GetPosition() < saveGame.GetLen())
		{
			// Get the saved dictionary from the next line in the save file
			var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);

			// Firstly, we need to create the object and add it to the tree and set its position.
			var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
			var newObject = (Node)newObjectScene.Instance();

			// Now we set the remaining variables.
			foreach (KeyValuePair<String, object> entry in nodeData)
			{
				string key = entry.Key.ToString();
				if (key == "Filename" || key == "Parent")
					continue;
				if (key == "PIT_HIGH_SCORE")
					pitScoreLabel.Text = "BEST ROUND: " + entry.Value.ToString();
				if (key == "SNAKE_HIGH_SCORE")
					snakeScoreLabel.Text = "BEST ROUND: " + entry.Value.ToString();
				if (key == "MOUNTAIN_HIGH_SCORE")
					mountainScoreLabel.Text = "BEST ROUND: " + entry.Value.ToString();
				newObject.Set(key, entry.Value);
			}
		}

		saveGame.Close();
	}
}


