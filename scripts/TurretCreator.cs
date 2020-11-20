using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurretCreator : Node2D
{
	
	PackedScene TurretScene = (PackedScene) ResourceLoader.Load("res://scenes/Turret.tscn");
	private Node2D mainNode;
	private Button buildButton;
	private Label label;
	private AudioStreamPlayer2D buildSound;
	private TextureRect range;
	private PlayerKinematic player = null;
	
	private Dictionary<Index, Turret> turretIndexes = new Dictionary<Index, Turret>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildButton = GetNode<Button>("BuildButton");
		label = GetNode<Label>("Label");
		buildSound = GetNode<AudioStreamPlayer2D>("BuildSound");
		range = GetNode<TextureRect>("Range");
	
	}
	
	public void init(Node2D mainNode) {
		this.mainNode = mainNode;
	}
	
	public void ShowOptions(PlayerKinematic player) {
		
		Index index = new Index((int) Position.x, (int) Position.y);
		int cost = 150;
		if (turretIndexes.ContainsKey(index)) {
			buildButton.Text = "Upgrade";
			label.Text = turretIndexes[index].GetCost().ToString();
			cost = turretIndexes[index].GetCost();
			range.RectScale = new Vector2(turretIndexes[index].GetRange() * 0.005f, turretIndexes[index].GetRange() * 0.005f);
		} else {
			buildButton.Text = "Build Buy";
			label.Text = "150";
			range.RectScale = new Vector2(1, 1);
		}
		
		if (player.GetSpendingMoney() < cost) {
			buildButton.Disabled = true;
		} else {
			buildButton.Disabled = false;
		}
		
		Visible = true;
		
	}
	
	public void SetPlayer(PlayerKinematic player) {
		this.player = player;
	}
	
	private void _on_BuildButton_pressed()
	{
		Index index = new Index((int) Position.x, (int) Position.y);
		if (turretIndexes.ContainsKey(index)) {
			int turrentCost = turretIndexes[index].GetCost();
			Turret turret = turretIndexes[index];
			if (player.GetSpendingMoney() >= turrentCost) {
				turret.Upgrade();
				buildSound.Play();
				player.Spend(turrentCost);
			}
		} else {
			if (player.GetSpendingMoney() >= 150) {
				buildSound.Play();
				player.Spend(150);
				Turret turret = (Turret) TurretScene.Instance();
				turret.Position = Position;
				mainNode.AddChild(turret);
				turretIndexes.Add(index, turret);
			}
		}
		Visible = false;
		buildButton.Disabled = false;
	}
}








