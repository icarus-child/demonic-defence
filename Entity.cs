using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Entity : CharacterBody2D
{
	public enum Team
	{
		Humans,
		Demons
	}

	[Export] public Team EntityTeam { get; set; }
	[Export] private float _health;
	[Export] private float _damage;
	[Export] private double _attackCooldown;
	[Export] private bool _doesAoe;
	[Export] private float _aoeRange;
	[Export] private float _wallMultiplier;
	[Export] private float _enemyMultiplier;
	[Export] private float speed;

	[Export]
	public Node2D Portal;
	public Node2D Target;
	private NavigationAgent2D _navigationAgent;
	private Area2D _aggroRange;
	private Area2D _attackRange;
	private readonly List<Node2D> _targetOptions = new();
	private AnimatedSprite2D _sprite;
	private Timer _attackCooldownTimer;
	private bool _canAttack = false;
	
    public override void _Ready()
    {
        _attackCooldownTimer = new()
        {
            OneShot = true,
            WaitTime = _attackCooldown,
			Autostart = true
        };
		AddChild(_attackCooldownTimer);
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_aggroRange = GetNode<Area2D>("AggroRange");
		_attackRange = GetNode<Area2D>("AttackRange");
		_aggroRange.BodyEntered += body => {
			_targetOptions.Add(body);
		};
		_aggroRange.BodyExited += body => {
			_targetOptions.Remove(body);
		};
		_attackCooldownTimer.Timeout += () => {
			_canAttack = true;
		};
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("idle");
    }

    public override void _Process(double delta)
    {
		if (Target is null) {
			if (_targetOptions.Count != 0) Target = GetClosestTargetable();
			else Target = Portal;
		}

	    // Make this not shit later
	    try {
		    int i = _navigationAgent.GetCurrentNavigationPathIndex() - 2;
		    _sprite.FlipH = _navigationAgent.GetCurrentNavigationPath()[i].X > ToGlobal(Position).X; // Make sprite face the target
	    } catch (IndexOutOfRangeException e) {}

    }

	public override void _PhysicsProcess(double delta)
	{
		if (Target is null && _targetOptions.Count == 1)
		{
			Target = GetClosestTargetable();
		} else if (Target is null) {
			return;
		}

		if (!_attackRange.OverlapsBody(Target))
		{
			GD.Print("AttackMove");
			AttackMove();
		} else {
			// Attack
			if (_canAttack) {
				GD.Print("Attack");
				_sprite.Play("attack");
			}
		}
	}

	// Attack Move implementation, need just a default Move implementation
	private void Path()
	{
		// Pathing
		if (_navigationAgent.IsNavigationFinished()) return;
		Vector2 direction = ToLocal(_navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
		_sprite.Play("walk");
	}

	private void AttackMove()
	{
		if (_targetOptions.Count != 0) Target = GetClosestTargetable();
		Path();
	}
	
	private void Move()
	{
		Path();
	}

	private void CreatePath()
	{
		_navigationAgent.TargetPosition = Target.GlobalPosition;
	}

	private Node2D GetClosestTargetable()
	{
		return _targetOptions.MinBy(node =>
		{
			return node.GlobalPosition.DistanceTo(GlobalPosition);
		});
	}

	public void TakeDamage(int damage) {
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
		// calculate targetting attacker
	}
	
	private void Die()
	{
		if (EntityTeam == Team.Humans) Game.Souls++;
		QueueFree();
	}
}

	/*public override void _Input(InputEvent @event)
	{

		if (EntityTeam == Team.Humans) return;
		if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
			if (_sprite.HasPoint(mouse.Position))
		{
				GD.Print("aaaaaaaaaa");
			{
				_selected = true;
			else
			}
			{

				if (_selected)
				{
					GD.Print("pain");
					_selected = false;
					Target = Game.Cursor;
				}
			}
		}
	}*/

