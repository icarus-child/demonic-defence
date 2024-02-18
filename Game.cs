using Godot;
using System;
using System.Collections.Generic;
using DemonTowerDefense;

public partial class Game : Node2D
{
	[Export]
	private static int _cost = 7;
	private static int _souls = _cost * 2;
	private readonly Dictionary<PackedScene, float> _humans = new ();
	private readonly Dictionary<PackedScene, float> _demons = new ();
	private readonly Random _rng = new ();

	private StaticBody2D _portal;
	private CameraControl _camera;
	private static Label _soulInfo;
	public static Entity Selected;

	private void RegisterHuman(String scene, float spawnChance)
	{
		_humans.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	private void RegisterDemon(String scene, float spawnChance)
	{
		_demons.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	public override void _Ready()
	{
		_portal = GetNode<StaticBody2D>("TileMap/Portal");
		_camera = GetNode<CameraControl>("Camera2D");
		_soulInfo = GetNode<Label>("Souls");

		RegisterDemon("res://characters/demons/LilGuy.tscn", 0.3f);
		RegisterDemon("res://characters/demons/AngelThing.tscn", 0.2f);

		RegisterHuman("res://characters/humans/Farmer.tscn", 0.3f);
		RegisterHuman("res://characters/humans/SaltyBoi.tscn", 0.25F);
		RegisterHuman("res://characters/humans/Wizard.tscn", 0.1F);
		SpawnHuman();
	}

	public static void AddSoul()
	{
		_souls++;
		_soulInfo.Text = "Souls: " + _souls;
	}

	private void SpawnDemon()
	{
		if (_souls < _cost) return;
		_souls -= _cost;

		Entity demon = _demons.RandomElementByWeight(e => e.Value).Key.Instantiate<Entity>();
		GetNode("TileMap").AddChild(demon.Cursor);
		_portal.AddChild(demon);
	}

	private void SpawnHuman()
	{
		Entity human = _humans.RandomElementByWeight(e => e.Value).Key.Instantiate<Entity>();
		human.Portal = _portal;
		Node node = GetNode("TileMap/Spawn");
		node.GetNode<Marker2D>(_rng.Next(1, node.GetChildCount() + 1).ToString()).AddChild(human);
	}
}
