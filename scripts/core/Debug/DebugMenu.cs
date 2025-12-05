using Godot;
using System;
using System.Collections.Generic;

namespace Combat{
    
    public partial class DebugMenu : CanvasLayer
    {   

        Label _listLabel;
        List<string> _debugList = new List<string>();
        int _cursorIndex;
        public override void _Ready()
        {
            _debugList.Add("Switch Scene");
            _listLabel = new Label();
            AddChild(_listLabel);
            DebugListToLabel();

        }

        public override void _Process(double delta)
        {
            CursorManager();
        }



        private void DebugListToLabel()
        {
            foreach (string s in _debugList)
            {
                _listLabel.Text += s + "\n";
            }

        }

        private void CursorManager()
        {
            switch (_cursorIndex)
            {
                case 0:
                    _listLabel.Text = ">" + _debugList[0];
                    break;


            }

        }


    }

}
