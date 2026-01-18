using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
using Resources;

namespace Combat{


    public partial class SubMenuSelect : CombatMenu
    
    {
        [Signal]
        public delegate void ActionSendEventHandler(ActionContext action); 
        string _action;
        CombatMenu _parentMenu;
        //ActionReference _actDictionary;


        public override void _Ready()
        {               
            _isActive = false;
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            GD.Print("SubMenuSelect Ready, EnemyList.Count BEFORE = " + _battleManager.EnemyList.Count);  
            _battleManager.ActorsReady += OnActorsReady;
            _battleManager.PlayerTurnFinished += GetCursorElements; 
            _parentMenu = GetNodeOrNull<CombatMenu>("%BattleMenu");
            _optionsLabel = GetNodeOrNull<RichTextLabel>("%TargetSelectLabel");
            if (_battleManager.EnemyList.Count > 0)
                OnActorsReady();
            //_actDictionary = new();

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

            if (@event.IsActionPressed("Confirm"))
                UsePressed();

            if (@event.IsActionPressed("Cancel"))
                Activate(_parentMenu, this);



        }
        


        public void GetCursorElements()
        {
            _cursor.Clear();
            _cursorIndex = 0;
                
            for (int i = 0; i < _enemyList.Count; i++)
            {
                _cursor.Add(_enemyList[i].Name);
                GD.Print(_enemyList[i]);
            }
            if (_cursor.Count == 0)
                _optionsLabel.Clear();


        }




    private void OnActorsReady()
        {
            
            _enemyList = _battleManager.EnemyList;
            GetCursorElements();
            GD.Print("Cursor.Count AFTER GetCursorElements = " + _cursor.Count);
            //DrawMenu(_cursor, _optionsLabel);

        }
     
    public void ActionSelect(string action)
        {
            if (_cursor.Count == 0 && _battleManager != null)
            {
                _enemyList = _battleManager.EnemyList;
                GetCursorElements();
            }
            _action = action;
        }

    protected override void UsePressed()

        {

            void ActionSend(ActionContext action) => _battleManager.EnqueueAction(action);



            var selectedAction = ActionReference.ActionDictionary[_action];
            IsActive = false;
            var targets = new List<IActor>
            {
                _enemyList[_cursorIndex]
            };
            ActionContext action = new(targets, _player, selectedAction);
            //var action = new ActionAttack();
            //_player.GetAction(action);
            _player.StateControl(CombatState.Queued);
            //EmitSignal(SignalName.ActionSend, action);
            ActionSend(action);
            

        }



    }

}
