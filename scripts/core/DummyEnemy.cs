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
    public ActorData Stats
    {
        get => _enemyStats; set
        {
            _enemyStats = value;
            // mirror into your singleton if you want
        }
    }
    public new string Name
        {
            get => base.Name;      // StringName -> string via implicit conversion
            set => base.Name = value; // string -> StringName via implicit conversion
        }

    List<IActor> _playerList;
    ActorData _enemyStats;
    int _currentHp;
    TextureRect _sprite;

    public override void _Ready()
    {
       
        _sprite = GetNodeOrNull<TextureRect>("EnemySprite");
        _sprite.Visible = true;
        var baseStats = GD.Load<ActorData>(StatsPath);
        _enemyStats = (ActorData)baseStats.Duplicate();
        CurrentHp = _enemyStats.MaxHp;
        AddToGroup("EnemyGroup");
        var BattleManager = GetNodeOrNull<BattleManager>("%BattleManager");
        _playerList = BattleManager.PlayerList;
        Name = _enemyStats.Name;
        GD.Print(Name + "Smile");
        State = CombatState.Wait;
        

    }

    public override void _Process(double delta)
    {
        StateManager();
        

    }
    
    private void StateManager()
    {
        if (CurrentHp <= 0)
        {
            if (!IsQueuedForDeletion())
                QueueFree();
                return;
        }

        switch (State)
        {
            case CombatState.Wait:
                break;
            case CombatState.Queued:
               // Brain.Tick(this, _playerList);
                State = CombatState.Wait;
                Atb = 0;
                break;

        }

    }
}
