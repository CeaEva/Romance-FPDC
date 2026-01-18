using Godot;
using System;
using System.Collections.Generic;
using Resources;
using System.Diagnostics;

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
        int _cursorLen;
        protected int _cursorIndex;
        int _subCursorIndex;
        MenuState _state;
        protected NinePatchRect _battleMenu;

        DummyEnemy _dummyEnemy;
        protected BattleManager _battleManager;
        protected List<IActor> _enemyList;
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
            _battleMenu = GetNodeOrNull<NinePatchRect>("%BattleMenu");
            _state = MenuState.Main;
            _dummyEnemy = GetNodeOrNull<DummyEnemy>("%DummyEnemy");
            _battleManager = GetNodeOrNull<BattleManager>("%BattleManager");
            _player = GetNodeOrNull<PlayerActor>("%PlayerActor");
            _subMenuSelect = GetNodeOrNull<SubMenuSelect>("%SubMenuSelect");
            _cursorLen = _cursor.Count;
            _enemyList = _battleManager.EnemyList;
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

            switch (_cursorIndex)
            {
               case 0: //Attack
                    //add submenu node, set state to select Maybe flag
                    //var subMenuSelect = new SubMenuSelect();
                    //AddChild(_subMenuSelect);
                    _state = MenuState.StandardAttack;
                    _player.StateControl(CombatState.Select);
                    _subMenuSelect.ActionSelect(_cursor[_cursorIndex]);
                    Activate(this, _subMenuSelect);
                    break;
                case 1:
                    // same, but set skills state and pass skills list
                    break; 
            }

            switch (_state)
            {
                case MenuState.StandardAttack:
                   // _battleMenu.Visible = false;
                    break;
            }

        }

       

        protected void DrawMenu(List<string> elements, RichTextLabel label)
        {



            _optionsLabel.Clear();

            foreach (string e in elements)
            {
                if (elements[_cursorIndex] == e)
                    label.AppendText(">" + e + "\n");
                else
                    label.AppendText(e + "\n");

            }



        }

        public void Activate(CombatMenu menu, CombatMenu nextMenu = null){


            menu.IsActive = !menu.IsActive;
            GD.Print(menu.IsActive);

            if (nextMenu != null)
                Activate(nextMenu);
                
        }




    }
}

