using Godot;
using Combat;
using System;

public partial class HpLabel : RichTextLabel
{

    public override void _Ready()
    {
        var Player = GetNodeOrNull<PlayerActor>("%PlayerActor");
        this.Text = "HP:" + Player.CurrentHp;
    }
    
}
