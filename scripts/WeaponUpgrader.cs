using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponUpgrader : Node2D
{
	private Button upgradeButton;
	private Label label;
	private AudioStreamPlayer2D upgradeSound;
	private PlayerKinematic player = null;
	private int weaponUpgradeCostMultiplier = 20;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		upgradeButton = GetNode<Button>("UpgradeButton");
		label = GetNode<Label>("Label");
		upgradeSound = GetNode<AudioStreamPlayer2D>("UpgradeSound");
	}
	
	public void ShowOptions(PlayerKinematic player) {
		
		this.player = player;
		int cost = player.GetWeaponDamage() * weaponUpgradeCostMultiplier;
		label.Text = cost.ToString();
		
		if (player.GetSpendingMoney() < cost) {
			upgradeButton.Disabled = true;
		} else {
			upgradeButton.Disabled = false;
		}
		
		Visible = true;
		
	}
	
	private void _on_UpgradeButton_pressed()
	{
		
		int cost = player.GetWeaponDamage() * weaponUpgradeCostMultiplier;
		if (player.GetSpendingMoney() >= cost) {
			upgradeSound.Play();
			player.Spend(cost);
			player.SetWeaponDamage(player.GetWeaponDamage() * 2);
		}
		Visible = false;
		upgradeButton.Disabled = false;
	}
}

