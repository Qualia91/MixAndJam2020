using Godot;
using System;

public class EnemyDamageTaken : Node2D
{
	private Particles2D particles2D;
	private Random rnd = new Random();

	public void action() {
		
		particles2D.Position = new Vector2(rnd.Next(10) - 5, rnd.Next(30) - 15);
		particles2D.Emitting = true;
		
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		particles2D = GetNode<Particles2D>("BloodParticles");
		
	}
}
