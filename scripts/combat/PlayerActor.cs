using Godot;
using Resources;
using System;

namespace Combat
{
    public interface IActor
    {
        CombatState State { get; set; }
        int Atb { get; set; }
        int CurrentHp { get; set; }
        string Name { get; set; }
        ActorData Stats { get; set; }

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
        private CombatMenu _menu;
        private ActorData _playerStats;
        private DummyBrain _brain;
        private int _currentHp;
        private IAction _queuedAction;

        public override void _Ready()
        {
            _menu = GetNodeOrNull<CombatMenu>("%BattleMenu");
            var statsResource = GD.Load<ActorData>("res://data/PlayerActors/ElleStats.tres");
            _playerStats = (ActorData)statsResource.Duplicate();
            AddToGroup("PlayerGroup");
            Name = _playerStats.Name;
            _currentHp = _playerStats.CurrentHp;
            _brain = _playerStats.Brain;

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
                    _menu?.Activate(_menu);
                    break;
                case CombatState.Select:
                   // _menu.Visible = false;
                    break;
                case CombatState.Queued:

                
                    break;



            }
            

        }

        public void GetAction(IAction action) => _queuedAction = action; //Currently unused
        

    }
}
