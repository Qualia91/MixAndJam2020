using Godot;
using System;

public class MainScene : Node2D
{
	
	private PackedScene EnemyScene = (PackedScene) ResourceLoader.Load("res://scenes/Enemy.tscn");
	private PackedScene TurretCreatorScene = (PackedScene) ResourceLoader.Load("res://scenes/TurretCreator.tscn");
	
	private int maxEnemies = 20;
	private int enemiesSpawned = 0;
	private int deadEnemies = 0;
	private Enemy[] enemies = null;

	private float lastSpawn = 0;
	private float currentTime = 0;
	private float spawnDelay = 1;
	
	private int startingPositionIndex = 0;
	private int startingPositionCount = 4;
	
	private Vector2[] startingPosition = new Vector2[4];
	
	private Navigation2D navigation2D;
	private Timer betweenRoundTimer;
	private Timer spawnTimer;
	private PlayerKinematic playerKinematic;
	
	private TurretCreator turretCreatorNode;
	
	private int round = 0;
	private int killCount = 0;
	private float timeSurvived = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		this.navigation2D = GetNode<Navigation2D>("Navigation2D");
		this.playerKinematic = GetNode<PlayerKinematic>("PlayerKinematic");
		this.betweenRoundTimer = GetNode<Timer>("BetweenRoundTimer");
		this.spawnTimer = GetNode<Timer>("SpawnTimer");
		
		this.turretCreatorNode = (TurretCreator) TurretCreatorScene.Instance();
		this.turretCreatorNode.init(this);
		this.turretCreatorNode.Visible = false;
		AddChild(turretCreatorNode);
		playerKinematic.AddTurret(turretCreatorNode);
		
		this.playerKinematic.SetRoundState(RoundState.TOWER_DEFENCE);
			
		startingPosition[0] = new Vector2(2100, 0);
		startingPosition[1] = new Vector2(-2100, 0);
		startingPosition[2] = new Vector2(0, 2100);
		startingPosition[3] = new Vector2(0, -2100);
		enemies = new Enemy[maxEnemies];
		
		
		playerKinematic.SetRound(round);
	}
	
	private void StartRound(int roundNumber) {
		
		if (roundNumber % 5 == 0) {
			maxEnemies++;
		}
		
		enemies = new Enemy[maxEnemies];
		
		for (int i = 0; i < maxEnemies; i++) {
			Enemy enemyInstance = (Enemy) EnemyScene.Instance();
			enemyInstance.SetDifficulty(round);
			enemyInstance.giveNavigation2D(navigation2D);
			enemies[i] = enemyInstance;
		}
		
		this.playerKinematic.SetRound(roundNumber);
		this.playerKinematic.SetEnemiesLeft(maxEnemies);
		this.playerKinematic.SetRoundState(RoundState.TOP_DOWN);
		
		spawnTimer.Start();
	}
	
	public Enemy[] GetEnemies() {
		return enemies;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		
		if (playerKinematic.IsEndGame()) {
			playerKinematic.EndGame(timeSurvived, round, killCount);
		} else {
		
			timeSurvived += delta;
			
			for (int i = 0; i < maxEnemies; i++) {
				if (enemies[i] != null) {
					if (enemies[i].ToRemove()) {
						enemies[i].QueueFree();
						enemies[i] = null;
						deadEnemies++;
						playerKinematic.Spend(-10);
						playerKinematic.SetEnemiesLeft(maxEnemies - deadEnemies);
						killCount++;
					}	
				}
			}
			
			if (maxEnemies - deadEnemies == 0) {
				this.playerKinematic.SetRoundState(RoundState.TOWER_DEFENCE);
				deadEnemies = 0;
				betweenRoundTimer.Start();
			}
		}
	}
	
	private void _on_BetweenRoundTimer_timeout()
	{	
		round++;
		StartRound(round);
	}	
	
	private void _on_SpawnTimer_timeout()
	{
		if (enemiesSpawned < maxEnemies) {
			Enemy enemyInstance = enemies[enemiesSpawned];
			if (enemyInstance != null) {
				startingPositionIndex++;
				enemyInstance.Position = startingPosition[startingPositionIndex % startingPositionCount];
				AddChild(enemyInstance);
				enemiesSpawned++;
			}
		} else {
			spawnTimer.Stop();
			enemiesSpawned = 0;
		}
	}
}






