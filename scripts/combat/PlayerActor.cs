using Godot;
using Overworld2d;
using Combat;
using Resources;
using System;

namespace Combat
{
    public interface IActor
    {
        CombatState State { get; set; }
        int Atb { get; set; }
        int CurrentHp { get; set; }

    }
    public enum CombatState
    {

        Wait,
        Menu,
        Select,
        Queued,
        Action,

    }
    public partial class PlayerActor : Node, IActor
    {
        public ActorData PlayerStats 
        { get => _playerStats; private set
            {
                _playerStats = value;
            } 
        }
        public int CurrentHp
        {
            get => _currentHp;  set
            {
                _currentHp = value;
            }
        }
        public CombatState State { get; set; }
        public int Atb { get; set; }
        NinePatchRect _menu;
        ActorData _playerStats;
        DummyBrain _brain;
        int _currentHp;

        
        
        public override void _Ready()
        {
            _menu = GetNodeOrNull<NinePatchRect>("%BattleMenu");
            var StatsPath = GD.Load<ActorData>("res://data/PlayerActors/ElleStats.tres");
            _playerStats = (ActorData)StatsPath.Duplicate();
            AddToGroup("PlayerGroup");
            Name = _playerStats.Name;
            _currentHp = _playerStats.CurrentHp;
            _brain = _playerStats.Brain;

        }

        public override void _Process(double delta)
        {
            if (State != CombatState.Wait)
            {

                StateControl();
                return;

            }   

            else
                return;
            
        }


        private void StateControl()
        {

            _menu.Visible = State == CombatState.Menu;

        }

    }
}
