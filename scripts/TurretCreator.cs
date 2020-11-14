using Godot;
using System;
using System.Collections;

public class TurretCreator : Node2D
{
	
	PackedScene TurretScene = (PackedScene) ResourceLoader.Load("res://scenes/Turret.tscn");
	private Node2D mainNode;
	private Button buildButton;
	private PlayerKinematic player = null;
	private int cost = 150;
	
	private ArrayList turretIndexes = new ArrayList();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildButton = GetNode<Button>("BuildButton");
	}
	
	public void init(Node2D mainNode) {
		this.mainNode = mainNode;
	}
	
	public void ShowOptions(PlayerKinematic player) {
		if (player.GetSpendingMoney() < cost) {
			buildButton.Disabled = true;
		} else {
			buildButton.Disabled = false;
		}
	}
	
	public void SetPlayer(PlayerKinematic player) {
		this.player = player;
	}

	private void _on_BuildButton_pressed()
	{
		Visible = false;
		Index index = new Index((int) Position.x, (int) Position.y);
		if (player.GetSpendingMoney() >= cost && !turretIndexes.Contains(index)) {
			player.Spend(cost);
			turretIndexes.Add(index);
			Turret turret = (Turret) TurretScene.Instance();
			turret.Position = Position;
			mainNode.AddChild(turret);
		}
	}
}



