using Godot;
using System;

public class Enemy : KinematicBody2D
{
	
	PackedScene EnemyDamageTakenScene = (PackedScene) ResourceLoader.Load("res://scenes/EnemyDamageTaken.tscn");
		
	private AudioStreamPlayer2D walkSound;
	private AnimatedSprite sprite;
	private HealthBar healthBar;
	private Vector2[] path = null;
	private int currentPathPointIndex = 0;
	private Vector2 _velocity = new Vector2(0, 0);
	private float speed = 200;
	private int pathSize = 0;
	private Random random = new Random();
	private AnimationPlayer anim;
	private bool animationPlaying = false;
	private Navigation2D navigation2D;
	private KinematicBody2D player;
	private CollisionShape2D collisionShape2D;
	private Timer attackTimer;
	private int damageTakenNodesCount = 10;
	private int damageTakenNodesIndex = 0;
	private EnemyDamageTaken[] damageTakenNodes;
	private float maxHealth = 100;
	private float currentHealth;
	private bool isUp = false;
	private bool dead = false;
	private bool toRemove = false;
	private bool ableToAttck = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		this.player = GetNode<KinematicBody2D>("../PlayerKinematic");
		walkSound = GetNode<AudioStreamPlayer2D>("sounds/walk");
		healthBar = GetNode<HealthBar>("HealthBarNode");
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		attackTimer = GetNode<Timer>("AttackTimer");
		this.damageTakenNodes = new EnemyDamageTaken[damageTakenNodesCount];
				
		for (int i = 0; i < damageTakenNodesCount; i++) {
			
			EnemyDamageTaken damageTakenInstance = (EnemyDamageTaken) EnemyDamageTakenScene.Instance();
			AddChild(damageTakenInstance);
			damageTakenNodes[i] = damageTakenInstance;
		}
		
		GetPath();
	}
	
	public void SetDifficulty(int round) {
		maxHealth = maxHealth + (round * round);
		currentHealth = maxHealth;
	}
	
	public void TakeDamage(float damage) {
		damageTakenNodes[damageTakenNodesIndex].action();
		
		currentHealth -= damage;
		healthBar.UpdateHealthBarPercentage(currentHealth/maxHealth * 100);
		if (currentHealth < 0) {
			death();
		}

		damageTakenNodesIndex++;
		if (damageTakenNodesIndex >= damageTakenNodesCount) {
			damageTakenNodesIndex = 0;
		}
	}
	
	public bool IsDead() {
		return this.dead;
	}
	
	public void death() {
		this.dead = true;
		collisionShape2D.Disabled = true;
		sprite.Play("death");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if (!dead) {
			// get direction to next point of path
			Vector2 diff = path[currentPathPointIndex] - Position;
			Vector2 directionToNextPointNorm = diff.Normalized();
					
			if (diff.LengthSquared() < 150) {
				currentPathPointIndex++;
			}
				
			_velocity = directionToNextPointNorm * speed;
			
			SetAnimation(_velocity);
			
			MoveAndSlide(_velocity);
			
			// check for collision with enemy
			if (ableToAttck) {
				for (int i = 0; i < GetSlideCount(); i++) {
					var collision = GetSlideCollision(i);
					if (((Node)collision.Collider).Name.Contains("Player")) {
						((PlayerKinematic)collision.Collider).TakeDamage(20);
						ableToAttck = false;
						attackTimer.Start(1);
					}
				}	
			}
		}
		
	}
	
	public void giveNavigation2D(Navigation2D navigation2D) {
		this.navigation2D = navigation2D;
	}
	
	private void SetAnimation(Vector2 velocity) {
		if (velocity.y < 0) {
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
	
	private void _on_AnimatedSprite_frame_changed()
	{
		if (sprite.Animation.Contains("walk")) {
			
			if (sprite.Frame % 2 == 1) {
				walkSound.Play();
			}
			
		}
	}
	
	private void _on_Timer_timeout()
	{
		GetPath();
	}
	
	private void GetPath() {
		if (!dead) {
			path = navigation2D.GetSimplePath(Position, player.Position, false);
			this.path = path;
			this.pathSize = 0;
			foreach (Vector2 item in path) {
				pathSize++;
			}
			currentPathPointIndex = 1;
		}
	}
	
	public bool ToRemove() {
		return toRemove;
	}
	
	private void _on_AnimatedSprite_animation_finished()
	{
		if (dead) {
			toRemove = true;
		}
	}
	
	private void _on_AttackTimer_timeout()
	{
		this.ableToAttck = true;
	}
}












