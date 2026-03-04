using Godot;
using Combat;
using Resources;

public partial class DummyEnemy : Node, IActor
{

    [Export] public string StatsPath;
    [Export] public DummyBrain Brain { get; set; }
    public CombatState State { get; set; }
    public int Atb { get; set; }
    public int CurrentHp
    {
        get => _currentHp;
        set => _currentHp = value;
    }
    public ActorData Stats
    {
        get => _enemyStats;
        set => _enemyStats = value;
    }
    public new string Name
    {
        get => base.Name;      // StringName -> string via implicit conversion
        set => base.Name = value; // string -> StringName via implicit conversion
    }
    public int StanceValue { get; set; }
    public bool StanceBroken { get; set; }
    private ActorData _enemyStats;
    private int _currentHp;
    private TextureRect _sprite;
    private BattleManager _battleManager;

    public override void _Ready()
    {
       
        _sprite = GetNodeOrNull<TextureRect>("EnemySprite");
        if (_sprite != null)
            _sprite.Visible = true;

        var baseStats = GD.Load<ActorData>(StatsPath);
        _enemyStats = (ActorData)baseStats.Duplicate();
        CurrentHp = _enemyStats.MaxHp;
        AddToGroup("EnemyGroup");
        _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
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
            State = CombatState.Dead;
            if (!IsQueuedForDeletion())
                GD.Print("Queue free");
            QueueFree();
            return;
        }

        switch (State)
        {
            case CombatState.Wait:
                break;
            case CombatState.Queued:
                _battleManager?.EnqueueAction(Brain.Tick(this, _battleManager));
                State = CombatState.Wait;
                Atb = 0;
                break;

        }

    }

   public async void Damage(int damage)
        {
            CurrentHp -= damage;
        }

    public void AddStance(int damage, IActor caller)
        {
            StanceValue += (damage/2) + caller.Stats.Spd;
        }

}
