using Godot;
using System;
using System.Collections.Generic;
using Resources;

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
        
        MenuState _state;
        NinePatchRect _battleMenu;

        DummyEnemy _dummyEnemy;
        BattleManager _battleManager;
        List<string> _cursor = ["Attack", "Items"];
        int _cursorLen;
        int _cursorIndex;
        RichTextLabel _optionsLabel;
        PlayerActor _player;

        public override void _Ready()
        {

            _battleMenu = GetNodeOrNull<NinePatchRect>("%BattleMenu");
            _battleMenu.Visible = false;
            _state = MenuState.Main;
            _dummyEnemy = GetNodeOrNull<DummyEnemy>("%DummyEnemy");
            _optionsLabel = GetNodeOrNull<RichTextLabel>("%BattleOptionsLabel");
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _cursorLen = _cursor.Count;


        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("Confirm"))
            {
                switch (_cursorIndex)
                {
                    case 0:
                        _state = MenuState.StandardAttack;
                        break;
                    case 1:
                        _state = MenuState.Items;
                        break;
                }
            }
            if (@event.IsActionPressed("MoveUp"))
            {
                
                _cursorIndex = (_cursorIndex - 1 + _cursor.Count) % _cursor.Count;
                CursorUpdate();
                GD.Print(_cursorIndex);
            }
            if (@event.IsActionPressed("MoveDown"))
            {
                
                _cursorIndex = (_cursorIndex + 1) % _cursor.Count;
                CursorUpdate();
            }
        }    

            

        private void CursorUpdate()
        {

            _optionsLabel.Clear();

            switch (_cursor[_cursorIndex])
            {
                case "Attack":
                    _optionsLabel.AppendText(">Attack \n Items");
                    break;
                case "Items":
                    _optionsLabel.AppendText("Attack \n >Items");
                    break;
            }
        }

        

    }
}


