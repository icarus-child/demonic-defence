using Godot;

public partial class Barrier : Sprite2D
{

	public int Health { get => _health; }
	[Export] private int _health = 100;
	public int TargetingPriority { get => _targetingPriority; }
	private int _targetingPriority = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void TakeDamage(int damage)
	{
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
	}
	
	private void Die()
	{
		QueueFree();
	}
}
