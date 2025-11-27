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

        public void Tick(DummyEnemy me, SceneTree playerGroup)
        {
            BasicAttack(me, playerGroup);
        }

    
     private void BasicAttack(DummyEnemy me, SceneTree tree)
     
        {
            // Get all nodes in the "Players" group
            var nodes = tree.GetNodesInGroup("Players"); // Godot.Collections.Array<Node>

            List<int> actorHealth = new();
            List<PlayerActor> actors = new();

            // First pass: collect actors + their HP
            foreach (Node node in nodes)
            {
                if (node is PlayerActor a)
                {
                    actors.Add(a);
                    actorHealth.Add(a.CurrentHp);
                }
            }

            if (actors.Count == 0)
                return;

            // Simple example: target the lowest HP actor
            int minHpIndex = 0;
            for (int i = 1; i < actorHealth.Count; i++)
            {
                if (actorHealth[i] < actorHealth[minHpIndex])
                    minHpIndex = i;
            }

            PlayerActor target = actors[minHpIndex];
            target.CurrentHp -= me.EnemyStats.Str;
            GD.Print("HP is " + target.CurrentHp);
        }

    }


    
}
