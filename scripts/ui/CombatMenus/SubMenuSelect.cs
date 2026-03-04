using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
using Resources;
using System.Threading.Tasks;

namespace Combat{


    public partial class SubMenuSelect : CombatMenu
    {
        [Signal]
        public delegate void ActionSendEventHandler(ActionContext action); 
        string _action;
        CombatMenu _parentMenu;


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

        }

        public override void _UnhandledInput(InputEvent @event)
        {

            if (!IsActive)

                return;

            if (_cursor.Count == 0)
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
            RefreshEnemyList();
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
            
            GetCursorElements();

        }
     
        public void ActionSelect(string action)
        {
            GetCursorElements();
            _action = action;
        }

        protected override void UsePressed()

        {
            RefreshEnemyList();
            _cursorIndex = Mathf.Clamp(_cursorIndex, 0, Mathf.Max(0, _enemyList.Count - 1));

            if (_cursor.Count == 0 || _enemyList.Count == 0)
            {
                GD.PrintErr("UsePressed called with no available targets.");
                return;
            }

            var selectedAction = ActionReference.ActionDictionary[_action];
            IsActive = false;
            var clampedIndex = Mathf.Clamp(_cursorIndex, 0, _enemyList.Count - 1);
            var targets = new List<IActor> { _enemyList[clampedIndex] };
            ActionContext action = new(targets, _player, selectedAction);
            _player.StateControl(CombatState.Queued);
            ActionSend(action);
            
            void ActionSend(ActionContext action) => _battleManager.EnqueueAction(action);

        }

        private void RefreshEnemyList()
        {
            if (_battleManager == null)
            {
                _enemyList = new List<IActor>();
                return;
            }

            var refreshedList = new List<IActor>();
            foreach (var actor in _battleManager.EnemyList)
            {
                if (!IsActorSelectable(actor))
                    continue;

                refreshedList.Add(actor);
            }

            _enemyList = refreshedList;
        }

        private static bool IsActorSelectable(IActor actor)
        {
            if (actor == null || actor.State == CombatState.Dead)
                return false;

            if (actor is not GodotObject godotObject)
                return true;

            if (!GodotObject.IsInstanceValid(godotObject))
                return false;

            if (actor is Node node && node.IsQueuedForDeletion())
                return false;

            return true;
        }




    }

}
