using Godot;
using System;

public class MainScene : Node2D
{
	
	private PackedScene EnemyScene = (PackedScene) ResourceLoader.Load("res://scenes/Enemy.tscn");
	
	private int maxEnemies = 20;
	private int enemiesSpawned = 0;
	private int deadEnemies = 0;
	private Enemy[] enemies;

	private float lastSpawn = 0;
	private float currentTime = 0;
	private float spawnDelay = 1;
	
	private int startingPositionIndex = 0;
	private int startingPositionCount = 2;
	
	private Vector2[] startinPosition = new Vector2[2];
	
	private Navigation2D navigation2D;
	private PlayerKinematic playerKinematic;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		
		this.navigation2D = GetNode<Navigation2D>("Navigation2D");
		this.playerKinematic = GetNode<PlayerKinematic>("PlayerKinematic");
		startinPosition[0] = new Vector2(1024, 0);
		startinPosition[1] = new Vector2(-1024, 0);
		enemies = new Enemy[maxEnemies];
		
		for (int i = 0; i < maxEnemies; i++) {
			Enemy enemyInstance = (Enemy) EnemyScene.Instance();
			enemyInstance.giveNavigation2D(navigation2D);
			enemies[i] = enemyInstance;
		}
		
		
		playerKinematic.SetRound(1);
		playerKinematic.SetEnemiesLeft(maxEnemies);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (deadEnemies == maxEnemies) {
			GD.Print("HER");
		}
		
		currentTime += delta;
		if (currentTime - lastSpawn > spawnDelay) {
			lastSpawn = currentTime;
			if (enemiesSpawned < maxEnemies) {
				Enemy enemyInstance = enemies[enemiesSpawned];
				startingPositionIndex++;
				enemyInstance.Position = startinPosition[startingPositionIndex % startingPositionCount];
				AddChild(enemyInstance);
				enemiesSpawned++;
			}
		}
		
		for (int i = 0; i < maxEnemies; i++) {
			if (enemies[i] != null) {
				if (enemies[i].ToRemove()) {
					enemies[i].QueueFree();
					enemies[i] = null;
					deadEnemies++;
					playerKinematic.SetEnemiesLeft(maxEnemies - deadEnemies);
				}	
			}
		}
	}
}
