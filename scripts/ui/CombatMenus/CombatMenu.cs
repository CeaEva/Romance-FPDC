using Godot;
using System.Collections.Generic;

namespace Combat
{
    
    public enum MenuState
    {
        
        Wait,
        Main,
        StandardAttack,
        Skills,
        Items,
        Run,
    

    }

    public partial class CombatMenu : NinePatchRect
    {
        
        protected bool IsActive
        {
            get => _isActive;

            set
            {
                if (_isActive == value)
                    return;

                _isActive = value;

 
                Visible = value;

                if (value)
                    DrawMenu(_cursor, _optionsLabel);
                else
                    _optionsLabel?.Clear();
            }

        }
        protected bool _isActive;
        protected int _cursorIndex;
        MenuState _state;
        protected BattleManager _battleManager;
        protected List<string> _cursor = new List<string>();
        protected RichTextLabel _optionsLabel;
        protected PlayerActor _player;

        SubMenuSelect _subMenuSelect;

        public override void _EnterTree()
        {

            _optionsLabel = GetNodeOrNull<RichTextLabel>("%BattleOptionsLabel");
            
        }


        public override void _Ready()
        {
            _isActive = false;
            _state = MenuState.Main;
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _subMenuSelect = GetNodeOrNull<SubMenuSelect>("%SubMenuSelect");
            _cursor = ["Attack", "Items"];


        }

        public override void _UnhandledInput(InputEvent @event)
        {

            if (!IsActive)
                return;

            if (@event.IsActionPressed("Confirm"))
            {
                UsePressed();
                GD.Print(_cursorIndex);
            }
            if (@event.IsActionPressed("MoveUp"))
            {
                
                _cursorIndex = (_cursorIndex - 1 + _cursor.Count) % _cursor.Count;
                DrawMenu(_cursor, _optionsLabel);
                GD.Print(_cursorIndex);
            }
            if (@event.IsActionPressed("MoveDown"))
            {
                
                _cursorIndex = (_cursorIndex + 1) % _cursor.Count;
                DrawMenu(_cursor, _optionsLabel);
            }
            if (@event.IsActionPressed("MoveRight"))
            {

                DrawMenu(_cursor, _optionsLabel);
                
            }
            if (@event.IsActionPressed("MoveLeft"))
            {
                   
                DrawMenu(_cursor, _optionsLabel);

            }
        }    

    
    
        protected virtual void UsePressed()
        {   
            _state = MenuState.Main;

            switch (_cursorIndex)
            {
               case 0: //Attack
                    _state = MenuState.StandardAttack;
                    break;
                case 1:
                    // same, but set skills state and pass skills list
                    break; 
            }

            switch (_state)
            {
                case MenuState.StandardAttack:
                    _player.StateControl(CombatState.Select);
                    IsActive = false;
                    _subMenuSelect?.ActionSelect(_cursor[_cursorIndex]);
                    break;
            }

        }

       

        protected void DrawMenu(List<string> elements, RichTextLabel label)
        {
            if (label == null)
                return;

            label.Clear();

            foreach (string e in elements)
            {
                if (elements[_cursorIndex] == e)
                    label.AppendText(">" + e + "\n");
                else
                    label.AppendText(e + "\n");

            }

        }

        public void SetActivePlayer(PlayerActor player)
        {
            if (player == null)
                return;

            _player = player;
            _subMenuSelect?.SetActivePlayer(player);
        }

        public void ShowMainMenu(PlayerActor player = null)
        {
            if (player != null)
                SetActivePlayer(player);

            _state = MenuState.Main;
            _cursorIndex = 0;
            _subMenuSelect?.CloseSelection();
            IsActive = true;
        }


    }
}
