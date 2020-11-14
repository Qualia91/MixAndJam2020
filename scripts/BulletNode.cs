using Godot;
using System;

public class BulletNode : Node2D
{
	
	private AnimationPlayer bulletAnimationPlayer;
	private Particles2D particles2D;
	private Line2D bulletPath;
	private Vector2 ZERO = new Vector2(0, 0);
	private Random rnd = new Random();

	public void shoot(Vector2 end) {
		
		bulletPath.ClearPoints();
		bulletPath.AddPoint(ZERO);
		bulletPath.AddPoint(end);
		particles2D.Position = end;
		bulletAnimationPlayer.Play("Shoot");
		
	}
	
	public void shoot(Vector2 start, Vector2 end) {
		
		bulletPath.ClearPoints();
		bulletPath.AddPoint(start);
		bulletPath.AddPoint(end);
		particles2D.Position = end;
		bulletAnimationPlayer.Play("Shoot");
		
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		bulletPath = GetNode<Line2D>("BulletPath");
		bulletAnimationPlayer = GetNode<AnimationPlayer>("BulletAnimationPlayer");
		particles2D = GetNode<Particles2D>("BulletEndParticle");
		
	}
}



