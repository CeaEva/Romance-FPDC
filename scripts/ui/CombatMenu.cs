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
        List<DummyEnemy> _enemyList;
        List<string> _cursor = ["Attack", "Items"];
        int _cursorLen;
        int _cursorIndex;
        int _subCursorIndex;
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
            _enemyList = _battleManager.EnemyList;
            DrawMenu(_cursor);


        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("Confirm"))
            {
                UsePressed();
            }
            if (@event.IsActionPressed("MoveUp"))
            {
                
                _cursorIndex = (_cursorIndex - 1 + _cursor.Count) % _cursor.Count;
                //LabelUpdate();
                GD.Print(_cursorIndex);
            }
            if (@event.IsActionPressed("MoveDown"))
            {
                
                _cursorIndex = (_cursorIndex + 1) % _cursor.Count;
                DrawMenu(_cursor, @event);
            }
            if (@event.IsActionPressed("MoveRight"))
            {

                DrawMenu(_cursor, @event);
                
            }
        }    

    
    
        private void UsePressed()
        {

            switch (_cursorIndex)
            {
                case 0:
                    _state = MenuState.StandardAttack;
                    _player.State = CombatState.Select;
                    break;
                case 1:
                    _state = MenuState.Items;
                    break;
            }

        }

       

        private void DrawMenu(List<string> elements, InputEvent @event = "null")
        {
            _optionsLabel.Clear();

            foreach (string e in elements)
            {
                if (elements[_cursorIndex] == e)
                    _optionsLabel.AppendText(">" + e + "\n");
                else
                    _optionsLabel.AppendText(e + "\n");

            }

            if (@event.IsActionPressed("MoveRight") || @event.IsActionPressed("MoveLeft"))
            {
                switch (_state)
                {
                    case MenuState.Main :
                        //TODO: add select logic
                        break;
                    case MenuState.StandardAttack:
                        _subCursorIndex = (_subCursorIndex + 1) % elements.Count;
                        GD.Print(_enemyList[_subCursorIndex].Name);
                         break;


                }

            }


        }

    }
}


