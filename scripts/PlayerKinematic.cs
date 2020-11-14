using Godot;
using System;

public class PlayerKinematic : KinematicBody2D
{
	
	PackedScene BulletScene = (PackedScene) ResourceLoader.Load("res://scenes/Bullet.tscn");
		
	public int Speed = 100;
	private Vector2 _velocity = new Vector2();
	private AnimatedSprite sprite;
	private CollisionShape2D collisionShape2D;
	private Particles2D bloodParticles;
	private Camera2D camera2D;
	private RayCast2D rayCast2D;
	
	private AudioStreamPlayer2D[] crunchSounds = new AudioStreamPlayer2D[4];

	private bool isUp = false;
	private Random rnd = new Random();
	private int walkSoundFrame = 0;
	private bool animationPlaying = false;
	private bool shooting = false;
	private int bulletIndex = 0;
	private int maxBullets = 10;
	
	private BulletNode[] bulletNodes;
	
	private float rateOfFire = 0.1f;
	private float elapsedTime = 0;
	private float lastFireTime = 0;
	
	private int bulletSpread = 40;
	
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
		
		this.bulletNodes = new BulletNode[maxBullets];
				
		for (int i = 0; i < maxBullets; i++) {
			
			BulletNode bulletInstance = (BulletNode) BulletScene.Instance();
			AddChild(bulletInstance);
		
			bulletNodes[i] = bulletInstance;
		}
	}
	
	private void shoot() {
		
		if (elapsedTime - lastFireTime  > rateOfFire) {
			lastFireTime = elapsedTime;
			
			Vector2 randomDisplacement = new Vector2(rnd.Next(bulletSpread) - bulletSpread/2, rnd.Next(bulletSpread) - bulletSpread/2);
		
			bulletNodes[bulletIndex].shoot(GetGlobalMousePosition() - Position + randomDisplacement);
			
			rayCast2D.CastTo = GetGlobalMousePosition() - Position + randomDisplacement;
			if (rayCast2D.IsColliding() && ((Node)rayCast2D.GetCollider()).Name.Contains("Enemy")) {
				((Enemy) rayCast2D.GetCollider()).TakeDamage(20);
			}
	
			bulletIndex++;
			if (bulletIndex >= maxBullets) {
				bulletIndex = 0;
			}
		}
		
	}

	public void GetInput(float dt)
	{
		elapsedTime += dt;
		// Detect up/down/left/right keystate and only move when pressed
		_velocity.x = slowAxis(_velocity.x);
		_velocity.y = slowAxis(_velocity.y);

		if (Input.IsActionPressed("ui_up")) {
			_velocity.y -= Speed;
		} if (Input.IsActionPressed("ui_down")) {
			_velocity.y += Speed;
		} if (Input.IsActionPressed("ui_left")) {
			_velocity.x -= Speed;
		} if (Input.IsActionPressed("ui_right")) {
			_velocity.x += Speed;
		} if (Input.IsActionPressed("shoot")) {
			shoot();
		}
		
		SetAnimation(_velocity);
		
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
	
	GetInput(delta);

	MoveAndSlide(_velocity);
	
	// check for collision with enemy
	//for (int i = 0; i < GetSlideCount(); i++) {
	//	var collision = GetSlideCollision(i);
	//	if (((Node)collision.Collider).Name.Contains("Enemy")) {
	//		death();
	//	}
	//}
	
  }

	public void death() {
	}
		
	private void _on_Sprite_frame_changed()
	{
		if (sprite.Animation.Contains("walk")) {
			
			if (sprite.Frame % 2 == 1) {
				crunchSounds[(walkSoundFrame++)%4].Play();
			}
			
		}
	}
	
}



