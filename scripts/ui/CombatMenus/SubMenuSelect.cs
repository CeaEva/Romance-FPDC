using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
using References;

namespace Combat{


    public partial class SubMenuSelect : CombatMenu
    
    {
        [Signal]
        public delegate void ActionContextEventHandler(ActionContext action); 
        string _action;
        ActionReference _actDictionary;


        public override void _Ready()
        {               
            //base._Ready();
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            GD.Print("SubMenuSelect Ready, EnemyList.Count BEFORE = " + _battleManager.EnemyList.Count);  
            _battleManager.ActorsReady += OnActorsReady; 
            _battleMenu = GetNodeOrNull<SubMenuSelect>("%SubMenuSelect");
            _optionsLabel = GetNodeOrNull<RichTextLabel>("%TargetSelectLabel");
            if (_battleManager.EnemyList.Count > 0)
                OnActorsReady();
        }

        public override void _Process(double delta)
        {

        }

        public override void _UnhandledInput(InputEvent @event)
        {

            if (!IsActive)

                return;

            if (@event.IsActionPressed("MoveUp"))
            {

                _cursorIndex = (_cursorIndex - 1 + _cursor.Count) % _cursor.Count;
                DrawMenu(_cursor, _optionsLabel);

            }

            if (@event.IsActionPressed("MoveDown"))
            {
                _cursorIndex = (_cursorIndex + 1) % _cursor.Count;
                DrawMenu(_cursor, _optionsLabel);
            }



        }
        


        public void GetCursorElements()
        {
                
            for (int i = 0; i < _enemyList.Count; i++)
            {
                _cursor.Add(_enemyList[i].Name);
                GD.Print(_enemyList[i]);
            }



        }




    private void OnActorsReady()
        {
            
            _enemyList = _battleManager.EnemyList;
            GetCursorElements();
            GD.Print("Cursor.Count AFTER GetCursorElements = " + _cursor.Count);
            //DrawMenu(_cursor, _optionsLabel);

        }
     
    public void Activate(string action)
        {
            if (_cursor.Count == 0 && _battleManager != null)
            {
                _enemyList = _battleManager.EnemyList;
                GetCursorElements();
            }
            IsActive = true;
            _action = action;
        }

    protected override void UsePressed()

        {
            var selectedAction = _actDictionary.ActionDictionary[_action];
            IsActive = false;
            var targets = new List<IActor>
            {
                _enemyList[_cursorIndex]
            };
            ActionContext action = new(targets, _player, selectedAction);
            //var action = new ActionAttack();
            //_player.GetAction(action);
            _player.StateControl(CombatState.Queued);
            EmitSignal(SignalName.ActionContext, action);

        }


    }

}
