using Godot;
using System;

public partial class HpLabel : RichTextLabel
{

    public override void _Ready()
    {
        this.Text = "HP:" + NewSave.I.ElleStats.CurrentHp;
    }
    
}
