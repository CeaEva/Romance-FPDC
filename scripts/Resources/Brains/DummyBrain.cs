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

        public ActionContext Tick(DummyEnemy me, BattleManager battleManager)
        {
            var action = BasicAttack(me, battleManager);
            action.SelectedAction = ActionReference.ActionDictionary["Attack"];
            GD.Print("Enemy: Ticking");
            return action;
        }

    
     private ActionContext BasicAttack(DummyEnemy me, BattleManager battleManager) //<= change tree to battle manger
     
        {
            
            if (battleManager.AllActorList.Count == 0)
            {
                GD.Print("No actors to attack");
                return new ActionContext(null, null, null);
            }

            List<int> actorHealth = new();

            // First pass: collect actors + their HP
            foreach (var actor in battleManager.PlayerList)
                actorHealth.Add(actor.CurrentHp);

            // Simple example: target the lowest HP actor
            int minHpIndex = 0;
            for (int i = 1; i < actorHealth.Count; i++)
            {
                if (actorHealth[i] < actorHealth[minHpIndex])
                    minHpIndex = i;
            }

            List<IActor> targets = [battleManager.PlayerList[minHpIndex]];
            
            //targets.CurrentHp -= me.EnemyStats.Str;
            //GD.Print("HP is " + targets.CurrentHp);
            return new ActionContext(targets, me, null); //return a 

        }

    }


    
}
