using Godot;
using System;

public class Turret : Node2D
{
	
	PackedScene BulletScene = (PackedScene) ResourceLoader.Load("res://scenes/Bullet.tscn");
		
	private MainScene mainScene;
	private Enemy target = null;
	private BulletNode[] bulletNodes;
	private int maxBullets = 10;
	private int bulletIndex = 0;
	private int rangeSquared = 40000;
	private Vector2 offset = new Vector2(32, 32);
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.mainScene = GetNode<MainScene>("../../MainNode");
		
		this.bulletNodes = new BulletNode[maxBullets];
				
		for (int i = 0; i < maxBullets; i++) {
			
			BulletNode bulletInstance = (BulletNode) BulletScene.Instance();
			AddChild(bulletInstance);
		
			bulletNodes[i] = bulletInstance;
		}
	}

	private void _on_SensingTimer_timeout()
	{
		Enemy[] enemies = mainScene.GetEnemies();
		
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
			target.TakeDamage(20);
	
			bulletIndex++;
			if (bulletIndex >= maxBullets) {
				bulletIndex = 0;
			}
		}
	}
	
}






