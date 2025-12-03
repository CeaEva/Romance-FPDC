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

        public void Tick(DummyEnemy me, List<PlayerActor> playerGroup)
        {
            BasicAttack(me, playerGroup);
            GD.Print("Enemy: Ticking");
        }

    
     private void BasicAttack(DummyEnemy me, List<PlayerActor> tree)
     
        {
            
            if (tree.Count == 0)
            {
                GD.Print("No actors to attack");
                return;
            }

            List<int> actorHealth = new();

            // First pass: collect actors + their HP
            foreach (Node node in tree)
            {
                if (node is PlayerActor a)
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

            PlayerActor target = tree[minHpIndex];
            target.CurrentHp -= me.EnemyStats.Str;
            GD.Print("HP is " + target.CurrentHp);
        }

    }


    
}
