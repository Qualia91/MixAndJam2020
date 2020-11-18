using Godot;
using System;

public class InGameUI : Node2D
{
	
	private Label enemiesLeftLabel;
	private Label roundLabel;
	private Label spendingMoneyLabel;
	private Label roundState;
	private Label weaponDamageLabel;
	private ItemList killFeed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.enemiesLeftLabel = GetNode<Label>("EnemiesLeftLabel");
		this.roundLabel = GetNode<Label>("RoundLabel");
		this.spendingMoneyLabel = GetNode<Label>("SpendingMoneyLabel");
		this.roundState = GetNode<Label>("RoundState");
		this.weaponDamageLabel = GetNode<Label>("WeaponDamageLabel");
		this.killFeed = GetNode<ItemList>("KillFeed");
	}
	
	public void AddKill(String kill) {
		killFeed.AddItem(kill);
		
		Timer timer = new Timer();
		AddChild(timer);
		timer.SetOneShot(true);
		timer.SetWaitTime(5);
		timer.Connect("timeout", this, "_timer_callback");
		timer.Autostart = true;
		timer.Start();
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
	
	public void _timer_callback() {
		killFeed.RemoveItem(0);
	}

}
