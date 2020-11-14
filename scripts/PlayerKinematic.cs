using Godot;
using System;

public class PlayerKinematic : KinematicBody2D
{
	
	PackedScene BulletScene = (PackedScene) ResourceLoader.Load("res://scenes/Bullet.tscn");
		
	public int normalSpeed = 300;
	public int shootingSpeed = 150;
	public int speed = 100;
	private Vector2 _velocity = new Vector2();
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;
	private Particles2D bloodParticles;
	private Camera2D camera2D;
	private RayCast2D rayCast2D;
	private TileMap tileMap;
	private InGameUI inGameUI;
	private EndGameUI endGameUI;
	
	private AudioStreamPlayer2D[] crunchSounds = new AudioStreamPlayer2D[4];

	private bool isUp = false;
	private Random rnd = new Random();
	private int walkSoundFrame = 0;
	private bool animationPlaying = false;
	private bool shooting = false;
	private int bulletIndex = 0;
	private int maxBullets = 10;
	private float maxHealth = 100;
	private float currentHealth = 100;
	private HealthBar healthBar;
	
	private BulletNode[] bulletNodes;
	
	private float rateOfFire = 0.1f;
	private float elapsedTime = 0;
	private float lastFireTime = 0;
	
	private int bulletSpread = 40;
	
	private bool dead = false;
	private bool endGame = false;
	
	private RoundState roundState = RoundState.TOP_DOWN;
	private TurretCreator turretCreatorNode;
	
	private int spendingMoney = 150;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite>("Sprite");
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		bloodParticles = GetNode<Particles2D>("BloodParticles");
		camera2D = GetNode<Camera2D>("Camera2D");
		
		crunchSounds[0] = GetNode<AudioStreamPlayer2D>("sounds/CrunchSoundOne");
		crunchSounds[1] = GetNode<AudioStreamPlayer2D>("sounds/CrunchSoundTwo");
		crunchSounds[2] = GetNode<AudioStreamPlayer2D>("sounds/CrunchSoundThree");
		crunchSounds[3] = GetNode<AudioStreamPlayer2D>("sounds/CrunchSoundFour");
		rayCast2D = GetNode<RayCast2D>("RayCast2D");
		
		healthBar = GetNode<HealthBar>("HealthBarNode");
		
		this.inGameUI = GetNode<InGameUI>("Camera2D/HudLayer/UI");
		this.tileMap = GetNode<TileMap>("../Navigation2D/TileMap");
		this.endGameUI = GetNode<EndGameUI>("Camera2D/HudLayer/EndGameUI");
		
		this.bulletNodes = new BulletNode[maxBullets];
				
		for (int i = 0; i < maxBullets; i++) {
			
			BulletNode bulletInstance = (BulletNode) BulletScene.Instance();
			AddChild(bulletInstance);
		
			bulletNodes[i] = bulletInstance;
		}
		endGameUI.Visible = false;
		
		inGameUI.SetSpendingMoney(spendingMoney);
	}
	
	public void AddTurret(TurretCreator turretCreatorNode) {
		this.turretCreatorNode = turretCreatorNode;
		turretCreatorNode.SetPlayer(this);
	}
	
	public void SetEnemiesLeft(int enemiesLeft) {
		inGameUI.SetEnemiesLeft(enemiesLeft);
	}
	
	public void SetRound(int round) {
		inGameUI.SetRound(round);
	}
	
	public void SetRoundState(RoundState roundState) {
		this.roundState = roundState;
		inGameUI.SetRoundState(roundState);
		if (roundState == RoundState.TOP_DOWN) {
			turretCreatorNode.Visible = false;
		}
	}
	
	private void shoot() {
		
		if (elapsedTime - lastFireTime  > rateOfFire) {
			lastFireTime = elapsedTime;
			
			Vector2 randomDisplacement = new Vector2(rnd.Next(bulletSpread) - bulletSpread/2, rnd.Next(bulletSpread) - bulletSpread/2);
			
			rayCast2D.CastTo = GetGlobalMousePosition() - Position + randomDisplacement;
			if (rayCast2D.IsColliding()) {
				if (((Node)rayCast2D.GetCollider()).Name.Contains("Enemy")) {
					((Enemy) rayCast2D.GetCollider()).TakeDamage(20);
				}
				bulletNodes[bulletIndex].shoot(rayCast2D.GetCollisionPoint() - Position + randomDisplacement);
			} else {
				bulletNodes[bulletIndex].shoot(GetGlobalMousePosition() - Position + randomDisplacement);
			}
	
			bulletIndex++;
			if (bulletIndex >= maxBullets) {
				bulletIndex = 0;
			}
		}
		
	}
	
	public override void _Input(InputEvent @event)
	{
		if (roundState == RoundState.TOWER_DEFENCE) {
			// Mouse in viewport coordinates.
			if (@event is InputEventMouseButton eventMouseButton) {
				int tile = tileMap.GetCellv(tileMap.WorldToMap(GetGlobalMousePosition()));
				if (tile == 0) {
					Vector2 pos = GetGlobalMousePosition();
					var posXDiff = pos.x % 64;
					var posYDiff = pos.y % 64;
					if (pos.x < 0) posXDiff += 64;
					if (pos.y < 0) posYDiff += 64;
					turretCreatorNode.ShowOptions(this);
					turretCreatorNode.Position = pos - new Vector2(posXDiff, posYDiff);
					turretCreatorNode.Visible = true;
				}
			}
		}
	}
	
	public int GetSpendingMoney() {
		return spendingMoney;
	}
	
	public void Spend(int spend) {
		this.spendingMoney -= spend;
		inGameUI.SetSpendingMoney(spendingMoney);
	}

	public void GetInput(float dt)
	{
		elapsedTime += dt;
		// Detect up/down/left/right keystate and only move when pressed
		_velocity.x = 0;
		_velocity.y = 0;

		if (Input.IsActionPressed("ui_up")) {
			_velocity.y -= 1;
		} if (Input.IsActionPressed("ui_down")) {
			_velocity.y += 1;
		} if (Input.IsActionPressed("ui_left")) {
			_velocity.x -= 1;
		} if (Input.IsActionPressed("ui_right")) {
			_velocity.x += 1;
		} 
		
		if (roundState == RoundState.TOWER_DEFENCE) {
			speed = normalSpeed;
			
		} else {
			if (Input.IsActionPressed("shoot")) {
				shoot();
				speed = shootingSpeed;
			} else {
				speed = normalSpeed;
			}
		}
		
		SetAnimation(_velocity);
		
		_velocity = _velocity.Normalized() * speed;
		
	}
	
	private void SetAnimation(Vector2 velocity) {
		if (velocity.y == 0 && velocity.x == 0) {
			// idle	
			if (isUp) {
				sprite.Play("idleUp");
			} else {
				sprite.Play("idleDown");
			}
		} else if (velocity.y < 0) {
			// going up
			isUp = true;
			if (velocity.x < 0) {
				// going left
				sprite.Play("walkLeftUp");
			} else {
				sprite.Play("walkRightUp");
			}
		} else {
			// going down
			isUp = false;
			if (velocity.x < 0) {
				// going left
				sprite.Play("walkLeftDown");
			} else {
				sprite.Play("walkRightDown");
			}
		}
	}
	
	private float slowAxis(float a) {
		a = a / 2;
		if (Math.Abs(a) < 10) {
			a = 0;
		}
		return a;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{

		if (!dead) {
			GetInput(delta);
			MoveAndSlide(_velocity);
		}

	}

	public void TakeDamage(float damage) {
		if (!dead) {
			currentHealth -= damage;
			healthBar.UpdateHealthBarPercentage(currentHealth/maxHealth * 100);
			if (currentHealth <= 0) {
				death();
			}
		}
	}
	
	public void death() {
		GD.Print("DEAD");
		dead = true;
		sprite.Play("death");
	}
		
	private void _on_Sprite_frame_changed()
	{
		if (sprite.Animation.Contains("walk")) {
			
			if (sprite.Frame % 2 == 1) {
				crunchSounds[(walkSoundFrame++)%4].Play();
			}
			
		}
	}
	
	public void EndGame(float timeSurvived, int round, int killCount) {
		inGameUI.Visible = false;
		endGameUI.Show(round, killCount, timeSurvived);
	}
	
	public bool IsEndGame() {
		return endGame;
	}
	
	private void _on_Sprite_animation_finished()
	{
		if (dead) {
			endGame = true;
		}
	}
	
}





