using Godot;
using System;

public partial class DcEnv : Node3D
{
        public void Funny()
        {
            GD.Print("Function succesful");

        }
   public override void _Ready()
    {
        Funny();
    }
    
}
