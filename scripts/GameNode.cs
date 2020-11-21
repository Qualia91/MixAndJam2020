using Godot;
using System;
using System.Collections.Generic;

public class GameNode : Node2D
{
	
	private PackedScene EnemyScene = (PackedScene) ResourceLoader.Load("res://scenes/Enemy.tscn");
	private PackedScene TurretCreatorScene = (PackedScene) ResourceLoader.Load("res://scenes/TurretCreator.tscn");
	private PackedScene WeaponUpgraderScene = (PackedScene) ResourceLoader.Load("res://scenes/WeaponUpgrader.tscn");
	private PackedScene HealthDropScene = (PackedScene) ResourceLoader.Load("res://scenes/HealthDrop.tscn");
	private PackedScene PointsDropScene = (PackedScene) ResourceLoader.Load("res://scenes/PointsDrop.tscn");
	private PackedScene DamageDropScene = (PackedScene) ResourceLoader.Load("res://scenes/DamageDrop.tscn");
	private PackedScene FireSnakeMapScene = (PackedScene) ResourceLoader.Load("res://scenes/FireSnakeMap.tscn");
	private PackedScene ThePitMapScene = (PackedScene) ResourceLoader.Load("res://scenes/ThePitMap.tscn");
	private PackedScene MountainMapScene = (PackedScene) ResourceLoader.Load("res://scenes/MountainMap.tscn");
	
	private int maxEnemies = 20;
	private int enemiesSpawned = 0;
	private int deadEnemies = 0;
	private Enemy[] enemies = null;
	
	private Vector2[] startingPosition;
	private int startingPositionCount;
	private int startingPositionIndex = 0;
	
	private int weaponUpgradeTileIndex = 20;
	private int shieldTileIndex = 21;
	private int floorTileIndex = 22;
	
	private Navigation2D navigation2D;
	private Timer betweenRoundTimer;
	private Timer spawnTimer;
	private PlayerKinematic playerKinematic;
	private AudioStreamPlayer2D roundStartSound;
	private TileMap tileMap;
	private TurretCreator turretCreatorNode;
	private WeaponUpgrader weaponUpgrader;
	private HighScoreSaveData highScoreSaveData;
	
	private int round = 0;
	private int killCount = 0;
	private float timeSurvived = 0;
	
	private Random random = new Random();
	
	private bool saved = false;
	private RoundState roundState = RoundState.TOWER_DEFENCE;
	
	private int levelIndex = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		
		var levelInitVariable = (LevelInitVariable)GetNode("/root/LevelInitVariable");
		
		this.playerKinematic = GetNode<PlayerKinematic>("PlayerKinematic");
		this.betweenRoundTimer = GetNode<Timer>("BetweenRoundTimer");
		this.spawnTimer = GetNode<Timer>("SpawnTimer");
		this.roundStartSound = GetNode<AudioStreamPlayer2D>("RoundStartSound");
		this.highScoreSaveData = GetNode<HighScoreSaveData>("HighScoreSaveData");
		
		this.turretCreatorNode = (TurretCreator) TurretCreatorScene.Instance();
		this.turretCreatorNode.init(this);
		this.turretCreatorNode.Visible = false;
		this.weaponUpgrader = (WeaponUpgrader) WeaponUpgraderScene.Instance();
		this.weaponUpgrader.Visible = false;
		AddChild(turretCreatorNode);
		AddChild(weaponUpgrader);
		playerKinematic.AddTurret(turretCreatorNode);
		playerKinematic.AddWeaponUpgrader(weaponUpgrader);
		
		this.playerKinematic.SetRoundState(RoundState.TOWER_DEFENCE);
		
		enemies = new Enemy[maxEnemies];
		
		playerKinematic.SetRound(round);
		
		saved = false;
		
		this.levelIndex = levelInitVariable.levelIndex;
		Init(levelIndex);
	}
	
	public TileMap GetTileMap() {
		return tileMap;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (roundState == RoundState.TOWER_DEFENCE) {
			// Mouse in viewport coordinates.
			if (@event is InputEventMouseButton eventMouseButton) {
				int tile = tileMap.GetCellv(tileMap.WorldToMap(GetGlobalMousePosition()));
				if (tile == weaponUpgradeTileIndex) {
					Vector2 pos = GetGlobalMousePosition();
					var posXDiff = pos.x % 64;
					var posYDiff = pos.y % 64;
					if (pos.x < 0) posXDiff += 64;
					if (pos.y < 0) posYDiff += 64;
					weaponUpgrader.ShowOptions(playerKinematic);
					weaponUpgrader.Position = pos - new Vector2(posXDiff, posYDiff);
				} else if (tile == shieldTileIndex) {
					Vector2 pos = GetGlobalMousePosition();
					var posXDiff = pos.x % 64;
					var posYDiff = pos.y % 64;
					if (pos.x < 0) posXDiff += 64;
					if (pos.y < 0) posYDiff += 64;
					weaponUpgrader.ShowOptions(playerKinematic);
					weaponUpgrader.Position = pos - new Vector2(posXDiff, posYDiff);
				} else if (tile != floorTileIndex) {
					Vector2 pos = GetGlobalMousePosition();
					var posXDiff = pos.x % 64;
					var posYDiff = pos.y % 64;
					if (pos.x < 0) posXDiff += 64;
					if (pos.y < 0) posYDiff += 64;
					turretCreatorNode.ShowOptions(playerKinematic);
					turretCreatorNode.Position = pos - new Vector2(posXDiff, posYDiff);
				} 
			}
		}
	}
	
	public void Init(int mapIndex) {
		if (mapIndex == 1) {
			this.navigation2D = (Navigation2D) FireSnakeMapScene.Instance();
			AddChild(navigation2D);
			
			startingPosition = new Vector2[2];
			startingPositionCount = 2;
			startingPosition[0] = new Vector2(3008, 0);
			startingPosition[1] = new Vector2(3008, -128);
			
			weaponUpgradeTileIndex = 20;
			shieldTileIndex = 21;
			floorTileIndex = 22;
		} else if (mapIndex == 2) {
			this.navigation2D = (Navigation2D) MountainMapScene.Instance();
			AddChild(navigation2D);
			
			startingPosition = new Vector2[3];
			startingPositionCount = 3;
			startingPosition[0] = new Vector2(-1408, 1664);
			startingPosition[1] = new Vector2(-64, 1664);
			startingPosition[2] = new Vector2(1152, 1664);
			
			weaponUpgradeTileIndex = 20;
			shieldTileIndex = 21;
			floorTileIndex = 22;
		} else {
			this.navigation2D = (Navigation2D) ThePitMapScene.Instance();
			AddChild(navigation2D);
			
			startingPosition = new Vector2[4];
			startingPositionCount = 4;
			startingPosition[0] = new Vector2(2100, 0);
			startingPosition[1] = new Vector2(-2100, 0);
			startingPosition[2] = new Vector2(0, 2100);
			startingPosition[3] = new Vector2(0, -2100);
			
			weaponUpgradeTileIndex = 20;
			shieldTileIndex = 21;
			floorTileIndex = 22;
		}
		this.tileMap = navigation2D.GetNode<TileMap>("TileMap");
	}
	
	private void StartRound(int roundNumber) {
		
		if (roundNumber % 5 == 0) {
			maxEnemies+=2;
		}
		
		if (roundNumber % 10 == 0) {
			spawnTimer.WaitTime *= 0.8f;
		}
		
		enemies = new Enemy[maxEnemies];
		
		for (int i = 0; i < maxEnemies; i++) {
			Enemy enemyInstance = (Enemy) EnemyScene.Instance();
			enemyInstance.Position = new Vector2(float.MaxValue, float.MaxValue);
			enemyInstance.SetDifficulty(round);
			enemyInstance.giveNavigation2D(navigation2D);
			enemies[i] = enemyInstance;
		}
		
		this.playerKinematic.SetRound(roundNumber);
		this.playerKinematic.SetEnemiesLeft(maxEnemies);
		roundState = RoundState.TOP_DOWN;
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
			highScoreSaveData.AddScore(round, levelIndex);
			if (!saved) {
				saved = true;
				SaveGame();
			}
		} else {
		
			timeSurvived += delta;
			
			for (int i = 0; i < maxEnemies; i++) {
				if (enemies[i] != null) {
					if (enemies[i].ToRemove()) {
						
						if (random.NextDouble() < 0.1) {	
							PointsDrop pointsDrop = (PointsDrop) PointsDropScene.Instance();
							pointsDrop.Position = enemies[i].Position;
							AddChild(pointsDrop);
						} else if (random.NextDouble() < 0.1) {	
							HealthDropNode healthDrop = (HealthDropNode) HealthDropScene.Instance();
							healthDrop.Position = enemies[i].Position;
							AddChild(healthDrop);
						} else if (random.NextDouble() < 0.01) {	
							DamageDrop damageDrop = (DamageDrop) DamageDropScene.Instance();
							damageDrop.Position = enemies[i].Position;
							AddChild(damageDrop);
						}
						
						enemies[i].QueueFree();
						enemies[i] = null;
						deadEnemies++;
						playerKinematic.Spend(-10);
						playerKinematic.EnemyKilled();
						playerKinematic.SetEnemiesLeft(maxEnemies - deadEnemies);
						killCount++;
						
						
					}	
				}
			}
			
			if (maxEnemies - deadEnemies == 0) {
				this.playerKinematic.SetRoundState(RoundState.TOWER_DEFENCE);
				roundState = RoundState.TOWER_DEFENCE;
				deadEnemies = 0;
				betweenRoundTimer.Start();
			}
		}
	}
	
	public void SaveGame()
	{
		var saveGame = new File();
		saveGame.Open("user://savegame.save", File.ModeFlags.Write);

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			if (saveNode.Filename.Empty())
			{
				GD.Print(String.Format("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
				continue;
			}

			if (!saveNode.HasMethod("Save"))
			{
				GD.Print(String.Format("persistent node '{0}' is missing a Save() function, skipped", saveNode.Name));
				continue;
			}

			var nodeData = saveNode.Call("Save");

			saveGame.StoreLine(JSON.Print(nodeData));
		}

		saveGame.Close();
	}
	
	private void _on_BetweenRoundTimer_timeout()
	{	
		round++;
		StartRound(round);
		roundStartSound.Play();
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






