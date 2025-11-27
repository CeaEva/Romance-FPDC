using Godot;
using System;
using Resources;

[GlobalClass]
public partial class NewSave : Node
{
    public static NewSave I { get; private set; } // Golbal Pointer Reference

    [Export] public ActorData ElleStats {  get => _elleStats ??= new ActorData(999, 5, 5, 7, 6, 7, 5, 0); set => _elleStats = value ?? new ActorData(999, 5, 5, 7, 6, 7, 5, 0);}
    
    public string EStatsPath;
    ActorData _elleStats;
    ActorData _dummyEni;

      public override void _EnterTree()
    {
        if (I != null && I != this)
            GD.PushWarning("NewSave: replacing existing singleton instance.");
        I = this; // set the singleton as early as possible
    }

    public override void _Ready()
    {
        I = this;
        EStatsPath = "res://data/PlayerActors/ElleStats.tres";
        var ElleBase = GD.Load<ActorData>(EStatsPath);
        _elleStats = (ActorData)ElleBase.Duplicate();
        GD.Print("Current hp " + _elleStats.CurrentHp);


    }
}
