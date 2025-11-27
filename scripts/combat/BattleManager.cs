using Godot;
using System;
using System.Collections.Generic;

namespace Combat
{
    

    public partial class BattleManager : Node
    {
        List<Node> _allActors = new List<Node>();
        List<PlayerActor> _playerList = new List<PlayerActor>();
        List<DummyEnemy> _enemyList = new List<DummyEnemy>();
        //Queue _turnQueue = new();
        ProgressBar atbBar;

        public override void _Ready()
        {
            
            ActorsToList("EnemyGroup");
            ActorsToList("PlayerGroup");
            GD.Print("Enemies: " + _enemyList + " Player: " + _playerList);
            var atbTimer = GetNode<Timer>("AtbTimer");
            atbTimer.Timeout += AtbTick;
            atbBar = GetNodeOrNull<ProgressBar>("%AtbBar");
            
        }

        public override void _Process(double delta)
        {
            

        }

        private void ActorsToList(string group)
        {
            var nodes = GetTree().GetNodesInGroup(group);

            foreach (Node node in nodes)
            {
                _allActors.Add(node);

                if (node is DummyEnemy dummy)
                    _enemyList.Add(dummy);
                else if (node is PlayerActor player)
                    _playerList.Add(player);

            }


        }

        private void AtbTick()
        {
            
            foreach (PlayerActor n in _playerList)
            {
                n.Atb += n.PlayerStats.Spd;
                GD.Print(n.Atb);
                atbBar.Value = n.Atb;
            }
            StateCheck();
        }

        private void StateCheck()
        {
            var atbMax = 100; 
            foreach (PlayerActor n in _playerList)
            {
                if (n.Atb >= atbMax)
                {
                    n.State = CombatState.Menu;
                    GD.Print("Player can menu");
                }

                

            }
            


        }


    }

}