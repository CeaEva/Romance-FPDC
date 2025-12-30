using Godot;
using Combat;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;


namespace Resources
{
    [GlobalClass]
    public partial class DummyBrain : Resource
    {

        public IAction Tick(DummyEnemy me, List<IActor> playerGroup)
        {
            GD.Print("Enemy: Ticking");
            return BasicAttack(me, playerGroup);
        }

    
     private IAction BasicAttack(DummyEnemy me, List<IActor> tree)
     
        {
            
            if (tree.Count == 0)
            {
                GD.Print("No actors to attack");
                return new ActionAttack();
            }

            List<int> actorHealth = new();

            // First pass: collect actors + their HP
            foreach (var actor in tree)
            {
                if (actor is PlayerActor a)
                {
                    actorHealth.Add(a.CurrentHp);
                }
            }

            // Simple example: target the lowest HP actor
            int minHpIndex = 0;
            for (int i = 1; i < actorHealth.Count; i++)
            {
                if (actorHealth[i] < actorHealth[minHpIndex])
                    minHpIndex = i;
            }

            List<IActor> targets = new List<IActor>();
            targets.Add(tree[minHpIndex]);
            //targets.CurrentHp -= me.EnemyStats.Str;
            //GD.Print("HP is " + targets.CurrentHp);
            return new ActionAttack();

        }

    }


    
}
