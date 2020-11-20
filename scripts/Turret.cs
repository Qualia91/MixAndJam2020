using Godot;
using System;

public class Turret : Node2D
{
	
	PackedScene BulletScene = (PackedScene) ResourceLoader.Load("res://scenes/Bullet.tscn");
		
	private GameNode gameNode;
	private Label levelLabel;
	private Timer shotTimer;
	private Timer sensingTimer;
	private Enemy target = null;
	private BulletNode[] bulletNodes;
	private int maxBullets = 10;
	private int bulletIndex = 0;
	private int rangeBase = 200;
	private int range = 200;
	private int rangeSquared = 40000;
	private Vector2 offset = new Vector2(32, 32);
	private int level = 1;
	private int damageBase = 20;
	private int costBase = 300;
	private int damage = 20;
	private int cost = 300;
	private float waitTime = 1;
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.gameNode = GetNode<GameNode>("../../GameNode");
		this.levelLabel = GetNode<Label>("LevelLabel");
		this.shotTimer = GetNode<Timer>("ShotTimer");
		this.sensingTimer = GetNode<Timer>("SensingTimer");
		
		this.bulletNodes = new BulletNode[maxBullets];
				
		for (int i = 0; i < maxBullets; i++) {
			
			BulletNode bulletInstance = (BulletNode) BulletScene.Instance();
			AddChild(bulletInstance);
		
			bulletNodes[i] = bulletInstance;
		}
		
	}
	
	public int GetLevel() {
		return level;
	}
	
	public int GetRange() {
		return range;
	}
	
	public int GetCost() {
		return cost;
	}
	
	public void Upgrade() {
		level++;
		damage = damageBase * level;
		cost = costBase * level;
		costBase += 10;
		levelLabel.Text = level.ToString();
		range = (int) (rangeBase + (rangeBase * ((level - 1) * 0.1f)));
		rangeSquared = range * range;
		
		shotTimer.WaitTime *= 0.9f;
		sensingTimer.WaitTime *= 0.9f;
		
	}

	private void _on_SensingTimer_timeout()
	{
		Enemy[] enemies = gameNode.GetEnemies();
		
		if (enemies != null) {
		
			float distanceAway = float.MaxValue;
			Enemy newTarget = null;
		
			foreach (Enemy enemy in enemies) {
				if (enemy != null && !enemy.IsDead()) {
					float dist = Position.DistanceSquaredTo(enemy.Position);
					if (dist < rangeSquared && dist < distanceAway) {
						dist = distanceAway;
						newTarget = enemy;
					}
				}
			}
			
			target = newTarget;
		}
	}

	private void _on_ShotTimer_timeout()
	{
		if (this.target != null) {
			
			bulletNodes[bulletIndex].shoot(offset, target.Position - Position);
			target.TakeDamage(damage);
	
			bulletIndex++;
			if (bulletIndex >= maxBullets) {
				bulletIndex = 0;
			}
		}
	}
	
}






