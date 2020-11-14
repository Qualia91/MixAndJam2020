using Godot;
using System;

public class Enemy : KinematicBody2D
{
	
	PackedScene EnemyDamageTakenScene = (PackedScene) ResourceLoader.Load("res://scenes/EnemyDamageTaken.tscn");
		
	private AudioStreamPlayer2D walkSound;
	private AnimatedSprite sprite;
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
	private int damageTakenNodesCount = 20;
	private int damageTakenNodesIndex = 0;
	private EnemyDamageTaken[] damageTakenNodes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		this.player = GetNode<KinematicBody2D>("../PlayerKinematic");
		walkSound = GetNode<AudioStreamPlayer2D>("sounds/walk");
		this.damageTakenNodes = new EnemyDamageTaken[damageTakenNodesCount];
				
		for (int i = 0; i < damageTakenNodesCount; i++) {
			
			EnemyDamageTaken damageTakenInstance = (EnemyDamageTaken) EnemyDamageTakenScene.Instance();
			AddChild(damageTakenInstance);
		
			damageTakenNodes[i] = damageTakenInstance;
		}
	}
	
	public void TakeDamage(float damage) {
		damageTakenNodes[damageTakenNodesIndex].action();

		damageTakenNodesIndex++;
		if (damageTakenNodesIndex >= damageTakenNodesCount) {
			damageTakenNodesIndex = 0;
		}
	}
	
	public void passPlayerReference(KinematicBody2D player) {
		
		this.player = player;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		
		var dir = (player.Position - Position).Normalized();
		_velocity = dir * speed;
		
		// check theres a path. if there is, follow it
		if (currentPathPointIndex != pathSize) {
			// get direction to next point of path
			Vector2 diff = path[currentPathPointIndex] - Position;
			Vector2 directionToNextPointNorm = diff.Normalized();
			
			if (diff.LengthSquared() < 150) {
				currentPathPointIndex++;
			}
			
			_velocity = directionToNextPointNorm * speed;
			
		}
		
		
		
		MoveAndSlide(_velocity);
		
	}
	
	public void giveNavigation2D(Navigation2D navigation2D) {
		this.navigation2D = navigation2D;
	}
	
	private void _on_AnimatedSprite_frame_changed()
	{
		if (sprite.Animation.Contains("walk")) {
			
			if (sprite.Frame % 2 == 1) {
				walkSound.Play();
			}
			
		}
	}
}



