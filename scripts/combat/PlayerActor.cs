using Godot;
using Resources;

namespace Combat
{
    public interface IActor
    {
        CombatState State { get; set; }
        int Atb { get; set; }
        int CurrentHp { get; set; }
        string Name { get; set; }
        ActorData Stats { get; set; }
        int StanceValue { get; set; }
        bool StanceBroken { get; set; }
        public void AddStance(int damage, IActor actor);
        public void Damage(int damage); 

    }
    public enum CombatState
    {

        Wait,
        Menu,
        Select,
        Queued,
        Action,
        Dead,

    }
    public partial class PlayerActor : Node, IActor
    {
        public ActorData Stats
        {
            get => _playerStats;
            set => _playerStats = value;
        }
        public int CurrentHp
        {
            get => _currentHp;
            set => _currentHp = value;
        }
        public new string Name
        {
            get => base.Name;      // StringName -> string via implicit conversion
            set => base.Name = value; // string -> StringName via implicit conversion
        }

        public CombatState State { get; set; }
        public int Atb { get; set; }
        public int StanceValue { get; set; }
        public bool StanceBroken { get; set; }
        private CombatMenu _menu;
        private ActorData _playerStats;
        private DummyBrain _brain;
        private int _currentHp;
        private int _targetHp;
        private bool _isDraining;
        [Export] RichTextLabel _hpLabel; 
            
        

        public override void _Ready()
        {
            _menu = GetNodeOrNull<CombatMenu>("%BattleMenu");
            var statsResource = GD.Load<ActorData>("res://data/PlayerActors/ElleStats.tres");
            _playerStats = (ActorData)statsResource.Duplicate();
            AddToGroup("PlayerGroup");
            Name = _playerStats.Name;
            _currentHp = _playerStats.CurrentHp;
            _brain = _playerStats.Brain;
            _hpLabel.AppendText($"HP: {CurrentHp}");
            _isDraining = false;
        }

        public override void _Process(double delta)
        {
            if (State != CombatState.Wait)
                return;
        }


        public void StateControl(CombatState newState)
        {
            if (State == newState)
                return;

            State = newState;
            switch (State)
            {
                case CombatState.Menu:
                    _menu?.ShowMainMenu(this);
                    break;
                case CombatState.Select:
                    break;
                case CombatState.Queued:

                
                    break;



            }
            

        }

        public async void Damage(int damage)
        {
            var tree = GetTree();
            if (tree == null || damage <= 0)
                return;
            
            if (_isDraining)
            {
                _targetHp -= damage;
                return;
            }
            else{
                _targetHp = CurrentHp - damage;
                _isDraining = !_isDraining;
            }
            
            var delta = (float)GetProcessDeltaTime();
            var baseDrain = 30f;
            var vitScale = 0.2f;
            var hpPerSec = baseDrain / (1f + Stats.Vit * vitScale);
            var hpStep = hpPerSec * (float)delta;
            var currentHpF = (float)CurrentHp;
            
            while (CurrentHp > _targetHp)
            {

                CurrentHp = Mathf.RoundToInt(currentHpF -= hpStep);

                await ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                
                _hpLabel.Clear();
                _hpLabel.AppendText($"HP: {CurrentHp}");
                GD.Print(CurrentHp);

            }
            _isDraining =! _isDraining;
            return;

        }

        public void AddStance(int damage, IActor caller)
        {
            StanceValue += (damage/2) + caller.Stats.Spd;
        }

    }
}
