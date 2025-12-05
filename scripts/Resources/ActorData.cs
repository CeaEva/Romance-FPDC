using Godot;
using System;

namespace Resources
{
    [GlobalClass]

    public partial class ActorData : Resource
    {
        [Export]public string Name { get; set; }
        [Export] public DummyBrain Brain { get; set; }
        [Export] public int MaxHp { get; set; }
        [Export] public int MaxDp { get; set; }
        [Export] public int CurrentHp { get; set; } // only for player actors
        [Export] public int CurrentDp { get; set; }

        [Export] public int Str { get; set; }
        [Export] public int Mag { get; set; }
        [Export] public int Vit { get; set; }
        [Export] public int Con { get; set; }
        [Export] public int Spd { get; set; }
        [Export] public int Lck { get; set; }


        public ActorData() : this(0, 0, 0, 0, 0, 0, 0, 0) { }

        public ActorData(int maxHp, int maxDp, int str, int mag, int vit, int con, int spd, int lck)
        {

            MaxHp = maxHp;
            MaxDp = maxDp;
            CurrentHp = maxHp;
            CurrentDp = maxDp;
            Str = str;
            Mag = mag;
            Vit = vit;
            Con = con;
            Lck = lck;

        }

        public int StandardAttack(DummyEnemy Target, ActorData Attacker)
        {
            var damage = Target.CurrentHp - Attacker.Str;
            GD.Print("CurrentHP: " + Target.CurrentHp + "MaxHP: " + Target.EnemyStats.MaxHp);
            return damage;
        }


    }
}