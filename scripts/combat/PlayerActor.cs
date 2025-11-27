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
        NinePatchRect menu;
        ActorData _playerStats;
        DummyBrain brain;
        int _currentHp;

        
        
        public override void _Ready()
        {
            menu = GetNodeOrNull<NinePatchRect>("../PlayerActor/ElleTab/BattleMenu");
            _playerStats = (ActorData)NewSave.I.ElleStats;
            AddToGroup("PlayerGroup");
            Name = _playerStats.Name;
            _currentHp = _playerStats.CurrentHp;
            brain = _playerStats.Brain;

        }

        public override void _Process(double delta)
        {
            
            StateControl();

        }


        private void StateControl()
        {

            switch (State)
            {
                case CombatState.Menu:
                    menu.Visible = true;
                    break;
                case CombatState.Action:
                    break;
            }

        }

    }
}