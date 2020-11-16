using Godot;
using System;

public class InGameUI : Node2D
{
	
	private Label enemiesLeftLabel;
	private Label roundLabel;
	private Label spendingMoneyLabel;
	private Label roundState;
	private Label weaponDamageLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.enemiesLeftLabel = GetNode<Label>("EnemiesLeftLabel");
		this.roundLabel = GetNode<Label>("RoundLabel");
		this.spendingMoneyLabel = GetNode<Label>("SpendingMoneyLabel");
		this.roundState = GetNode<Label>("RoundState");
		this.weaponDamageLabel = GetNode<Label>("WeaponDamageLabel");
	}
	
	public void SetRoundState(RoundState roundState) {
		if (roundState == RoundState.TOWER_DEFENCE) {
			this.roundState.Text = "BUILD";
		} else {
			this.roundState.Text = "SHOOT";
		}
	}
	
	public void SetEnemiesLeft(int enemiesLeft) {
		this.enemiesLeftLabel.Text = "Enemies Left: " + enemiesLeft;
	}
	
	public void SetRound(int round) {
		this.roundLabel.Text = "Round: " + round;
	}
	
	public void SetSpendingMoney(int money) {
		this.spendingMoneyLabel.Text = "Spending Money: " + money;
	}
	
	public void SetWeaponDamage(float weaponDamage) {
		this.weaponDamageLabel.Text = "Weapon Damage: " + weaponDamage;
	}

}
