using Godot;
using System;
using Resources;

[GlobalClass]
public partial class NewSave : Node
{
    public static NewSave I { get; private set; } // Golbal Pointer Reference 
    public ActorData ElleStats { get => _elleStats; private set => _elleStats = value; }

    public string EStatsPath;
    ActorData _elleStats;

      public override void _EnterTree()
    {
        if (I != null && I != this)
            GD.PushWarning("NewSave: replacing existing singleton instance.");
        I = this; // set the singleton as early as possible
    }

    public override void _Ready()
    {
        EStatsPath = "res://data/PlayerActors/ElleStats.tres";              
        var ElleBase = GD.Load<ActorData>(EStatsPath);                      //This will be in for loop for all party members put into a list
        _elleStats = (ActorData)ElleBase.Duplicate();
        GD.Print("Current hp " + _elleStats.CurrentHp);


    }
}
