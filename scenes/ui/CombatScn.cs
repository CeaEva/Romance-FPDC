using Godot;
using System;
using System.Collections.Generic;
using NewSaveStats;


public enum CombatState
{
    Attack,
    Selection
}
public partial class CombatScn : Control
{

    //public list<Enemy> EnemyList = new();
    int _cursor = 0;
    CombatState _state;
    DummyEnemy _dummyEnemy;
    int _cursorIndex;
    NewSaveData _Player = NewSave.I.ElleStats;

    public override void _Ready()
    {

        _state = CombatState.Attack;

    }

    public override void _UnhandledInput(InputEvent @event)
    {

        if (@event.IsActionPressed("Confirm"))
        {
            switch (_cursor)
            {

                case 0:
                    _state = _state = CombatState.Attack;
                    StateChange();
                    break;

            }
        }

    }

    private void StateChange()
    {

        switch (_state)
        {


            case CombatState.Attack:

                //_Player.Attack();
                GD.Print("Am attac >:3");
                break;
            
        }

        

    }
    
}
