using Godot;
using Combat;
using System;
using Resources;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

public partial class DummyEnemy : Node, IActor
{

    [Export]public string StatsPath;
    [Export]public DummyBrain Brain { get; set; }
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
    List<PlayerActor> _playerList;
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
        var BattleManager = GetNodeOrNull<BattleManager>("%BattleManager");
        _playerList = BattleManager.PlayerList;
        Name = _enemyStats.Name;
        State = CombatState.Wait;
        

    }

    public override void _Process(double delta)
    {
        StateManager();
        if (CurrentHp > 0) return;
        else
        {
            QueueFree();
        }

    }
    
    private void StateManager()
    {
        switch (State)
        {
            case CombatState.Wait:
                break;
            case CombatState.Queued:
                Brain.Tick(this, _playerList);
                State = CombatState.Wait;
                Atb = 0;
                break;

        }

    }
}
