using Godot;
using Combat;
using System;
using Resources;
using System.Threading.Tasks.Dataflow;

public partial class DummyEnemy : Node, IActor
{

    [Export]public string StatsPath;
    public CombatState State { get; set; }
    public int Atb { get; set; }
    public int CurrentHp
    {
        get => _currentHp;  set
        {
            _currentHp = value;
        }
    }
    public ActorData EnemyStats
    {
        get => _enemyStats; private set
        {
            _enemyStats = value;
            // mirror into your singleton if you want
        }
    }
    ActorData _enemyStats;
    int _currentHp;
    TextureRect _sprite;

    public override void _Ready()
    {
       
       _sprite = GetNodeOrNull<TextureRect>("../DummyEnemy/EnemySprite");
       _sprite.Visible = true;
        var baseStats = GD.Load<ActorData>(StatsPath);
        EnemyStats = (ActorData)baseStats.Duplicate();
        CurrentHp = EnemyStats.MaxHp;
        AddToGroup("EnemyGroup");
        Name = _enemyStats.Name;
        

    }

    public override void _Process(double delta)
    {
        if (CurrentHp > 0) return;
        else
        {
            QueueFree();
        }

    }
    
}
