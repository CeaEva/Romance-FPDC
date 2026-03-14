using Godot;
using System.Collections.Generic;
using Resources;

namespace Combat{


    public partial class SubMenuSelect : CombatMenu
    {
        private enum SelectionMode
        {
            EnemyTarget,
            ExtraTurnRecipient
        }

        string _action;
        CombatMenu _parentMenu;
        SelectionMode _selectionMode = SelectionMode.EnemyTarget;
        List<IActor> _enemyList = new();
        List<IActor> _partyTargets = new();


        public override void _Ready()
        {               
            _isActive = false;
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            _battleManager.ActorsReady += OnActorsReady;
            _battleManager.PlayerTurnFinished += GetCursorElements; 
            _battleManager.ExtraTurnSelectionRequested += BeginExtraTurnSelection;
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
            {
                UsePressed();
                GetViewport().SetInputAsHandled();
                return;
            }

            if (@event.IsActionPressed("Cancel"))
            {
                CloseSelection();
                _parentMenu?.ShowMainMenu(_player);
                GetViewport().SetInputAsHandled();
            }



        }
        


        public void GetCursorElements()
        {
            RefreshEnemyList();
            _cursor.Clear();
            _cursorIndex = 0;
            _selectionMode = SelectionMode.EnemyTarget;
                 
            for (int i = 0; i < _enemyList.Count; i++)
                _cursor.Add(_enemyList[i].Name);

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
            IsActive = true;
            DrawMenu(_cursor, _optionsLabel);
        }

        protected override void UsePressed()

        {
            if (_selectionMode == SelectionMode.ExtraTurnRecipient)
            {
                GrantExtraTurnToSelectedPlayer();
                return;
            }

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
            _ = _battleManager.EnqueueAction(action);

        }

        private void BeginExtraTurnSelection()
        {
            RefreshPartyList();
            _selectionMode = SelectionMode.ExtraTurnRecipient;
            _cursor.Clear();
            _cursorIndex = 0;

            foreach (var actor in _partyTargets)
                _cursor.Add(actor.Name);

            if (_cursor.Count == 0)
            {
                GD.PrintErr("Extra turn selection requested, but no selectable party members were found.");
                CloseSelection();
                return;
            }

            IsActive = true;
            DrawMenu(_cursor, _optionsLabel);
        }

        private void GrantExtraTurnToSelectedPlayer()
        {
            RefreshPartyList();
            _cursorIndex = Mathf.Clamp(_cursorIndex, 0, Mathf.Max(0, _partyTargets.Count - 1));

            if (_partyTargets.Count == 0)
            {
                GD.PrintErr("GrantExtraTurnToSelectedPlayer called with no available players.");
                CloseSelection();
                return;
            }

            var chosen = _partyTargets[_cursorIndex];
            chosen.Atb = 100;
            chosen.State = CombatState.Wait;

            if (chosen is PlayerActor chosenPlayer)
            {
                SetActivePlayer(chosenPlayer);
                chosenPlayer.State = CombatState.Menu;
                _parentMenu?.ShowMainMenu(chosenPlayer);
            }

            CloseSelection();
        }

        public new void SetActivePlayer(PlayerActor player)
        {
            if (player == null)
                return;

            _player = player;
        }

        public void CloseSelection()
        {
            _selectionMode = SelectionMode.EnemyTarget;
            IsActive = false;
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

        private void RefreshPartyList()
        {
            if (_battleManager == null)
            {
                _partyTargets = new List<IActor>();
                return;
            }

            var refreshedList = new List<IActor>();
            foreach (var actor in _battleManager.PlayerList)
            {
                if (!IsActorSelectable(actor))
                    continue;

                refreshedList.Add(actor);
            }

            _partyTargets = refreshedList;
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
